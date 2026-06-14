#nullable enable
using System.Collections.Generic;
using ClassicUO.Configuration;
using ClassicUO.Game.Data;
using ClassicUO.Game.GameObjects;
using ClassicUO.Game.Managers;
using Myra.Graphics2D.UI;

namespace ClassicUO.Game.UI.MyraWindows.Widgets.Assistant.Agents;

public static class DressAgentTabContent
{
    public static Widget Build()
    {
        var lang = Language.Instance.Assistant.Agents.Dress;
        var common = Language.Instance.UiCommons;

        if (DressAgentManager.Instance == null)
            return new MyraLabel(lang.DressAgentNotLoaded, MyraLabel.TextStyle.P);

        DressConfig? selectedConfig = null;
        var leftPanel = new VerticalStackPanel { Spacing = 4 };
        var rightPanel = new VerticalStackPanel { Spacing = 4 };

        void BuildItemsGrid(VerticalStackPanel itemsPanel)
        {
            itemsPanel.Widgets.Clear();
            if (selectedConfig == null || selectedConfig.Items.Count == 0)
            {
                itemsPanel.Widgets.Add(new MyraLabel(lang.NoItemsConfigured, MyraLabel.TextStyle.P));
                return;
            }

            var grid = new MyraGrid();
            grid.SetupWithHeaders(
                GridColumnInfo.Auto(lang.ColSerial),
                GridColumnInfo.Fill(lang.ColName),
                GridColumnInfo.Auto(lang.ColLayer),
                GridColumnInfo.Auto(lang.ColActions)
            );

            int dataRow = 1;
            for (int i = selectedConfig.Items.Count - 1; i >= 0; i--)
            {
                DressItem item = selectedConfig.Items[i];
                grid.AddWidget(new MyraLabel($"{item.Serial:X}", MyraLabel.TextStyle.P, MyraLabel.AlignMode.Right), dataRow, 0);
                grid.AddWidget(new MyraLabel(item.Name, MyraLabel.TextStyle.P), dataRow, 1);
                grid.AddWidget(new MyraLabel(((Layer)item.Layer).ToString(), MyraLabel.TextStyle.P), dataRow, 2);
                DressItem captured = item;
                grid.AddWidget(MyraStyle.ApplyButtonDangerStyle(new MyraButton(common.Delete, () =>
                {
                    DressAgentManager.Instance.RemoveItemFromConfig(selectedConfig, captured.Serial);
                    BuildItemsGrid(itemsPanel);
                }) { Tooltip = lang.RemoveThisItem }), dataRow, 3);
                dataRow++;
            }

            itemsPanel.Widgets.Add(grid);
        }

        void BuildConfigList()
        {
            leftPanel.Widgets.Clear();
            leftPanel.Widgets.Add(new MyraLabel(lang.DressConfigurations, MyraLabel.TextStyle.H3));
            leftPanel.Widgets.Add(new MyraButton(lang.AddConfiguration, () =>
            {
                DressConfig newConfig = DressAgentManager.Instance.CreateNewConfig(
                    string.Format(lang.ConfigNameFormat, DressAgentManager.Instance.CurrentPlayerConfigs.Count + 1));
                selectedConfig = newConfig;
                BuildConfigList();
                BuildConfigDetails();
            }));

            foreach (DressConfig config in DressAgentManager.Instance.CurrentPlayerConfigs)
            {
                DressConfig captured = config;
                var btn = new MyraButton(string.Format(lang.ConfigItemsFormat, config.Name, config.Items.Count), () =>
                {
                    selectedConfig = captured;
                    BuildConfigDetails();
                });
                if (!string.IsNullOrEmpty(config.CharacterName))
                    btn.Tooltip = string.Format(lang.CharacterTooltipFormat, config.CharacterName);
                leftPanel.Widgets.Add(btn);
            }
        }

        void BuildConfigDetails()
        {
            rightPanel.Widgets.Clear();
            if (selectedConfig == null)
            {
                rightPanel.Widgets.Add(new MyraLabel(lang.SelectConfigToViewDetails, MyraLabel.TextStyle.P));
                return;
            }

            // Name
            var nameBox = new MyraInputBox { Text = selectedConfig.Name, Width = 200 };
            nameBox.TextChangedByUser += (_, _) =>
            {
                if (!string.IsNullOrWhiteSpace(nameBox.Text))
                {
                    selectedConfig.Name = nameBox.Text.Trim();
                    DressAgentManager.Instance.Save();
                }
            };
            var nameRow = new HorizontalStackPanel { Spacing = 4 };
            nameRow.Widgets.Add(new MyraLabel(lang.NameLabel, MyraLabel.TextStyle.P));
            nameRow.Widgets.Add(nameBox);
            rightPanel.Widgets.Add(nameRow);

            // Action buttons
            var actionRow = new HorizontalStackPanel { Spacing = 4 };
            actionRow.Widgets.Add(new MyraButton(lang.Dress, () =>
            {
                DressAgentManager.Instance.DressFromConfig(selectedConfig);
                GameActions.Print(string.Format(lang.DressingFromConfig, selectedConfig.Name));
            }));
            actionRow.Widgets.Add(new MyraButton(lang.Undress, () =>
            {
                DressAgentManager.Instance.UndressFromConfig(selectedConfig);
                GameActions.Print(string.Format(lang.UndressingFromConfig, selectedConfig.Name));
            }));
            actionRow.Widgets.Add(new MyraButton(lang.CreateDressMacro, () =>
            {
                DressAgentManager.Instance.CreateDressMacro(selectedConfig.Name);
                GameActions.Print(string.Format(lang.CreatedDressMacro, selectedConfig.Name));
            }));
            actionRow.Widgets.Add(new MyraButton(lang.CreateUndressMacro, () =>
            {
                DressAgentManager.Instance.CreateUndressMacro(selectedConfig.Name);
                GameActions.Print(string.Format(lang.CreatedUndressMacro, selectedConfig.Name));
            }));
            actionRow.Widgets.Add(MyraStyle.ApplyButtonDangerStyle(new MyraButton(common.Delete, () =>
            {
                DressAgentManager.Instance.DeleteConfig(selectedConfig);
                List<DressConfig> configs = DressAgentManager.Instance.CurrentPlayerConfigs;
                selectedConfig = configs.Count > 0 ? configs[0] : null;
                BuildConfigList();
                BuildConfigDetails();
            })));
            rightPanel.Widgets.Add(actionRow);

            // KR Equip Packet
            rightPanel.Widgets.Add(new MyraSpacer(15, 1));
            rightPanel.Widgets.Add(MyraCheckButton.CreateWithCallback(
                selectedConfig.UseKREquipPacket,
                b => { selectedConfig.UseKREquipPacket = b; DressAgentManager.Instance.Save(); },
                lang.UseKREquipPacket,
                lang.UseKREquipPacketTooltip));

            // Undress bag
            rightPanel.Widgets.Add(new MyraSpacer(15, 1));
            rightPanel.Widgets.Add(new MyraLabel(lang.UndressBagSettings, MyraLabel.TextStyle.H3));
            var undressBagRow = new HorizontalStackPanel { Spacing = 4 };
            undressBagRow.Widgets.Add(new MyraButton(lang.SetUndressBag, () =>
            {
                GameActions.Print(lang.SelectContainerForUndressedItems, 82);
                World.Instance.TargetManager.SetTargeting(target =>
                {
                    if (target is Entity entity && SerialHelper.IsItem(entity))
                    {
                        if (selectedConfig == null) return;
                        DressAgentManager.Instance.SetUndressBag(selectedConfig, entity.Serial);
                        GameActions.Print(string.Format(lang.UndressBagSet, $"{entity.Serial:X}"), Constants.HUE_SUCCESS);
                        BuildConfigDetails();
                    }
                    else
                        GameActions.Print(lang.OnlyItemsCanBeSelected);
                });
            }));
            if (selectedConfig.UndressBagSerial != 0)
            {
                undressBagRow.Widgets.Add(new MyraLabel(string.Format(lang.CurrentSerialFormat, $"{selectedConfig.UndressBagSerial:X}"), MyraLabel.TextStyle.P));
                undressBagRow.Widgets.Add(MyraStyle.ApplyButtonDangerStyle(new MyraButton(common.Clear, () =>
                {
                    DressAgentManager.Instance.SetUndressBag(selectedConfig, 0);
                    BuildConfigDetails();
                })));
            }
            else
                undressBagRow.Widgets.Add(new MyraLabel(lang.DefaultYourBackpack, MyraLabel.TextStyle.P));
            rightPanel.Widgets.Add(undressBagRow);

            // Items section
            rightPanel.Widgets.Add(new MyraSpacer(15, 1));
            rightPanel.Widgets.Add(new MyraLabel(lang.ItemsToDressUndress, MyraLabel.TextStyle.H3));
            var itemsPanel = new VerticalStackPanel { Spacing = 2 };
            var itemActionRow = new HorizontalStackPanel { Spacing = 4 };
            itemActionRow.Widgets.Add(new MyraButton(lang.AddCurrentlyEquipped, () =>
            {
                DressAgentManager.Instance.AddCurrentlyEquippedItems(selectedConfig);
                GameActions.Print(lang.AddedCurrentlyEquippedItems);
                BuildItemsGrid(itemsPanel);
            }));
            itemActionRow.Widgets.Add(new MyraButton(lang.TargetItemToAdd, () =>
            {
                GameActions.Print(lang.TargetItemToAddPrint, 82);
                World.Instance.TargetManager.SetTargeting(obj =>
                {
                    if (obj is Entity entity && SerialHelper.IsItem(entity))
                    {
                        if (selectedConfig == null) return;
                        DressAgentManager.Instance.AddItemToConfig(selectedConfig, entity.Serial, entity.Name);
                        GameActions.Print(string.Format(lang.AddedItem, entity.Name));
                        BuildItemsGrid(itemsPanel);
                    }
                    else
                        GameActions.Print(lang.OnlyItemsCanBeAdded);
                });
            }));
            itemActionRow.Widgets.Add(MyraStyle.ApplyButtonDangerStyle(new MyraButton(lang.ClearAllItems, () =>
            {
                DressAgentManager.Instance.ClearConfig(selectedConfig);
                GameActions.Print(lang.ClearedAllItems);
                BuildItemsGrid(itemsPanel);
            })));
            rightPanel.Widgets.Add(itemActionRow);
            BuildItemsGrid(itemsPanel);
            rightPanel.Widgets.Add(new ScrollViewer { MaxHeight = 250, Content = itemsPanel });
        }

        BuildConfigList();
        BuildConfigDetails();

        var root = new HorizontalStackPanel { Spacing = 8 };
        root.Widgets.Add(new ScrollViewer { Width = 200, Content = leftPanel });
        root.Widgets.Add(rightPanel);
        return root;
    }
}
