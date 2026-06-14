using ClassicUO.Configuration;
using Myra.Graphics2D.UI;

namespace ClassicUO.Game.UI.MyraWindows.Widgets.Assistant;

public static class GeneralTab
{
    public static Widget Build()
    {
        var tabs = new MyraTabControl();
        tabs.AddTab(Language.Instance.Assistant.SubTabOptions, GeneralTabContent.Build);
        tabs.AddTab(Language.Instance.Assistant.SubTabHUD, HudTabContent.Build);
        tabs.AddTab(Language.Instance.Assistant.SubTabSpellBar, SpellBarTabContent.Build);
        tabs.AddTab(Language.Instance.Assistant.SubTabTitleBar, TitleBarTabContent.Build);
        tabs.AddTab(Language.Instance.Assistant.SubTabSpellIndicators, SpellIndicatorTabContent.Build);
        tabs.AddTab(Language.Instance.Assistant.SubTabFriends, FriendsListTabContent.Build);
        tabs.AddTab(Language.Instance.Assistant.SubTabPathfinding, PathfindingTabContent.Build);
        tabs.SelectFirst();
        return tabs;
    }
}
