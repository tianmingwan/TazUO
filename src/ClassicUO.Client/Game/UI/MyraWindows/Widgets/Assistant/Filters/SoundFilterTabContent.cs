#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using ClassicUO.Configuration;
using ClassicUO.Game.Managers;
using ClassicUO.Utility;
using Myra.Graphics2D.UI;

namespace ClassicUO.Game.UI.MyraWindows.Widgets.Assistant.Filters;

public static class SoundFilterTabContent
{
    public static Widget Build()
    {
        var lang = Language.Instance.Assistant.SoundFilter;
        var ui = Language.Instance.UiCommons;
        var root = new VerticalStackPanel { Spacing = 6 };

        root.Widgets.Add(new MyraLabel(lang.HeaderDescription, MyraLabel.TextStyle.H3));

        var lastSoundPanel = new VerticalStackPanel { Spacing = 2 };
        var filtersPanel = new VerticalStackPanel { Spacing = 2 };

        void BuildFilterList()
        {
            filtersPanel.Widgets.Clear();
            var filterList = SoundFilterManager.Instance.FilteredSounds.OrderBy(x => x).ToList();

            if (filterList.Count == 0)
            {
                filtersPanel.Widgets.Add(new MyraLabel(lang.NoSoundsFiltered, MyraLabel.TextStyle.P));
                return;
            }

            filtersPanel.Widgets.Add(new MyraLabel(string.Format(lang.TotalFiltered, filterList.Count), MyraLabel.TextStyle.P));

            filtersPanel.Widgets.Add(MyraStyle.ApplyButtonDangerStyle(new MyraButton(ui.ClearAllFilters, () =>
            {
                SoundFilterManager.Instance.Clear();
                BuildFilterList();
            })));

            var grid = new MyraGrid();
            grid.SetupWithHeaders(
                GridColumnInfo.Auto(lang.ColSoundId),
                GridColumnInfo.Fill(lang.ColActions)
            );

            int dataRow = 1;
            for (int i = filterList.Count - 1; i >= 0; i--)
            {
                int soundId = filterList[i];

                // Track current ID so we can remove-old/add-new on edit without rebuilding
                int[] current = { soundId };
                var soundBox = new MyraInputBox { Text = soundId.ToString() };
                soundBox.TextChangedByUser += (_, _) =>
                {
                    if (int.TryParse(soundBox.Text, out int newId))
                    {
                        newId = Math.Clamp(newId, 0, 65535);
                        if (newId != current[0])
                        {
                            SoundFilterManager.Instance.RemoveFilter(current[0]);
                            SoundFilterManager.Instance.AddFilter(newId);
                            current[0] = newId;
                        }
                    }
                };
                grid.AddWidget(soundBox, dataRow, 0);

                int capturedId = soundId;
                var actionsPanel = new HorizontalStackPanel { Spacing = 4 };
                actionsPanel.Widgets.Add(
                    new MyraButton(ui.Play, () => Client.Game.Audio.PlaySound(current[0], true))
                    {
                        Tooltip = lang.PlayTooltip,
                    }
                );
                actionsPanel.Widgets.Add(
                    MyraStyle.ApplyButtonDangerStyle(
                        new MyraButton(
                            ui.Delete,
                            () =>
                            {
                                SoundFilterManager.Instance.RemoveFilter(current[0]);
                                BuildFilterList();
                            }
                        )
                        {
                            Tooltip = lang.DeleteTooltip,
                        }
                    )
                );

                grid.AddWidget(actionsPanel, dataRow, 1);


                dataRow++;
            }

            filtersPanel.Widgets.Add(grid);
        }

        void BuildLastSoundSection()
        {
            lastSoundPanel.Widgets.Clear();
            lastSoundPanel.Widgets.Add(new MyraLabel(lang.RecentlyPlayed, MyraLabel.TextStyle.H3));

            int c = 0;
            foreach ((int, string) sound in Client.Game.Audio.LastPlayedSounds.GetItems())
            {
                c++;

                int id = sound.Item1;

                var row = new HorizontalStackPanel { Spacing = 4 };
                row.Widgets.Add(new MyraLabel(string.Format(lang.SoundIdLabel, id, sound.Item2), MyraLabel.TextStyle.P));
                row.Widgets.Add(new MyraButton(ui.AddFilter, () =>
                {
                    SoundFilterManager.Instance.AddFilter(id);
                    BuildFilterList();
                }) { Tooltip = lang.AddFilterTooltip });
                row.Widgets.Add(new MyraButton(ui.PlayAgain, () =>
                    Client.Game.Audio.PlaySound(id, true)) { Tooltip = lang.PlayAgainTooltip });
                lastSoundPanel.Widgets.Add(row);
            }

            lastSoundPanel.Widgets.Add(new MyraButton(ui.Refresh, () => BuildLastSoundSection())
            {
                Tooltip = lang.RefreshTooltip
            }.PlaceBefore(new MyraLabel(lang.TipText, MyraLabel.TextStyle.P)));

            if (c == 0)
            {
                var row = new HorizontalStackPanel { Spacing = 4 };
                row.Widgets.Add(new MyraLabel(lang.NoSoundPlayed, MyraLabel.TextStyle.P));
                row.Widgets.Add(new MyraButton(ui.Refresh, () => BuildLastSoundSection())
                    { Tooltip = lang.RefreshTooltip });
                lastSoundPanel.Widgets.Add(row);
            }
        }

        var addFilterPanel = new VerticalStackPanel { Visible = false, Spacing = 4 };
        var newSoundBox = new MyraInputBox { HintText = lang.SoundIdHint, Width = 120 };

        var addConfirmRow = new HorizontalStackPanel { Spacing = 4 };
        addConfirmRow.Widgets.Add(new MyraButton(ui.Add, () =>
        {
            if (int.TryParse(newSoundBox.Text, out int soundId))
            {
                soundId = Math.Clamp(soundId, 0, 65535);
                SoundFilterManager.Instance.AddFilter(soundId);
                newSoundBox.Text = "";
                addFilterPanel.Visible = false;
                BuildFilterList();
            }
        }));
        addConfirmRow.Widgets.Add(new MyraButton(ui.TestPlay, () =>
        {
            if (int.TryParse(newSoundBox.Text, out int soundId))
                Client.Game.Audio.PlaySound(Math.Clamp(soundId, 0, 65535), true);
        }) { Tooltip = lang.TestPlayTooltip });
        addConfirmRow.Widgets.Add(new MyraButton(ui.Cancel, () =>
        {
            addFilterPanel.Visible = false;
            newSoundBox.Text = "";
        }));

        var addFieldRow = new HorizontalStackPanel { Spacing = 4 };
        addFieldRow.Widgets.Add(new MyraLabel(lang.SoundIdLabelPlain, MyraLabel.TextStyle.P)
            { Tooltip = lang.SoundIdLabelTooltip });
        addFieldRow.Widgets.Add(newSoundBox);

        addFilterPanel.Widgets.Add(new MyraLabel(lang.AddSoundFilterLabel, MyraLabel.TextStyle.H3));
        addFilterPanel.Widgets.Add(addFieldRow);
        addFilterPanel.Widgets.Add(addConfirmRow);

        var actionRow = new HorizontalStackPanel { Spacing = 4 };
        actionRow.Widgets.Add(new MyraButton(lang.AddFilterEntry, () => addFilterPanel.Visible = !addFilterPanel.Visible));
        actionRow.Widgets.Add(new MyraButton(ui.Import, () =>
        {
            try
            {
                string? json = Clipboard.GetClipboardText();
                if (string.IsNullOrWhiteSpace(json))
                {
                    GameActions.Print(lang.ClipboardEmpty, Constants.HUE_ERROR);
                    return;
                }

                HashSet<int>? importedFilters = JsonSerializer.Deserialize(json, HashSetIntContext.Default.HashSetInt32);
                if (importedFilters == null)
                {
                    GameActions.Print(lang.ParseFailed, Constants.HUE_ERROR);
                    return;
                }

                int added = 0;
                foreach (int id in importedFilters)
                {
                    if (SoundFilterManager.Instance.FilteredSounds.Add(Math.Clamp(id, 0, 65535)))
                        added++;
                }
                SoundFilterManager.Instance.Save();
                BuildFilterList();
                GameActions.Print(string.Format(lang.ImportSuccess, added), Constants.HUE_SUCCESS);
            }
            catch (Exception ex)
            {
                GameActions.Print(string.Format(lang.ImportFailed, ex.Message), Constants.HUE_ERROR);
            }
        }) { Tooltip = lang.ImportTooltip });
        actionRow.Widgets.Add(new MyraButton(ui.Export, () =>
        {
            try
            {
                string json = JsonSerializer.Serialize(
                    SoundFilterManager.Instance.FilteredSounds,
                    HashSetIntContext.Default.HashSetInt32);
                json.CopyToClipboard();
                GameActions.Print(
                    string.Format(lang.ExportSuccess, SoundFilterManager.Instance.FilteredSounds.Count),
                    Constants.HUE_SUCCESS);
            }
            catch (Exception ex)
            {
                GameActions.Print(string.Format(lang.ExportFailed, ex.Message), Constants.HUE_ERROR);
            }
        }) { Tooltip = lang.ExportTooltip });

        BuildLastSoundSection();
        root.Widgets.Add(lastSoundPanel);
        root.Widgets.Add(actionRow);
        root.Widgets.Add(addFilterPanel);
        root.Widgets.Add(new MyraLabel(lang.FilteredSoundsLabel, MyraLabel.TextStyle.H3));
        BuildFilterList();
        root.Widgets.Add(new ScrollViewer { Height = 250, Content = filtersPanel });

        return root;
    }
}
