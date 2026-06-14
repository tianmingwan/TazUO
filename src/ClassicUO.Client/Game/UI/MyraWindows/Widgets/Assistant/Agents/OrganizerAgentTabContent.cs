#nullable enable
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ClassicUO.Configuration;
using ClassicUO.Game.GameObjects;
using ClassicUO.Game.Managers;
using ClassicUO.Utility;
using Myra.Graphics2D;
using Myra.Graphics2D.UI;

namespace ClassicUO.Game.UI.MyraWindows.Widgets.Assistant.Agents;

public static class OrganizerAgentTabContent
{
    public static Widget Build()
    {
        var lang = Language.Instance.Assistant.Agents.Organizer;
        var common = Language.Instance.UiCommons;

        OrganizerConfig? selectedConfig = null;
        var leftPanel = new VerticalStackPanel { Spacing = 4 };
        var rightPanel = new VerticalStackPanel { Spacing = 4 };

        void BuildItemsGrid(VerticalStackPanel itemsPanel)
        {
            itemsPanel.Widgets.Clear();
            if (selectedConfig == null || selectedConfig.ItemConfigs.Count == 0)
            {
                itemsPanel.Widgets.Add(new MyraLabel(lang.NoItemsConfigured, MyraLabel.TextStyle.H3));
                return;
            }

            var grid = new MyraGrid();
            grid.SetupWithHeaders(
                GridColumnInfo.Auto(lang.ColArt),
                GridColumnInfo.Auto(lang.ColHue),
                GridColumnInfo.Auto(lang.ColAmount),
                GridColumnInfo.Fill(lang.ColDestination),
                GridColumnInfo.Auto(lang.ColEnabled),
                GridColumnInfo.Auto(lang.ColActions)
            );

            int dataRow = 1;
            for (int i = selectedConfig.ItemConfigs.Count - 1; i >= 0; i--)
            {
                OrganizerItemConfig item = selectedConfig.ItemConfigs[i];

                // Art / Graphic
                Widget artWidget =
                    item.Graphic > 0
                        ? new MyraArtTexture((uint)item.Graphic)
                        {
                            Tooltip = string.Format(lang.GraphicTooltip, $"{item.Graphic:X4}"),
                            Margin = new Thickness(2, 0),
                        }
                        : new MyraLabel($"{item.Graphic:X4}", MyraLabel.TextStyle.P);
                grid.AddWidget(artWidget, dataRow, 0);

                // Hue
                var hueBox = MyraInputBox.Hue(item.Hue);
                hueBox.TextChangedByUser += (_, _) =>
                {
                    if (MyraInputBox.TryParseHue(hueBox.Text, out ushort hue))
                        item.Hue = hue;
                };
                grid.AddWidget(hueBox, dataRow, 1);

                // Amount
                var amountBox = new MyraInputBox
                {
                    Text = item.Amount.ToString(),
                    Tooltip = lang.AmountTooltip,
                    Width = 80,
                };
                amountBox.TextChangedByUser += (_, _) =>
                {
                    if (ushort.TryParse(amountBox.Text, out ushort amount))
                        item.Amount = amount;
                };
                grid.AddWidget(amountBox, dataRow, 2);

                // Destination (rebuild the cell in-place via a container panel)
                var destCell = new HorizontalStackPanel { Spacing = 4 };
                OrganizerItemConfig captured = item;

                void BuildDestCell()
                {
                    destCell.Widgets.Clear();
                    if (captured.DestContSerial != 0)
                    {
                        var label = new MyraLabel($"{captured.DestContSerial:X}", MyraLabel.TextStyle.P) { Tooltip = lang.PerItemDestination };
                        StackPanel.SetProportionType(label, ProportionType.Fill);
                        destCell.Widgets.Add(label);
                        destCell.Widgets.Add(MyraStyle.ApplyButtonDangerStyle(new MyraButton("X", () =>
                        {
                            captured.DestContSerial = 0;
                            BuildDestCell();
                        }) { Tooltip = lang.ClearAndUseConfigDestination }));
                    }
                    else
                    {
                        var label = new MyraLabel(lang.Config, MyraLabel.TextStyle.P) { Tooltip = lang.UsingConfigDestination };
                        StackPanel.SetProportionType(label, ProportionType.Fill);
                        destCell.Widgets.Add(label);
                        destCell.Widgets.Add(new MyraButton(lang.Set, () =>
                        {
                            GameActions.Print(lang.SelectDestinationContainerForItem, 82);
                            World.Instance.TargetManager.SetTargeting(destination =>
                            {
                                if (destination is Entity destEntity && SerialHelper.IsItem(destEntity))
                                {
                                    captured.DestContSerial = destEntity.Serial;
                                    GameActions.Print(string.Format(lang.PerItemDestinationSet, $"{destEntity.Serial:X}"), Constants.HUE_SUCCESS);
                                    BuildDestCell();
                                }
                                else
                                    GameActions.Print(lang.OnlyItemsCanBeSelected);
                            });
                        }) { Tooltip = lang.SetPerItemDestination });
                    }
                }

                BuildDestCell();
                grid.AddWidget(destCell, dataRow, 3);

                // Enabled
                var cb = MyraCheckButton.CreateWithCallback(item.Enabled, b => item.Enabled = b);
                cb.HorizontalAlignment = HorizontalAlignment.Center;
                grid.AddWidget(cb, dataRow, 4);

                // Delete
                grid.AddWidget(MyraStyle.ApplyButtonDangerStyle(new MyraButton(common.Delete, () =>
                {
                    selectedConfig.DeleteItemConfig(captured);
                    BuildItemsGrid(itemsPanel);
                }) { Tooltip = lang.DeleteThisItem }), dataRow, 5);

                dataRow++;
            }

            itemsPanel.Widgets.Add(grid);
        }

        void BuildConfigList()
        {
            leftPanel.Widgets.Clear();
            leftPanel.Widgets.Add(new MyraButton(lang.AddOrganizer, () =>
            {
                OrganizerConfig newConfig = OrganizerAgent.Instance.NewOrganizerConfig();
                selectedConfig = newConfig;
                BuildConfigList();
                BuildConfigDetails();
            }));
            leftPanel.Widgets.Add(new MyraLabel(lang.List, MyraLabel.TextStyle.H3));

            foreach (OrganizerConfig config in OrganizerAgent.Instance.OrganizerConfigs)
            {
                OrganizerConfig capturedConfig = config;
                int enabledItems = config.ItemConfigs.Count(ic => ic.Enabled);
                var btn = new MyraButton(config.Name, () =>
                {
                    selectedConfig = capturedConfig;
                    BuildConfigDetails();
                }) { Tooltip = string.Format(lang.EnabledItemsCount, enabledItems) };
                leftPanel.Widgets.Add(btn);
            }
        }

        void BuildConfigDetails()
        {
            rightPanel.Widgets.Clear();
            if (selectedConfig == null)
            {
                rightPanel.Widgets.Add(new MyraLabel(lang.SelectOrganizerToViewDetails, MyraLabel.TextStyle.P));
                return;
            }

            // Enabled + Name
            var topRow = new HorizontalStackPanel { Spacing = 8 };
            topRow.Widgets.Add(MyraCheckButton.CreateWithCallback(
                selectedConfig.Enabled, b => selectedConfig.Enabled = b, lang.Enabled));
            var nameBox = new MyraInputBox { Text = selectedConfig.Name, Width = 150 };
            nameBox.TextChangedByUser += (_, _) =>
            {
                if (!string.IsNullOrWhiteSpace(nameBox.Text))
                    selectedConfig.Name = nameBox.Text;
            };
            topRow.Widgets.Add(new MyraLabel(lang.NameLabel, MyraLabel.TextStyle.P));
            topRow.Widgets.Add(nameBox);
            rightPanel.Widgets.Add(topRow);

            // Action buttons
            var actionRow = new HorizontalStackPanel { Spacing = 4 };
            actionRow.Widgets.Add(new MyraButton(lang.RunOrganizer, () =>
                OrganizerAgent.Instance.RunOrganizer(selectedConfig.Name)));
            actionRow.Widgets.Add(new MyraButton(lang.Duplicate, () =>
            {
                OrganizerConfig? duped = OrganizerAgent.Instance.DupeConfig(selectedConfig);
                if (duped != null)
                {
                    selectedConfig = duped;
                    BuildConfigList();
                    BuildConfigDetails();
                }
            }));
            actionRow.Widgets.Add(new MyraButton(lang.CreateMacro, () =>
            {
                OrganizerAgent.Instance.CreateOrganizerMacroButton(selectedConfig.Name);
                GameActions.Print(string.Format(lang.CreatedOrganizerMacro, selectedConfig.Name));
            }));
            actionRow.Widgets.Add(new MyraButton(common.Import, () =>
            {
                string? json = Clipboard.GetClipboardText();
                if (json.NotNullNotEmpty() && OrganizerAgent.Instance.ImportFromJson(json))
                {
                    BuildConfigList();
                    return;
                }
                GameActions.Print(lang.ClipboardNoValidExport, Constants.HUE_ERROR);
            }) { Tooltip = lang.ImportTooltip });
            actionRow.Widgets.Add(new MyraButton(common.Export, () =>
            {
                OrganizerAgent.Instance.GetJsonExport(selectedConfig)?.CopyToClipboard();
                GameActions.Print(lang.ExportedOrganizer, Constants.HUE_SUCCESS);
            }) { Tooltip = lang.ExportTooltip });
            actionRow.Widgets.Add(MyraStyle.ApplyButtonDangerStyle(new MyraButton(common.Delete, () =>
            {
                OrganizerAgent.Instance.DeleteConfig(selectedConfig);
                List<OrganizerConfig> configs = OrganizerAgent.Instance.OrganizerConfigs;
                selectedConfig = configs.Count > 0 ? configs[0] : null;
                BuildConfigList();
                BuildConfigDetails();
            })));
            rightPanel.Widgets.Add(actionRow);

            // Container settings
            rightPanel.Widgets.Add(new MyraSpacer(5, 1));
            rightPanel.Widgets.Add(new MyraLabel(lang.ContainerSettings, MyraLabel.TextStyle.H2));
            var contRow = new HorizontalStackPanel { Spacing = 4 };
            contRow.Widgets.Add(new MyraButton(lang.SetSourceContainer, () =>
            {
                GameActions.Print(lang.SelectSourceContainer, 82);
                World.Instance.TargetManager.SetTargeting(source =>
                {
                    if (source is Entity sourceEntity && SerialHelper.IsItem(sourceEntity))
                    {
                        if (selectedConfig == null) return;
                        selectedConfig.SourceContSerial = sourceEntity.Serial;
                        GameActions.Print(string.Format(lang.SourceContainerSet, $"{sourceEntity.Serial:X4}", sourceEntity.Name), Constants.HUE_SUCCESS);
                        BuildConfigDetails();
                    }
                    else
                        GameActions.Print(lang.OnlyItemsCanBeSelected);
                });
            }));
            contRow.Widgets.Add(new MyraButton(lang.SetDestinationContainer, () =>
            {
                GameActions.Print(lang.SelectDestinationContainer, 82);
                World.Instance.TargetManager.SetTargeting(destination =>
                {
                    if (destination is Entity destEntity && SerialHelper.IsItem(destEntity))
                    {
                        if (selectedConfig == null) return;
                        selectedConfig.DestContSerial = destEntity.Serial;
                        GameActions.Print(string.Format(lang.DestinationContainerSet, $"{destEntity.Serial:X4}", destEntity.Name), Constants.HUE_SUCCESS);
                        BuildConfigDetails();
                    }
                    else
                        GameActions.Print(lang.OnlyItemsCanBeSelected);
                });
            }));
            rightPanel.Widgets.Add(contRow);

            var contInfoRow = new HorizontalStackPanel { Spacing = 12 };
            string sourceText = selectedConfig.SourceContSerial != 0
                ? string.Format(lang.SourceLabelFormat, $"{selectedConfig.SourceContSerial:X4}")
                : lang.SourceYourBackpack;
            contInfoRow.Widgets.Add(new MyraLabel(sourceText, MyraLabel.TextStyle.P));
            string destText = selectedConfig.DestContSerial != 0
                ? string.Format(lang.DestinationLabelFormat, $"{selectedConfig.DestContSerial:X4}")
                : lang.DestinationNotSet;
            contInfoRow.Widgets.Add(new MyraLabel(destText, MyraLabel.TextStyle.P));
            rightPanel.Widgets.Add(contInfoRow);

            // Items section
            rightPanel.Widgets.Add(new MyraSpacer(5, 1));
            rightPanel.Widgets.Add(new MyraLabel(lang.ItemsToOrganize, MyraLabel.TextStyle.H2));

            var itemsPanel = new VerticalStackPanel { Spacing = 2 };

            // Add item buttons
            var addEntryPanel = new VerticalStackPanel { Visible = false, Spacing = 4 };
            var newGraphicBox = new MyraInputBox { HintText = lang.GraphicHexHint, Width = 150 };
            var newHueBox = MyraInputBox.Hue(ushort.MaxValue, 80, lang.HueAnyHint);

            var addItemRow = new HorizontalStackPanel { Spacing = 4 };
            addItemRow.Widgets.Add(new MyraButton(lang.TargetItemToAdd, () =>
            {
                World.Instance.TargetManager.SetTargeting(obj =>
                {
                    if (obj is Entity objEntity && SerialHelper.IsItem(objEntity))
                    {
                        if (selectedConfig == null) return;
                        OrganizerItemConfig newItemConfig = selectedConfig.NewItemConfig();
                        newItemConfig.Graphic = objEntity.Graphic;
                        newItemConfig.Hue = objEntity.Hue;
                        GameActions.Print(string.Format(lang.AddedItemGraphic, $"{objEntity.Graphic:X}", $"{objEntity.Hue:X}"));
                        BuildItemsGrid(itemsPanel);
                    }
                    else
                        GameActions.Print(lang.OnlyItemsCanBeAdded);
                });
            }));
            addItemRow.Widgets.Add(new MyraButton(lang.AddItemManually, () => addEntryPanel.Visible = !addEntryPanel.Visible));
            rightPanel.Widgets.Add(addItemRow);

            // Manual add form
            var addFieldsRow = new HorizontalStackPanel { Spacing = 4 };
            addFieldsRow.Widgets.Add(new MyraLabel(lang.GraphicLabel, MyraLabel.TextStyle.P) { Tooltip = lang.GraphicLabelTooltip });
            addFieldsRow.Widgets.Add(newGraphicBox);
            addFieldsRow.Widgets.Add(new MyraLabel(lang.HueLabel, MyraLabel.TextStyle.P) { Tooltip = lang.HueLabelTooltip });
            addFieldsRow.Widgets.Add(newHueBox);

            var addConfirmRow = new HorizontalStackPanel { Spacing = 4 };
            addConfirmRow.Widgets.Add(new MyraButton(common.Add, () =>
            {
                if (ushort.TryParse(newGraphicBox.Text, NumberStyles.HexNumber, null, out ushort graphic))
                {
                    OrganizerItemConfig newItemConfig = selectedConfig.NewItemConfig();
                    newItemConfig.Graphic = graphic;

                    if (MyraInputBox.TryParseHue(newHueBox.Text, out ushort hue))
                        newItemConfig.Hue = hue;

                    newGraphicBox.Text = "";
                    newHueBox.Text = "";
                    addEntryPanel.Visible = false;
                    BuildItemsGrid(itemsPanel);
                }
            }));
            addConfirmRow.Widgets.Add(new MyraButton(common.Cancel, () =>
            {
                addEntryPanel.Visible = false;
                newGraphicBox.Text = "";
                newHueBox.Text = "";
            }));

            addEntryPanel.Widgets.Add(new MyraLabel(lang.ManualEntry, MyraLabel.TextStyle.H3));
            addEntryPanel.Widgets.Add(addFieldsRow);
            addEntryPanel.Widgets.Add(addConfirmRow);
            rightPanel.Widgets.Add(addEntryPanel);

            BuildItemsGrid(itemsPanel);
            rightPanel.Widgets.Add(new ScrollViewer { MaxHeight = 250, Content = itemsPanel });
        }

        BuildConfigList();
        BuildConfigDetails();

        var root = new HorizontalStackPanel { Spacing = MyraStyle.STANDARD_SPACING };
        root.Widgets.Add(new ScrollViewer { Width = 160, Content = leftPanel });
        root.Widgets.Add(rightPanel);
        return root;
    }
}
