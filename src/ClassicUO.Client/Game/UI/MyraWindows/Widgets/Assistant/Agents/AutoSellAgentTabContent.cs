#nullable enable
using System.Collections.Generic;
using ClassicUO.Configuration;
using ClassicUO.Game.GameObjects;
using ClassicUO.Game.Managers;
using ClassicUO.Utility;
using Myra.Graphics2D.UI;

namespace ClassicUO.Game.UI.MyraWindows.Widgets.Assistant.Agents;

public static class AutoSellAgentTabContent
{
    public static Widget Build()
    {
        Profile? profile = ProfileManager.CurrentProfile;
        var lang = Language.Instance.Assistant.Agents.AutoSell;
        var common = Language.Instance.UiCommons;

        if (profile == null)
            return new MyraLabel(lang.ProfileNotLoaded, MyraLabel.TextStyle.P);

        var root = new VerticalStackPanel { Spacing = 6 };

        root.Widgets.Add(MyraCheckButton.CreateWithCallback(
            profile.SellAgentEnabled, b => profile.SellAgentEnabled = b, lang.EnableAutoSell));

        root.Widgets.Add(new MyraLabel(lang.OptionsHeader, MyraLabel.TextStyle.H3));
        root.Widgets.Add(MyraHSlider.SliderWithLabel(
            lang.MaxTotalItems,
            out _,
            v => profile.SellAgentMaxItems = (int)v,
            0, 1000,
            profile.SellAgentMaxItems));
        root.Widgets.Add(MyraHSlider.SliderWithLabel(
            lang.MaxUniqueItems,
            out _,
            v => profile.SellAgentMaxUniques = (int)v,
            0, 100,
            profile.SellAgentMaxUniques));

        root.Widgets.Add(new MyraLabel(lang.EntriesHeader, MyraLabel.TextStyle.H3));

        var entriesPanel = new VerticalStackPanel { Spacing = 4 };

        void BuildEntriesList()
        {
            entriesPanel.Widgets.Clear();
            List<BuySellItemConfig> entries = BuySellAgent.Instance?.SellConfigs ?? new List<BuySellItemConfig>();

            if (entries.Count == 0)
            {
                entriesPanel.Widgets.Add(new MyraLabel(lang.NoEntriesConfigured, MyraLabel.TextStyle.H3));
                return;
            }

            var grid = new MyraGrid();
            grid.SetupWithHeaders(
                GridColumnInfo.Auto(lang.ColArt),
                GridColumnInfo.Fill(lang.ColGraphic),
                GridColumnInfo.Fill(lang.ColHue),
                GridColumnInfo.Fill(lang.ColMaxAmount),
                GridColumnInfo.Fill(lang.ColMinOnHand),
                GridColumnInfo.Auto(lang.ColEnabled),
                GridColumnInfo.Auto(lang.ColActions)
            );

            int dataRow = 1;
            for (int i = entries.Count - 1; i >= 0; i--)
            {
                BuySellItemConfig entry = entries[i];

                if (entry.Graphic > 0)
                    grid.AddWidget(new MyraArtTexture((uint)entry.Graphic), dataRow, 0);

                var graphicBox = new MyraInputBox { Text = entry.Graphic.ToString() };
                graphicBox.TextChangedByUser += (_, _) =>
                {
                    if (StringHelper.TryParseInt(graphicBox.Text, out int g) && g is > 0 and <= ushort.MaxValue)
                        entry.Graphic = (ushort)g;
                };
                grid.AddWidget(graphicBox, dataRow, 1);

                var hueBox = MyraInputBox.Hue(entry.Hue);
                hueBox.Width = null;
                hueBox.TextChangedByUser += (_, _) =>
                {
                    if (MyraInputBox.TryParseHue(hueBox.Text, out ushort hue))
                        entry.Hue = hue;
                };
                grid.AddWidget(hueBox, dataRow, 2);

                var maxAmountBox = new MyraInputBox
                {
                    Text = entry.MaxAmount == ushort.MaxValue ? "0" : entry.MaxAmount.ToString(),
                    Tooltip = lang.SetToZeroUnlimited,
                };
                maxAmountBox.TextChangedByUser += (_, _) =>
                {
                    if (ushort.TryParse(maxAmountBox.Text, out ushort ma))
                        entry.MaxAmount = ma == 0 ? ushort.MaxValue : ma;
                };
                grid.AddWidget(maxAmountBox, dataRow, 3);

                var restockBox = new MyraInputBox
                {
                    Text = entry.RestockUpTo.ToString(),
                    Tooltip = lang.MinOnHandTooltip,
                };
                restockBox.TextChangedByUser += (_, _) =>
                {
                    if (ushort.TryParse(restockBox.Text, out ushort r)) entry.RestockUpTo = r;
                };
                grid.AddWidget(restockBox, dataRow, 4);

                var cb = MyraCheckButton.CreateWithCallback(entry.Enabled, b => entry.Enabled = b);
                cb.HorizontalAlignment = HorizontalAlignment.Center;
                grid.AddWidget(cb, dataRow, 5);

                grid.AddWidget(MyraStyle.ApplyButtonDangerStyle(new MyraButton(common.Delete, () =>
                {
                    BuySellAgent.Instance?.DeleteConfig(entry);
                    BuildEntriesList();
                })), dataRow, 6);

                dataRow++;
            }

            entriesPanel.Widgets.Add(grid);
        }

        BuildEntriesList();

        // Inline add entry panel
        var addEntryPanel = new VerticalStackPanel { Visible = false, Spacing = 4 };
        var newGraphicBox = new MyraInputBox { HintText = lang.GraphicIdHint, Width = 80 };
        var newHueBox = MyraInputBox.Hue(ushort.MaxValue, 80, lang.HueAnyHint);
        var newMaxAmountBox = new MyraInputBox { HintText = lang.MaxAmountHint, Width = 130 };
        var newRestockBox = new MyraInputBox { HintText = lang.MinOnHandHint, Width = 130 };

        var addFieldsRow1 = new HorizontalStackPanel { Spacing = 4 };
        addFieldsRow1.Widgets.Add(new MyraLabel(lang.GraphicLabel, MyraLabel.TextStyle.P));
        addFieldsRow1.Widgets.Add(newGraphicBox);
        addFieldsRow1.Widgets.Add(new MyraLabel(lang.HueLabel, MyraLabel.TextStyle.P));
        addFieldsRow1.Widgets.Add(newHueBox);

        var addFieldsRow2 = new HorizontalStackPanel { Spacing = 4 };
        addFieldsRow2.Widgets.Add(new MyraLabel(lang.MaxAmountLabel, MyraLabel.TextStyle.P));
        addFieldsRow2.Widgets.Add(newMaxAmountBox);
        addFieldsRow2.Widgets.Add(new MyraLabel(lang.MinOnHandLabel, MyraLabel.TextStyle.P));
        addFieldsRow2.Widgets.Add(newRestockBox);

        void ClearAddFields()
        {
            newGraphicBox.Text = "";
            newHueBox.Text = "";
            newMaxAmountBox.Text = "";
            newRestockBox.Text = "";
        }

        var addConfirmRow = new HorizontalStackPanel { Spacing = 4 };
        addConfirmRow.Widgets.Add(new MyraButton(common.Add, () =>
        {
            if (StringHelper.TryParseInt(newGraphicBox.Text, out int graphic))
            {
                BuySellItemConfig newConfig = BuySellAgent.Instance.NewSellConfig();
                newConfig.Graphic = (ushort)graphic;

                if (MyraInputBox.TryParseHue(newHueBox.Text, out ushort hue))
                    newConfig.Hue = hue;
                else
                    newConfig.Hue = ushort.MaxValue;

                if (!string.IsNullOrEmpty(newMaxAmountBox.Text) && ushort.TryParse(newMaxAmountBox.Text, out ushort maxAmount))
                    newConfig.MaxAmount = maxAmount == 0 ? ushort.MaxValue : maxAmount;

                if (!string.IsNullOrEmpty(newRestockBox.Text) && ushort.TryParse(newRestockBox.Text, out ushort restock))
                    newConfig.RestockUpTo = restock;

                ClearAddFields();
                addEntryPanel.Visible = false;
                BuildEntriesList();
            }
        }));
        addConfirmRow.Widgets.Add(new MyraButton(common.Cancel, () =>
        {
            addEntryPanel.Visible = false;
            ClearAddFields();
        }));

        addEntryPanel.Widgets.Add(new MyraLabel(lang.AddNewEntry, MyraLabel.TextStyle.H3));
        addEntryPanel.Widgets.Add(addFieldsRow1);
        addEntryPanel.Widgets.Add(addFieldsRow2);
        addEntryPanel.Widgets.Add(addConfirmRow);

        // Action buttons
        var actionRow = new HorizontalStackPanel { Spacing = 6 };
        actionRow.Widgets.Add(new MyraButton(lang.AddManualEntry, () => addEntryPanel.Visible = !addEntryPanel.Visible));
        actionRow.Widgets.Add(new MyraButton(lang.AddFromTarget, () =>
        {
            GameActions.Print(Client.Game.UO.World, lang.TargetItemToAdd);
            World.Instance.TargetManager.SetTargeting(targeted =>
            {
                if (targeted is Entity entity && SerialHelper.IsItem(entity))
                {
                    if (BuySellAgent.Instance.TryGetSellConfig(entity.Graphic, entity.Hue, out _))
                        return;
                    BuySellItemConfig newConfig = BuySellAgent.Instance.NewSellConfig();
                    newConfig.Graphic = entity.Graphic;
                    newConfig.Hue = entity.Hue;
                    BuildEntriesList();
                }
            });
        }) { Tooltip = lang.AddFromTargetTooltip });
        actionRow.Widgets.Add(new MyraButton(lang.AddFromContainer, () =>
        {
            GameActions.Print(Client.Game.UO.World, lang.TargetContainerToAddAllItems);
            World.Instance.TargetManager.SetTargeting(targeted =>
            {
                if (targeted is Item container)
                {
                    int added = 0;
                    for (LinkedObject i = container.Items; i != null; i = i.Next)
                    {
                        if (i is Item item)
                        {
                            if (BuySellAgent.Instance.TryGetSellConfig(item.Graphic, item.Hue, out _))
                                continue;
                            BuySellItemConfig newConfig = BuySellAgent.Instance.NewSellConfig();
                            newConfig.Graphic = item.Graphic;
                            newConfig.Hue = item.Hue;
                            added++;
                        }
                    }
                    GameActions.Print(Client.Game.UO.World, string.Format(lang.AddedItemsFromContainer, added));
                    BuildEntriesList();
                }
            });
        }) { Tooltip = lang.AddFromContainerTooltip });
        actionRow.Widgets.Add(MyraStyle.ApplyButtonDangerStyle(new MyraButton(lang.ClearAll, () =>
        {
            BuySellAgent.Instance.SellConfigs?.Clear();
            BuildEntriesList();
        }) { Tooltip = lang.ClearAllTooltip }));
        actionRow.Widgets.Add(new MyraButton(common.Import, () =>
        {
            string? json = Clipboard.GetClipboardText();
            if (json.NotNullNotEmpty() && BuySellAgent.ImportFromJson(json, AgentType.Sell))
            {
                GameActions.Print(lang.ImportedSellList, Constants.HUE_SUCCESS);
                BuildEntriesList();
                return;
            }
            GameActions.Print(lang.ClipboardNoValidExport, Constants.HUE_ERROR);
        }) { Tooltip = lang.ImportTooltip });
        actionRow.Widgets.Add(new MyraButton(common.Export, () =>
        {
            BuySellAgent.GetJsonExport(AgentType.Sell)?.CopyToClipboard();
            GameActions.Print(lang.ExportedSellList, Constants.HUE_SUCCESS);
        }) { Tooltip = lang.ExportTooltip });

        root.Widgets.Add(actionRow);
        root.Widgets.Add(addEntryPanel);
        root.Widgets.Add(new ScrollViewer { MaxHeight = 300, Content = entriesPanel });

        return root;
    }
}
