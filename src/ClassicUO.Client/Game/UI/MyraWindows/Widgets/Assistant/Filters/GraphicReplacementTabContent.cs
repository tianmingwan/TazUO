#nullable enable
using System.Collections.Generic;
using System.Linq;
using ClassicUO.Configuration;
using ClassicUO.Game.GameObjects;
using ClassicUO.Game.Managers;
using ClassicUO.Utility;
using Myra.Graphics2D.UI;

namespace ClassicUO.Game.UI.MyraWindows.Widgets.Assistant.Filters;

public static class GraphicReplacementTabContent
{
    private static readonly string[] TypeNames = { "Mobile", "Land", "Static" };
    private static readonly byte[] TypeValues = { 1, 2, 3 };

    private static string GetTypeName(byte t) => t switch { 1 => "Mobile", 2 => "Land", _ => "Static" };

    public static Widget Build()
    {
        var lang = Language.Instance.Assistant.GraphicReplacement;
        var ui = Language.Instance.UiCommons;
        var root = new VerticalStackPanel { Spacing = 6 };

        root.Widgets.Add(new MyraLabel(lang.HeaderDescription, MyraLabel.TextStyle.H3));

        var filtersPanel = new VerticalStackPanel { Spacing = 2 };

        void BuildFilterList()
        {
            filtersPanel.Widgets.Clear();
            Dictionary<(ushort, byte), GraphicChangeFilter> filters = GraphicsReplacement.GraphicFilters;

            if (filters.Count == 0)
            {
                filtersPanel.Widgets.Add(new MyraLabel(lang.NoReplacements, MyraLabel.TextStyle.H3));
                return;
            }

            var grid = new MyraGrid();
            grid.SetupWithHeaders(
                GridColumnInfo.Auto(lang.ColOriginal),
                GridColumnInfo.Auto(lang.ColType),
                GridColumnInfo.Fill(lang.ColReplacement),
                GridColumnInfo.Fill(lang.ColPreview),
                GridColumnInfo.Fill(lang.ColNewHue),
                GridColumnInfo.Auto(lang.ColActions)
            );

            var filterList = filters.Values.ToList();
            int dataRow = 1;
            for (int i = filterList.Count - 1; i >= 0; i--)
            {
                GraphicChangeFilter filter = filterList[i];

                // Original — show as label (changing original = key change, use delete+re-add)
                grid.AddWidget(new MyraLabel($"0x{filter.OriginalGraphic:X4}", MyraLabel.TextStyle.P, MyraLabel.AlignMode.Right), dataRow, 0);

                // Type — cycle button using wrapper panel (key change requires rebuild)
                var typeWrapper = new HorizontalStackPanel();
                void BuildTypeBtn()
                {
                    typeWrapper.Widgets.Clear();
                    var btn = new MyraButton(GetTypeName(filter.OriginalType), () =>
                    {
                        int idx = System.Array.IndexOf(TypeValues, filter.OriginalType);
                        byte newType = TypeValues[(idx + 1) % TypeValues.Length];
                        GraphicsReplacement.DeleteFilter(filter.OriginalGraphic, filter.OriginalType);
                        GraphicsReplacement.NewFilter(
                            filter.OriginalGraphic, newType,
                            filter.ReplacementGraphic, newType,
                            filter.NewHue);
                        BuildFilterList();
                    }) { Tooltip = lang.CycleTypeTooltip, MinWidth = 65 };
                    btn.Content.HorizontalAlignment = HorizontalAlignment.Center;
                    typeWrapper.Widgets.Add(btn);
                }
                BuildTypeBtn();
                grid.AddWidget(typeWrapper, dataRow, 1);

                // Preview wrapper — rebuilt in-place when replacement changes
                var previewWrapper = new HorizontalStackPanel { Spacing = 2 };
                void BuildPreview()
                {
                    previewWrapper.Widgets.Clear();
                    if (filter.OriginalType == 3)
                    {
                        previewWrapper.Widgets.Add(new MyraArtTexture(filter.OriginalGraphic));
                        previewWrapper.Widgets.Add(new MyraLabel("\u2192", MyraLabel.TextStyle.P));
                        previewWrapper.Widgets.Add(new MyraArtTexture(filter.ReplacementGraphic));
                    }
                    else
                    {
                        previewWrapper.Widgets.Add(new MyraLabel(
                            $"0x{filter.OriginalGraphic:X4} \u2192 0x{filter.ReplacementGraphic:X4}", MyraLabel.TextStyle.P));
                    }
                }
                BuildPreview();
                grid.AddWidget(previewWrapper, dataRow, 3);

                // Replacement Graphic — inline edit, immediate commit + preview update
                var replacementBox = new MyraInputBox { Text = $"0x{filter.ReplacementGraphic:X4}" };
                replacementBox.TextChangedByUser += (_, _) =>
                {
                    string txt = replacementBox.Text ?? "";
                    if (StringHelper.TryParseInt(txt, out int newReplacement) && newReplacement is >= 0 and <= ushort.MaxValue)
                    {
                        filter.ReplacementGraphic = (ushort)newReplacement;
                        filter.ReplacementType = filter.OriginalType;
                        BuildPreview();
                    }
                };
                grid.AddWidget(replacementBox, dataRow, 2);

                // Hue — inline edit, immediate commit
                var hueBox = MyraInputBox.Hue(filter.NewHue);
                hueBox.TextChangedByUser += (_, _) =>
                {
                    if (MyraInputBox.TryParseHue(hueBox.Text, out ushort hue))
                        filter.NewHue = hue;
                };
                grid.AddWidget(hueBox, dataRow, 4);

                // Delete
                ushort capturedOrigGraphic = filter.OriginalGraphic;
                byte capturedOrigType = filter.OriginalType;
                grid.AddWidget(MyraStyle.ApplyButtonDangerStyle(new MyraButton(ui.Delete, () =>
                {
                    GraphicsReplacement.DeleteFilter(capturedOrigGraphic, capturedOrigType);
                    BuildFilterList();
                }) { Tooltip = lang.DeleteTooltip }), dataRow, 5);

                dataRow++;
            }

            filtersPanel.Widgets.Add(grid);
        }

        // Add entry panel
        var addEntryPanel = new VerticalStackPanel { Visible = false, Spacing = 4 };
        var newOriginalBox = new MyraInputBox { HintText = lang.OriginalHint, Width = 170 };
        var newReplacementBox = new MyraInputBox { HintText = lang.ReplacementHint, Width = 170 };
        var newHueBox = MyraInputBox.Hue(ushort.MaxValue, 120, lang.HueHint);
        int[] newTypeIndex = { 2 }; // Default: Static

        var newTypeWrapper = new HorizontalStackPanel();
        var validationLabel = new MyraLabel("", MyraLabel.TextStyle.P) { Visible = false };

        void BuildNewTypeBtn()
        {
            newTypeWrapper.Widgets.Clear();
            newTypeWrapper.Widgets.Add(new MyraButton(TypeNames[newTypeIndex[0]], () =>
            {
                newTypeIndex[0] = (newTypeIndex[0] + 1) % TypeNames.Length;
                BuildNewTypeBtn();
            }) { Tooltip = lang.CycleTypeTooltip });
        }
        BuildNewTypeBtn();

        var addConfirmRow = new HorizontalStackPanel { Spacing = 4 };
        addConfirmRow.Widgets.Add(new MyraButton(ui.Add, () =>
        {
            string origText = newOriginalBox.Text ?? "";
            string replText = newReplacementBox.Text ?? "";

            if (!StringHelper.TryParseInt(origText, out int origGraphic) ||
                !StringHelper.TryParseInt(replText, out int replGraphic))
                return;

            if (!MyraInputBox.TryParseHue(newHueBox.Text, out ushort hue))
            {
                if (!string.IsNullOrEmpty(newHueBox.Text))
                {
                    validationLabel.Text = string.Format(lang.InvalidHue, newHueBox.Text);
                    validationLabel.Visible = true;
                    return;
                }

                hue = ushort.MaxValue;
            }

            validationLabel.Visible = false;
            byte type = TypeValues[newTypeIndex[0]];
            GraphicsReplacement.NewFilter((ushort)origGraphic, type, (ushort)replGraphic, type, hue);

            newOriginalBox.Text = "";
            newReplacementBox.Text = "";
            newHueBox.Text = "";
            newTypeIndex[0] = 2;
            BuildNewTypeBtn();
            addEntryPanel.Visible = false;
            BuildFilterList();
        }));
        addConfirmRow.Widgets.Add(new MyraButton(ui.Cancel, () =>
        {
            addEntryPanel.Visible = false;
            newOriginalBox.Text = "";
            newReplacementBox.Text = "";
            newHueBox.Text = "";
            validationLabel.Visible = false;
        }));

        var addFieldsRow1 = new HorizontalStackPanel { Spacing = 4 };
        addFieldsRow1.Widgets.Add(new MyraLabel(lang.OriginalLabel, MyraLabel.TextStyle.P));
        addFieldsRow1.Widgets.Add(newOriginalBox);
        addFieldsRow1.Widgets.Add(new MyraLabel(lang.ReplacementLabel, MyraLabel.TextStyle.P));
        addFieldsRow1.Widgets.Add(newReplacementBox);

        var addFieldsRow2 = new HorizontalStackPanel { Spacing = 4 };
        addFieldsRow2.Widgets.Add(new MyraLabel(lang.TypeLabel, MyraLabel.TextStyle.P));
        addFieldsRow2.Widgets.Add(newTypeWrapper);
        addFieldsRow2.Widgets.Add(new MyraLabel(lang.NewHueLabel, MyraLabel.TextStyle.P));
        addFieldsRow2.Widgets.Add(newHueBox);

        addEntryPanel.Widgets.Add(new MyraLabel(lang.NewEntryLabel, MyraLabel.TextStyle.H3));
        addEntryPanel.Widgets.Add(addFieldsRow1);
        addEntryPanel.Widgets.Add(addFieldsRow2);
        addEntryPanel.Widgets.Add(validationLabel);
        addEntryPanel.Widgets.Add(addConfirmRow);

        var actionRow = new HorizontalStackPanel { Spacing = 4 };
        actionRow.Widgets.Add(new MyraButton(lang.AddEntry, () => addEntryPanel.Visible = !addEntryPanel.Visible));
        actionRow.Widgets.Add(new MyraButton(lang.TargetEntity, () =>
        {
            if (World.Instance == null) return;
            World.Instance.TargetManager.SetTargeting(targeted =>
            {
                if (targeted == null) return;
                ushort graphic = 0;
                ushort hue = 0;
                byte entityType = 3;

                if (targeted is Mobile mob) { graphic = mob.Graphic; hue = mob.Hue; entityType = 1; }
                else if (targeted is Land land) { graphic = land.Graphic; hue = land.Hue; entityType = 2; }
                else if (targeted is Entity entity) { graphic = entity.Graphic; hue = entity.Hue; }
                else if (targeted is Static stat) { graphic = stat.Graphic; hue = stat.Hue; }
                else if (targeted is GameObject obj) { graphic = obj.Graphic; hue = obj.Hue; }
                else return;

                GraphicsReplacement.NewFilter(graphic, entityType, graphic, entityType, hue);
                BuildFilterList();
            });
        }) { Tooltip = lang.TargetEntityTooltip });
        actionRow.Widgets.Add(new MyraButton(ui.Import, () =>
        {
            string? json = Clipboard.GetClipboardText();
            if (json.NotNullNotEmpty() && GraphicsReplacement.ImportFromJson(json))
            {
                BuildFilterList();
                return;
            }
            GameActions.Print(lang.ClipboardInvalid, Constants.HUE_ERROR);
        }) { Tooltip = lang.ImportTooltip });
        actionRow.Widgets.Add(new MyraButton(ui.Export, () =>
        {
            GraphicsReplacement.GetJsonExport()?.CopyToClipboard();
            GameActions.Print(lang.Exported, Constants.HUE_SUCCESS);
        }) { Tooltip = lang.ExportTooltip });
        actionRow.Widgets.Add(new MyraButton(lang.ApplyAll, () =>
        {
            World? world = World.Instance;
            if (world == null) return;
            int count = 0;
            foreach (Mobile mobile in world.Mobiles.Values.ToList())
                if (!mobile.IsDestroyed && mobile.OriginalGraphic != 0) { mobile.Graphic = mobile.OriginalGraphic; count++; }
            foreach (Item item in world.Items.Values.ToList())
                if (!item.IsDestroyed && item.OriginalGraphic != 0) { item.Graphic = item.OriginalGraphic; count++; }
            GameActions.Print(string.Format(lang.Refreshed, count));
        }) { Tooltip = lang.ApplyAllTooltip });

        root.Widgets.Add(actionRow);
        root.Widgets.Add(addEntryPanel);
        root.Widgets.Add(new MyraLabel(lang.CurrentReplacements, MyraLabel.TextStyle.H3));
        BuildFilterList();
        root.Widgets.Add(new ScrollViewer { Height = 300, Content = filtersPanel });

        return root;
    }
}
