#nullable enable
using System;
using System.Collections.Generic;
using ClassicUO.Configuration;
using ClassicUO.Game.Data;
using ClassicUO.Game.Managers;
using ClassicUO.Utility;
using Myra.Graphics2D.UI;

namespace ClassicUO.Game.UI.MyraWindows.Widgets.Assistant.ItemDatabase;

public static class ItemDatabaseTabContent
{
    public static Widget Build()
    {
        var lang = Language.Instance.Assistant.ItemDatabase;
        var common = Language.Instance.UiCommons;

        Profile? profile = ProfileManager.CurrentProfile;
        if (profile == null)
            return new MyraLabel(lang.ProfileNotLoaded, MyraLabel.TextStyle.P);

        var root = new VerticalStackPanel { Spacing = 6 };

        root.Widgets.Add(MyraCheckButton.CreateWithCallback(
            profile.ItemDatabaseEnabled,
            b => profile.ItemDatabaseEnabled = b,
            lang.EnableItemDatabase));

        List<ItemInfo> searchResults = new();
        bool searchInProgress = false;

        string searchName = "";
        string searchProps = "";
        uint searchGraphic = 0;
        int searchHue = -1;
        int searchLayer = -1;
        int searchContainer = 0;
        bool onGroundOnly = false;
        bool inContainersOnly = false;
        bool currentCharOnly = false;
        int maxResults = 100;

        TextBox nameBox = null!;
        TextBox propsBox = null!;
        TextBox graphicBox = null!;
        TextBox hueBox = null!;
        TextBox layerBox = null!;
        TextBox containerBox = null!;
        MyraHSlider? maxResultsSlider = null;

        var resultsPanel = new VerticalStackPanel { Spacing = 2 };
        var statusLabel = new MyraLabel(lang.ReadyToSearch, MyraLabel.TextStyle.P);

        void BuildResultsGrid()
        {
            resultsPanel.Widgets.Clear();
            if (searchResults.Count == 0)
            {
                resultsPanel.Widgets.Add(new MyraLabel(lang.NoResultsToDisplay, MyraLabel.TextStyle.P));
                return;
            }

            var grid = new MyraGrid();
            grid.SetupWithHeaders(
                GridColumnInfo.Auto(lang.ColArt),
                GridColumnInfo.Fill(lang.ColName),
                GridColumnInfo.Auto(lang.ColHue),
                GridColumnInfo.Auto(lang.ColLayer),
                GridColumnInfo.Auto(lang.LocationLabel),
                GridColumnInfo.Auto(lang.ColContainer),
                GridColumnInfo.Auto(lang.ColCharacter),
                GridColumnInfo.Auto(lang.ColUpdated),
                GridColumnInfo.Auto(lang.ColActions)
            );

            int dataRow = 1;
            foreach (ItemInfo item in searchResults)
            {
                if (item.Graphic > 0)
                    grid.AddWidget(
                        new MyraArtTexture(item.Graphic)
                            { Tooltip = $"Graphic: {item.Graphic} (0x{item.Graphic:X})" },
                        dataRow, 0);

                var nameLabel = new MyraLabel(item.Name, MyraLabel.TextStyle.P);
                if (!string.IsNullOrEmpty(item.Properties))
                    nameLabel.Tooltip = item.Properties.Replace("|", "\n");
                grid.AddWidget(nameLabel, dataRow, 1);

                grid.AddWidget(new MyraLabel($"{item.Hue}", MyraLabel.TextStyle.P, MyraLabel.AlignMode.Right), dataRow, 2);

                grid.AddWidget(
                    new MyraLabel($"{item.Layer}", MyraLabel.TextStyle.P, MyraLabel.AlignMode.Right)
                        { Tooltip = $"Layer value: {(int)item.Layer}" },
                    dataRow, 3);

                string locationStr = item.OnGround ? $"{item.X}, {item.Y}" : lang.ContainerLabel;
                grid.AddWidget(new MyraLabel(locationStr, MyraLabel.TextStyle.P), dataRow, 4);

                string containerStr = (item.Container != 0 && item.Container != 0xFFFFFFFF)
                    ? $"0x{item.Container:X}"
                    : lang.GroundLabel;
                grid.AddWidget(new MyraLabel(containerStr, MyraLabel.TextStyle.P), dataRow, 5);

                grid.AddWidget(new MyraLabel(item.CharacterName, MyraLabel.TextStyle.P), dataRow, 6);

                TimeSpan timeAgo = DateTime.Now - item.UpdatedTime;
                string timeStr = timeAgo.TotalDays >= 1   ? string.Format(common.DaysAgo, timeAgo.Days)
                    : timeAgo.TotalHours >= 1             ? string.Format(common.HoursAgo, timeAgo.Hours)
                    : timeAgo.TotalMinutes >= 1           ? string.Format(common.MinutesAgo, (int)timeAgo.TotalMinutes)
                    : common.JustNow;
                grid.AddWidget(new MyraLabel(timeStr, MyraLabel.TextStyle.P), dataRow, 7);

                ItemInfo captured = item;
                grid.AddWidget(
                    new MyraButton(lang.Details, () => new ItemDetailMyraWindow(captured))
                        { Tooltip = lang.ViewDetailedItemInfo },
                    dataRow, 8);

                dataRow++;
            }

            resultsPanel.Widgets.Add(grid);
        }

        void PerformSearch()
        {
            if (searchInProgress) return;
            if (!profile.ItemDatabaseEnabled)
            {
                statusLabel.Text = lang.ItemDatabaseIsDisabled;
                return;
            }

            searchInProgress = true;
            statusLabel.Text = common.Searching;
            searchResults.Clear();
            resultsPanel.Widgets.Clear();

            ushort? graphic   = searchGraphic > 0   ? (ushort)searchGraphic   : null;
            ushort? hue       = searchHue >= 0      ? (ushort)searchHue       : null;
            Layer?  layer     = searchLayer >= 0    ? (Layer)searchLayer      : null;
            uint?   container = searchContainer > 0 ? (uint)searchContainer   : null;
            string? name      = string.IsNullOrWhiteSpace(searchName)  ? null : searchName.Trim();
            string? props     = string.IsNullOrWhiteSpace(searchProps) ? null : searchProps.Trim();
            uint?   character = null;
            bool?   ground    = null;

            if (currentCharOnly && Client.Game.UO?.World?.Player != null)
                character = Client.Game.UO.World.Player.Serial;

            if (onGroundOnly && !inContainersOnly)       ground = true;
            else if (inContainersOnly && !onGroundOnly)  ground = false;

            ItemDatabaseManager.Instance.SearchItems(
                results =>
                {
                    MainThreadQueue.EnqueueAction(() =>
                    {
                        searchResults   = results ?? new List<ItemInfo>();
                        searchInProgress = false;
                        BuildResultsGrid();
                        statusLabel.Text = searchResults.Count == 0        ? lang.NoItemsFound
                            : searchResults.Count >= maxResults             ? string.Format(lang.FoundItemsMaxLimitReached, searchResults.Count)
                            : string.Format(lang.FoundItems, searchResults.Count);
                    });
                },
                graphic:    graphic,
                hue:        hue,
                name:       name,
                properties: props,
                container:  container,
                layer:      layer,
                character:  character,
                onGround:   ground,
                limit:      maxResults
            );
        }

        void ClearSearch()
        {
            searchName    = "";  nameBox.Text    = "";
            searchProps   = "";  propsBox.Text   = "";
            searchGraphic = 0;   graphicBox.Text = "0";
            searchHue     = -1;  hueBox.Text     = "-1";
            searchLayer   = -1;  layerBox.Text   = "-1";
            searchContainer = 0; containerBox.Text = "0";
            onGroundOnly       = false;
            inContainersOnly   = false;
            currentCharOnly    = false;
            maxResults         = 100;
            if (maxResultsSlider != null) maxResultsSlider.Value = 100;
            statusLabel.Text = lang.SearchCleared;
        }

        root.Widgets.Add(new MyraLabel(lang.SearchOptions, MyraLabel.TextStyle.H3));

        nameBox = new MyraInputBox { HintText = lang.ItemNameHint, Width = 280 };
        nameBox.TextChangedByUser += (_, _) => searchName = nameBox.Text ?? "";

        propsBox = new MyraInputBox { HintText = lang.PropertyTextHint, Width = 280 };
        propsBox.TextChangedByUser += (_, _) => searchProps = propsBox.Text ?? "";

        graphicBox = new MyraInputBox { Text = "0", Width = 100, Tooltip = lang.GraphicIdTooltip };
        graphicBox.TextChangedByUser += (_, _) =>
        {
            if (StringHelper.TryParseUint(graphicBox.Text ?? "", out uint g)) searchGraphic = g;
        };

        hueBox = MyraInputBox.Hue(ushort.MaxValue, 80, lang.HueSearchTooltip);
        hueBox.TextChangedByUser += (_, _) =>
        {
            if (MyraInputBox.TryParseHue(hueBox.Text, out ushort h))
                searchHue = h;
            else if (hueBox.Text == "-1")
                searchHue = -1;
        };

        layerBox = new MyraInputBox { Text = "-1", Width = 80, Tooltip = lang.LayerSearchTooltip };
        layerBox.TextChangedByUser += (_, _) =>
        {
            if (int.TryParse(layerBox.Text, out int l)) searchLayer = l;
        };

        var nameRow = new HorizontalStackPanel { Spacing = 4 };
        nameRow.Widgets.Add(new MyraLabel(lang.NameLabel, MyraLabel.TextStyle.P));
        nameRow.Widgets.Add(nameBox);
        root.Widgets.Add(nameRow);

        var propsRow = new HorizontalStackPanel { Spacing = 4 };
        propsRow.Widgets.Add(new MyraLabel(lang.PropertiesLabel, MyraLabel.TextStyle.P));
        propsRow.Widgets.Add(propsBox);
        root.Widgets.Add(propsRow);

        var graphicHueRow = new HorizontalStackPanel { Spacing = 8 };
        graphicHueRow.Widgets.Add(new MyraLabel(lang.GraphicIdLabel, MyraLabel.TextStyle.P));
        graphicHueRow.Widgets.Add(graphicBox);
        graphicHueRow.Widgets.Add(new MyraLabel(lang.HueLabel, MyraLabel.TextStyle.P));
        graphicHueRow.Widgets.Add(hueBox);
        root.Widgets.Add(graphicHueRow);

        var layerRow = new HorizontalStackPanel { Spacing = 4 };
        layerRow.Widgets.Add(new MyraLabel(lang.LayerLabel, MyraLabel.TextStyle.P));
        layerRow.Widgets.Add(layerBox);
        root.Widgets.Add(layerRow);

        var advancedPanel = new VerticalStackPanel { Visible = false, Spacing = 4 };

        containerBox = new MyraInputBox { Text = "0", Width = 120, Tooltip = Language.Instance.Assistant.ItemDatabase.SearchContainerTooltip };
        containerBox.TextChangedByUser += (_, _) =>
        {
            if (StringHelper.TryParseInt(containerBox.Text ?? "", out int c)) searchContainer = c;
        };

        var contRow = new HorizontalStackPanel { Spacing = 4 };
        contRow.Widgets.Add(new MyraLabel(lang.ContainerSerialLabel, MyraLabel.TextStyle.P));
        contRow.Widgets.Add(containerBox);
        advancedPanel.Widgets.Add(contRow);

        var locationCheckRow = new HorizontalStackPanel { Spacing = 12 };
        locationCheckRow.Widgets.Add(
            MyraCheckButton.CreateWithCallback(false, b => onGroundOnly = b, lang.OnGroundOnly));
        locationCheckRow.Widgets.Add(
            MyraCheckButton.CreateWithCallback(false, b => inContainersOnly = b, lang.InContainersOnly));
        locationCheckRow.Widgets.Add(
            MyraCheckButton.CreateWithCallback(false, b => currentCharOnly = b, lang.CurrentCharacterOnly));
        advancedPanel.Widgets.Add(locationCheckRow);

        HorizontalStackPanel sliderWidget = MyraHSlider.SliderWithLabel(
            lang.MaxResults,
            out MyraHSlider ms,
            v => maxResults = (int)v,
            10, 1000, 100);
        maxResultsSlider = ms;
        advancedPanel.Widgets.Add(sliderWidget);

        root.Widgets.Add(MyraCheckButton.CreateWithCallback(false, b =>
        {
            advancedPanel.Visible = b;
            if (!b)
            {
                searchContainer  = 0; containerBox.Text = "0";
                onGroundOnly     = false;
                inContainersOnly = false;
            }
        }, lang.AdvancedSearch));
        root.Widgets.Add(advancedPanel);

        var actionRow = new HorizontalStackPanel { Spacing = 4 };
        actionRow.Widgets.Add(new MyraButton(lang.Search,        () => PerformSearch()));
        actionRow.Widgets.Add(new MyraButton(lang.ClearFields,  () => ClearSearch()));
        actionRow.Widgets.Add(new MyraButton(lang.ClearResults, () =>
        {
            searchResults.Clear();
            BuildResultsGrid();
            statusLabel.Text = lang.ResultsCleared;
        }));
        root.Widgets.Add(actionRow);

        root.Widgets.Add(new MyraLabel(lang.DatabaseMaintenance, MyraLabel.TextStyle.H3));

        int[] clearDays = { 120 };
        bool[] clearInProgress = { false };
        var clearDaysBox = new MyraInputBox { Text = "120", Width = 60, Tooltip = lang.ClearOldEntriesTooltip };
        clearDaysBox.TextChangedByUser += (_, _) =>
        {
            if (int.TryParse(clearDaysBox.Text, out int d) && d >= 1) clearDays[0] = d;
        };

        var clearStatusLabel = new MyraLabel("", MyraLabel.TextStyle.P) { Visible = false };

        async void DoClear()
        {
            if (clearInProgress[0]) return;
            clearInProgress[0] = true;
            clearStatusLabel.Text    = string.Format(lang.ClearingEntriesOlderThan, clearDays[0]);
            clearStatusLabel.Visible = true;
            try
            {
                await ItemDatabaseManager.Instance.ClearOldDataAsync(TimeSpan.FromDays(clearDays[0]));
                clearStatusLabel.Text = string.Format(lang.ClearedEntriesOlderThan, clearDays[0]);
            }
            catch (Exception ex)
            {
                clearStatusLabel.Text = string.Format(lang.ErrorMessage, ex.Message);
            }
            finally
            {
                clearInProgress[0] = false;
            }
        }

        var maintenanceRow = new HorizontalStackPanel { Spacing = 4 };
        maintenanceRow.Widgets.Add(new MyraLabel(lang.ClearEntriesOlderThan, MyraLabel.TextStyle.P));
        maintenanceRow.Widgets.Add(clearDaysBox);
        maintenanceRow.Widgets.Add(new MyraLabel(lang.Days, MyraLabel.TextStyle.P));
        maintenanceRow.Widgets.Add(new MyraButton(lang.ClearOldEntries, DoClear));
        root.Widgets.Add(maintenanceRow);
        root.Widgets.Add(clearStatusLabel);

        root.Widgets.Add(new MyraLabel(lang.Status, MyraLabel.TextStyle.H3));
        root.Widgets.Add(statusLabel);
        root.Widgets.Add(new MyraLabel(lang.Results, MyraLabel.TextStyle.H3));
        BuildResultsGrid();
        root.Widgets.Add(new ScrollViewer { MaxHeight = 300, Content = resultsPanel });

        return root;
    }
}
