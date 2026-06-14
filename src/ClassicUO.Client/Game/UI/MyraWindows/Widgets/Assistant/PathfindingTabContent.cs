#nullable enable
using ClassicUO.Configuration;
using ClassicUO.Game.Managers;
using Myra.Graphics2D.UI;

namespace ClassicUO.Game.UI.MyraWindows.Widgets.Assistant;

public static class PathfindingTabContent
{
    public static Widget Build()
    {
        var lang = Language.Instance.Assistant.Pathfinding;

        var root = new HorizontalStackPanel { Spacing = MyraStyle.STANDARD_SPACING };

        #region LeftSide
        var leftStack = new VerticalStackPanel { Spacing = MyraStyle.STANDARD_SPACING };

        leftStack.Widgets.Add(
            MyraCheckButton.CreateWithCallback(
                World.Instance?.Player?.Pathfinder.UseLongDistancePathfinding ?? false,
                b =>
                {
                    if (World.Instance?.Player != null)
                        World.Instance.Player.Pathfinder.UseLongDistancePathfinding = b;
                    Client.Settings?.SetAsync(SettingsScope.Global, Constants.SqlSettings.USE_LONG_DISTANCE_PATHING, b);
                },
                lang.LongDistancePathfinding,
                lang.LongDistancePathfindingTooltip));

        HorizontalStackPanel genTimeRow = MyraHSlider.SliderWithLabel(
            lang.PathfindingGenTimeMs,
            out MyraHSlider genTimeSlider,
            v =>
            {
                int ms = (int)v;
                Client.Settings?.SetAsync(SettingsScope.Global, Constants.SqlSettings.LONG_DISTANCE_PATHING_SPEED, ms);
                if (WalkableManager.Instance != null)
                    WalkableManager.Instance.TargetGenerationTimeMs = ms;
            },
            min: 1,
            max: 50,
            value: Client.Settings.Get(SettingsScope.Global, Constants.SqlSettings.LONG_DISTANCE_PATHING_SPEED, 2));
        genTimeSlider.Tooltip = lang.GenTimeTooltip;
        leftStack.Widgets.Add(genTimeRow);

        var progressLabel = new MyraLabel(lang.CacheProgressNA, MyraLabel.TextStyle.P)
        {
            Tooltip = lang.CacheProgressTooltip
        };

        void RefreshProgress()
        {
            if (WalkableManager.Instance != null)
            {
                var (current, total) = WalkableManager.Instance.GetCurrentMapGenerationProgress();
                if (total > 0)
                    progressLabel.Text = string.Format(lang.CacheProgressCurrentTotal, current, total, (float)current / total * 100f);
                else
                    progressLabel.Text = lang.CacheProgressNA;
            }
            else
            {
                progressLabel.Text = lang.CacheProgressNA;
            }
        }

        RefreshProgress();

        var common = Language.Instance.UiCommons;
        var progressRow = new HorizontalStackPanel { Spacing = MyraStyle.STANDARD_SPACING };
        progressRow.Widgets.Add(progressLabel);
        progressRow.Widgets.Add(new MyraButton(common.Refresh, RefreshProgress));
        leftStack.Widgets.Add(progressRow);

        leftStack.Widgets.Add(new MyraButton(lang.ResetCurrentMapCache, () =>
        {
            if (World.Instance != null)
                WalkableManager.Instance?.StartFreshGeneration(World.Instance.MapIndex);
            RefreshProgress();
        })
        { Tooltip = lang.ResetCurrentMapCacheTooltip });

        root.Widgets.Add(leftStack);
        #endregion

        #region RightSide

        var rightSide = new VerticalStackPanel { Spacing = MyraStyle.STANDARD_SPACING };

        HorizontalStackPanel zLevelSliderWidget = MyraHSlider.SliderWithLabel(
            lang.PathfindingZLevelDiff,
            out MyraHSlider zLevelSlider, v
                => { ProfileManager.CurrentProfile?.PathfindingZLevelDiff = (int)v; },
            1,
            50,
            ProfileManager.CurrentProfile.PathfindingZLevelDiff);
        zLevelSlider.Tooltip = lang.ZLevelSliderTooltip;

        rightSide.Widgets.Add(zLevelSliderWidget);

        root.Widgets.Add(rightSide);
        #endregion

        return root;
    }
}
