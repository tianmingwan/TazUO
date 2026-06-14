#nullable enable
using ClassicUO.Configuration;
using ClassicUO.Game.Managers;
using Myra.Graphics2D.UI;

namespace ClassicUO.Game.UI.MyraWindows.Widgets.Assistant;

public static class TitleBarTabContent
{
    public static Widget Build()
    {
        var lang = Language.Instance.Assistant.TitleBar;
        var common = Language.Instance.UiCommons;

        Profile profile = ProfileManager.CurrentProfile;

        var outer = new VerticalStackPanel { Spacing = 6 };

        outer.Widgets.Add(new MyraLabel(
            lang.HeaderDescription,
            MyraLabel.TextStyle.H3));

        outer.Widgets.Add(MyraCheckButton.CreateWithCallback(profile.EnableTitleBarStats,
            b =>
            {
                profile.EnableTitleBarStats = b;
                if (b)
                    TitleBarStatsManager.ForceUpdate();
                else
                    Client.Game.SetWindowTitle(
                        string.IsNullOrEmpty(World.Instance.Player?.Name)
                            ? string.Empty
                            : World.Instance.Player.Name);
            }, lang.EnableTitleBarStats));

        outer.Widgets.Add(new MyraSpacer(15, 5));
        outer.Widgets.Add(new MyraLabel(lang.DisplayMode, MyraLabel.TextStyle.H2));

        var previewLabel = new MyraLabel(TitleBarStatsManager.GetPreviewText(), MyraLabel.TextStyle.P);

        void SetMode(TitleBarStatsMode mode)
        {
            profile.TitleBarStatsMode = mode;
            TitleBarStatsManager.ForceUpdate();
            previewLabel.Text = TitleBarStatsManager.GetPreviewText();
        }

        var radioGroup = new VerticalStackPanel { Spacing = 4 };

        var rbText = new RadioButton
        {
            Content = new MyraLabel(lang.TextModeLabel, MyraLabel.TextStyle.P),
            IsPressed = profile.TitleBarStatsMode == TitleBarStatsMode.Text
        };
        rbText.PressedChanged += (_, _) => { if (rbText.IsPressed) SetMode(TitleBarStatsMode.Text); };

        var rbPercent = new RadioButton
        {
            Content = new MyraLabel(lang.PercentModeLabel, MyraLabel.TextStyle.P),
            IsPressed = profile.TitleBarStatsMode == TitleBarStatsMode.Percent
        };
        rbPercent.PressedChanged += (_, _) => { if (rbPercent.IsPressed) SetMode(TitleBarStatsMode.Percent); };

        var rbBar = new RadioButton
        {
            Content = new MyraLabel(lang.ProgressBarModeLabel, MyraLabel.TextStyle.P),
            IsPressed = profile.TitleBarStatsMode == TitleBarStatsMode.ProgressBar
        };
        rbBar.PressedChanged += (_, _) => { if (rbBar.IsPressed) SetMode(TitleBarStatsMode.ProgressBar); };

        radioGroup.Widgets.Add(rbText);
        radioGroup.Widgets.Add(rbPercent);
        radioGroup.Widgets.Add(rbBar);
        outer.Widgets.Add(radioGroup);

        outer.Widgets.Add(new MyraSpacer(15, 5));
        outer.Widgets.Add(new MyraLabel(common.Preview, MyraLabel.TextStyle.H2));
        outer.Widgets.Add(previewLabel);

        return outer;
    }
}
