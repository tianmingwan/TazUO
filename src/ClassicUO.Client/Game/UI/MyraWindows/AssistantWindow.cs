using ClassicUO.Configuration;
using ClassicUO.Game.Managers;
using ClassicUO.Game.UI;
using ClassicUO.Game.UI.Controls;
using ClassicUO.Game.UI.MyraWindows.Widgets;
using ClassicUO.Game.UI.MyraWindows.Widgets.Assistant;
using ClassicUO.Game.UI.MyraWindows.Widgets.Assistant.Agents;
using ClassicUO.Game.UI.MyraWindows.Widgets.Assistant.Filters;
using ClassicUO.Game.UI.MyraWindows.Widgets.Assistant.ItemDatabase;
using ClassicUO.Game.UI.MyraWindows.Widgets.Assistant.Macros;
using ClassicUO.Game.UI.MyraWindows.Widgets.Assistant.Skills;

namespace ClassicUO.Game.UI.MyraWindows;

public class AssistantWindow : MyraControl
{
    public static void Show()
    {
        foreach (IGui g in UIManager.Gumps)
        {
            if (g is AssistantWindow w)
            {
                w.CenterInViewPort();
                w.BringOnTop();
                return;
            }
        }
        UIManager.Add(new AssistantWindow());
    }

    private SkillsTabContent _skillsTabContent;

    public AssistantWindow() : base(Language.Instance.Assistant.WindowTitle)
    {
        CanBeSaved = true;
        Build();
        CenterInViewPort();

        EventSink.SkillValueChangedEvent += EventSkillUpdated;
        EventSink.SkillBaseChangedEvent += EventSkillUpdated;
        EventSink.SkillCapChangedEvent += EventSkillUpdated;
    }

    private void EventSkillUpdated(object sender, SkillChangeArgs e) => _skillsTabContent?.UpdateSkills();

    public override void Dispose()
    {
        base.Dispose();

        MacrosTabContent.Cleanup();

        EventSink.SkillValueChangedEvent -= EventSkillUpdated;
        EventSink.SkillBaseChangedEvent -= EventSkillUpdated;
        EventSink.SkillCapChangedEvent -= EventSkillUpdated;
    }

    private void Build()
    {
        var tabs = new MyraTabControl();
        tabs.AddTab(Language.Instance.Assistant.TabGeneral, GeneralTab.Build);
        tabs.AddTab(Language.Instance.Assistant.TabAgents, AgentTab.Build);
        tabs.AddTab(Language.Instance.Assistant.TabFilters, FiltersTab.Build);
        tabs.AddTab(Language.Instance.Assistant.TabItemDatabase, ItemDatabaseTabContent.Build);
        tabs.AddTab(Language.Instance.Assistant.TabMacros, MacrosTabContent.Build);
        tabs.AddTab(Language.Instance.Assistant.TabSkills, () => _skillsTabContent = new());
        tabs.SelectFirst();
        SetRootContent(tabs);
    }
}
