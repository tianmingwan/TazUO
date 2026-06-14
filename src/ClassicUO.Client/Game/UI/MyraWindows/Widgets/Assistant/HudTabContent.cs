using System;
using System.Collections.Generic;
using ClassicUO.Configuration;
using ClassicUO.Game.Data;
using ClassicUO.Game.Managers;
using ClassicUO.Utility;
using Myra.Graphics2D.UI;

namespace ClassicUO.Game.UI.MyraWindows.Widgets.Assistant;

public static class HudTabContent
{
    public static Widget Build()
    {
        var lang = Language.Instance.Assistant.Hud;
        Profile profile = ProfileManager.CurrentProfile;

        var regularFlags = new List<HideHudFlags>();
        foreach (HideHudFlags flag in Enum.GetValues(typeof(HideHudFlags)))
        {
            if (flag == HideHudFlags.None || flag == HideHudFlags.All) continue;
            regularFlags.Add(flag);
        }

        var checkButtons = new Dictionary<HideHudFlags, CheckButton>();

        foreach (HideHudFlags flag in regularFlags)
        {
            checkButtons[flag] = MyraCheckButton.CreateWithCallback(ByteFlagHelper.HasFlag(profile.HideHudGumpFlags, (ulong)flag),
                b =>
                {
                    profile.HideHudGumpFlags = b ? ByteFlagHelper.AddFlag(profile.HideHudGumpFlags, (ulong)flag) : ByteFlagHelper.RemoveFlag(profile.HideHudGumpFlags, (ulong)flag);
                }, HideHudManager.GetFlagName(flag), GetTooltip(flag));
        }

        var outerStack = new VerticalStackPanel { Spacing = 6 };

        outerStack.Widgets.Add(new MyraLabel(
            lang.HeaderDescription,
            MyraLabel.TextStyle.H3));


        var grid = new MyraGrid();
        grid.AddColumn(new Proportion(ProportionType.Auto), 4);
        grid.ColumnSpacing = 12;
        for (int i = 0; i < regularFlags.Count; i++) {
            HideHudFlags flag = regularFlags[i];
            grid.AddWidget(checkButtons[flag], i / 4, i % 4);
        }
        outerStack.Widgets.Add(grid);


        var buttonRow = new HorizontalStackPanel { Spacing = 4 };
        buttonRow.Widgets.Add(new MyraButton(lang.SelectAll, () => SetAllChecked(checkButtons, profile, true)));

        var deselectBtn = new MyraButton(lang.DeselectAll, () => SetAllChecked(checkButtons, profile, false));
        StackPanel.SetProportionType(deselectBtn, ProportionType.Fill);
        buttonRow.Widgets.Add(deselectBtn);

        buttonRow.Widgets.Add(new MyraButton(lang.ToggleHudNow, () => HideHudManager.ToggleHidden(profile.HideHudGumpFlags))
        {
            Tooltip = lang.ToggleHudNowTooltip
        });
        outerStack.Widgets.Add(buttonRow);

        return outerStack;
    }

    private static void SetAllChecked(Dictionary<HideHudFlags, CheckButton> buttons, Profile profile, bool state)
    {
        profile.HideHudGumpFlags = state ? (ulong)HideHudFlags.All : 0UL;
        foreach (var (_, cb) in buttons)
            cb.IsChecked = state;
    }

    private static string GetTooltip(HideHudFlags flag)
    {
        var lang = Language.Instance.Assistant.Hud;
        return flag switch
        {
            HideHudFlags.Paperdoll => lang.PaperdollTooltip,
            HideHudFlags.WorldMap => lang.WorldMapTooltip,
            HideHudFlags.GridContainers => lang.GridContainersTooltip,
            HideHudFlags.Containers => lang.ContainersTooltip,
            HideHudFlags.Healthbars => lang.HealthbarsTooltip,
            HideHudFlags.StatusBar => lang.StatusBarTooltip,
            HideHudFlags.SpellBar => lang.SpellBarTooltip,
            HideHudFlags.Journal => lang.JournalTooltip,
            HideHudFlags.XMLGumps => lang.XmlGumpsTooltip,
            HideHudFlags.NearbyCorpseLoot => lang.NearbyCorpseLootTooltip,
            HideHudFlags.MacroButtons => lang.MacroButtonsTooltip,
            HideHudFlags.SkillButtons => lang.SkillButtonsTooltip,
            HideHudFlags.SkillsMenus => lang.SkillsMenusTooltip,
            HideHudFlags.TopMenuBar => lang.TopMenuBarTooltip,
            HideHudFlags.DurabilityTracker => lang.DurabilityTrackerTooltip,
            HideHudFlags.BuffBar => lang.BuffBarTooltip,
            HideHudFlags.CounterBar => lang.CounterBarTooltip,
            HideHudFlags.InfoBar => lang.InfoBarTooltip,
            HideHudFlags.SpellIcons => lang.SpellIconsTooltip,
            HideHudFlags.NameOverheadGump => lang.NameOverheadGumpTooltip,
            HideHudFlags.ScriptManagerGump => lang.ScriptManagerGumpTooltip,
            HideHudFlags.PlayerChar => lang.PlayerCharTooltip,
            HideHudFlags.Mouse => lang.MouseTooltip,
            HideHudFlags.HealthBarCollector => lang.HealthBarCollectorTooltip,
            HideHudFlags.AbilityButtons => lang.AbilityButtonsTooltip,
            HideHudFlags.DebugGump => lang.DebugGumpTooltip,
            _ => null
        };
    }
}
