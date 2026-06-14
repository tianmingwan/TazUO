using ClassicUO.Configuration;
using Myra.Graphics2D.UI;

namespace ClassicUO.Game.UI.MyraWindows.Widgets.Assistant.Agents;

public static class AgentTab
{
    public static Widget Build()
    {
        var tabs = new MyraTabControl();
        tabs.AddTab(Language.Instance.Assistant.SubTabAutoLoot, AutoLootAgentTabContent.Build);
        tabs.AddTab(Language.Instance.Assistant.SubTabDress, DressAgentTabContent.Build);
        tabs.AddTab(Language.Instance.Assistant.SubTabAutoBuy, AutoBuyAgentTabContent.Build);
        tabs.AddTab(Language.Instance.Assistant.SubTabAutoSell, AutoSellAgentTabContent.Build);
        tabs.AddTab(Language.Instance.Assistant.SubTabBandage, BandageAgentTabContent.Build);
        tabs.AddTab(Language.Instance.Assistant.SubTabOrganizer, OrganizerAgentTabContent.Build);
        tabs.AddTab(Language.Instance.Assistant.SubTabStatLock, AutoStatLockAgentTabContent.Build);
        tabs.SelectFirst();
        return tabs;
    }
}
