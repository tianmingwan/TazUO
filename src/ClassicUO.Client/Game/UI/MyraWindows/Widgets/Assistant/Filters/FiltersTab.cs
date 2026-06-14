using ClassicUO.Configuration;
using Myra.Graphics2D.UI;

namespace ClassicUO.Game.UI.MyraWindows.Widgets.Assistant.Filters;

public static class FiltersTab
{
    public static Widget Build()
    {
        var tabs = new MyraTabControl();
        tabs.AddTab(Language.Instance.Assistant.SubTabGraphics, GraphicReplacementTabContent.Build);
        tabs.AddTab(Language.Instance.Assistant.SubTabJournalFilter, JournalFilterTabContent.Build);
        tabs.AddTab(Language.Instance.Assistant.SubTabSoundFilter, SoundFilterTabContent.Build);
        tabs.AddTab(Language.Instance.Assistant.SubTabMusicFilter, MusicFilterTabContent.Build);
        tabs.AddTab(Language.Instance.Assistant.SubTabSeasonFilter, SeasonFilterTabContent.Build);
        tabs.SelectFirst();
        return tabs;
    }
}
