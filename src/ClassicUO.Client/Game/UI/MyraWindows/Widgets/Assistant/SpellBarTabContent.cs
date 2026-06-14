#nullable enable
using System;
using ClassicUO.Configuration;
using ClassicUO.Game.Managers;
using ClassicUO.Input;
using Myra.Graphics2D.UI;
using SDL3;

namespace ClassicUO.Game.UI.MyraWindows.Widgets.Assistant;

public static class SpellBarTabContent
{
    public static Widget Build()
    {
        var lang = Language.Instance.Assistant.SpellBar;
        var common = Language.Instance.UiCommons;

        Profile profile = ProfileManager.CurrentProfile;

        int listeningSlot = -1;
        SDL.SDL_Keycode capturedKey = SDL.SDL_Keycode.SDLK_UNKNOWN;
        SDL.SDL_Keymod capturedMod = SDL.SDL_Keymod.SDL_KMOD_NONE;
        Action? currentUnsubscribe = null;

        var keyLabels = new MyraLabel[10];
        var normalPanels = new HorizontalStackPanel[10];
        var editPanels = new HorizontalStackPanel[10];

        string GetKeyDisplay(int slot) =>
            SpellBarManager.GetKetNames(slot) is { Length: > 0 } s ? s : lang.NoneHotkey;

        void StopListening()
        {
            currentUnsubscribe?.Invoke();
            currentUnsubscribe = null;
            int prev = listeningSlot;
            listeningSlot = -1;
            capturedKey = SDL.SDL_Keycode.SDLK_UNKNOWN;
            capturedMod = SDL.SDL_Keymod.SDL_KMOD_NONE;
            if (prev >= 0 && prev < 10)
            {
                keyLabels[prev].Text = GetKeyDisplay(prev);
                normalPanels[prev].Visible = true;
                editPanels[prev].Visible = false;
            }
        }

        void ApplyCapturedHotkey()
        {
            if (listeningSlot < 0) return;
            SpellBarManager.SetButtons(listeningSlot, capturedMod, capturedKey, []);
            Game.UI.Gumps.SpellBar.SpellBar.Instance?.SetupHotkeyLabels();
            StopListening();
        }

        void StartListening(int slot)
        {
            StopListening();
            listeningSlot = slot;
            capturedKey = SDL.SDL_Keycode.SDLK_UNKNOWN;
            capturedMod = SDL.SDL_Keymod.SDL_KMOD_NONE;

            keyLabels[slot].Text = lang.PressAKey;
            normalPanels[slot].Visible = false;
            editPanels[slot].Visible = true;

            void Handler(string hotkey)
            {
                (capturedKey, capturedMod) = ParseHotKeyString(hotkey);
                keyLabels[slot].Text = KeysTranslator.TryGetKey(capturedKey, capturedMod);
            }

            Keyboard.KeyDownEvent += Handler;
            currentUnsubscribe = () => Keyboard.KeyDownEvent -= Handler;
        }

        var leftCol = new VerticalStackPanel { Spacing = 6 };

        leftCol.Widgets.Add(MyraCheckButton.CreateWithCallback(
            SpellBarManager.IsEnabled(),
            _ =>
            {
                if (SpellBarManager.ToggleEnabled())
                    UIManager.Add(new Game.UI.Gumps.SpellBar.SpellBar(Client.Game.UO.World));
                else
                    Game.UI.Gumps.SpellBar.SpellBar.Instance?.Dispose();
            },
            lang.EnableSpellbar, lang.EnableSpellbarTooltip));

        leftCol.Widgets.Add(MyraCheckButton.CreateWithCallback(
            profile.SpellBar_ShowHotkeys,
            b =>
            {
                profile.SpellBar_ShowHotkeys = b;
                Game.UI.Gumps.SpellBar.SpellBar.Instance?.SetupHotkeyLabels();
            },
            lang.DisplayHotkeys, lang.DisplayHotkeysTooltip));

        leftCol.Widgets.Add(new MyraSpacer(15, 5));
        leftCol.Widgets.Add(new MyraLabel(lang.RowManagement, MyraLabel.TextStyle.H2));
        var rowBtns = new HorizontalStackPanel { Spacing = 4 };
        rowBtns.Widgets.Add(new MyraButton(lang.AddRow, () =>
        {
            SpellBarManager.SpellBarRows.Add(new SpellBarRow());
            Game.UI.Gumps.SpellBar.SpellBar.Instance?.Build();
        }) { Tooltip = lang.AddRowTooltip });
        rowBtns.Widgets.Add(new MyraButton(lang.RemoveRow, () =>
        {
            if (SpellBarManager.SpellBarRows.Count > 1)
                SpellBarManager.SpellBarRows.RemoveAt(SpellBarManager.SpellBarRows.Count - 1);
            Game.UI.Gumps.SpellBar.SpellBar.Instance?.Build();
        }) { Tooltip = lang.RemoveRowTooltip });
        leftCol.Widgets.Add(rowBtns);

        leftCol.Widgets.Add(new MyraSpacer(15, 5));
        leftCol.Widgets.Add(new MyraLabel(lang.PresetManagement, MyraLabel.TextStyle.H2));

        var presetSavePanel = new VerticalStackPanel { Spacing = 4, Visible = false };
        var presetNameBox = new MyraInputBox { MinWidth = 150, HintText = lang.PresetNameHint };
        var presetSaveRow = new HorizontalStackPanel { Spacing = 4 };
        presetSaveRow.Widgets.Add(new MyraLabel(lang.NameLabel, MyraLabel.TextStyle.P));
        presetSaveRow.Widgets.Add(presetNameBox);
        presetSaveRow.Widgets.Add(new MyraButton(common.Save, () =>
        {
            if (!string.IsNullOrEmpty(presetNameBox.Text))
            {
                SpellBarManager.SaveCurrentRowPreset(presetNameBox.Text);
                presetNameBox.Text = "";
                presetSavePanel.Visible = false;
            }
        }));
        presetSaveRow.Widgets.Add(new MyraButton(common.Cancel, () =>
        {
            presetNameBox.Text = "";
            presetSavePanel.Visible = false;
        }));
        presetSavePanel.Widgets.Add(presetSaveRow);

        var presetLoadPanel = new VerticalStackPanel { Spacing = 4, Visible = false };
        var presetListPanel = new VerticalStackPanel { Spacing = 2 };
        presetLoadPanel.Widgets.Add(presetListPanel);
        presetLoadPanel.Widgets.Add(new MyraButton(common.Cancel, () => presetLoadPanel.Visible = false));

        var presetActionBtns = new HorizontalStackPanel { Spacing = 4 };
        presetActionBtns.Widgets.Add(new MyraButton(lang.SavePreset, () =>
        {
            presetLoadPanel.Visible = false;
            presetSavePanel.Visible = !presetSavePanel.Visible;
        }) { Tooltip = lang.SavePresetTooltip });
        presetActionBtns.Widgets.Add(new MyraButton(lang.LoadPreset, () =>
        {
            presetSavePanel.Visible = false;

            presetListPanel.Widgets.Clear();
            string[] presets = SpellBarManager.ListPresets();
            if (presets.Length == 0)
            {
                presetListPanel.Widgets.Add(new MyraLabel(lang.NoPresetsAvailable, MyraLabel.TextStyle.P));
            }
            else
            {
                presetListPanel.Widgets.Add(new MyraLabel(lang.SelectPresetToLoad, MyraLabel.TextStyle.P));
                foreach (string preset in presets)
                {
                    string p = preset;
                    presetListPanel.Widgets.Add(new MyraButton(p, () =>
                    {
                        SpellBarManager.ImportPreset(p);
                        presetLoadPanel.Visible = false;
                    }));
                }
            }

            presetLoadPanel.Visible = !presetLoadPanel.Visible;
        }) { Tooltip = lang.LoadPresetTooltip });

        leftCol.Widgets.Add(presetActionBtns);
        leftCol.Widgets.Add(presetSavePanel);
        leftCol.Widgets.Add(presetLoadPanel);

        var rightCol = new VerticalStackPanel { Spacing = 6 };
        rightCol.Widgets.Add(new MyraLabel(lang.HotkeyConfiguration, MyraLabel.TextStyle.H2));

        var hotkeyGrid = new MyraGrid();
        hotkeyGrid.AddColumn(new Proportion(ProportionType.Pixels, 60));
        hotkeyGrid.AddColumn(new Proportion(ProportionType.Pixels, 8));
        hotkeyGrid.AddColumn(new Proportion(ProportionType.Auto));
        hotkeyGrid.AddColumn(new Proportion(ProportionType.Pixels, 8));
        hotkeyGrid.AddColumn(new Proportion(ProportionType.Auto));

        for (int i = 0; i < 10; i++)
        {
            int slot = i;

            keyLabels[slot] = new MyraLabel(GetKeyDisplay(slot), MyraLabel.TextStyle.P);

            normalPanels[slot] = new HorizontalStackPanel { Spacing = 4 };
            normalPanels[slot].Widgets.Add(new MyraButton(lang.Set, () => StartListening(slot)));
            normalPanels[slot].Widgets.Add(new MyraButton(lang.Clear, () =>
            {
                SpellBarManager.SetButtons(slot, SDL.SDL_Keymod.SDL_KMOD_NONE, SDL.SDL_Keycode.SDLK_UNKNOWN, []);
                keyLabels[slot].Text = GetKeyDisplay(slot);
                Game.UI.Gumps.SpellBar.SpellBar.Instance?.SetupHotkeyLabels();
            }));

            editPanels[slot] = new HorizontalStackPanel { Spacing = 4, Visible = false };
            editPanels[slot].Widgets.Add(new MyraButton(common.Apply, () => ApplyCapturedHotkey()));
            editPanels[slot].Widgets.Add(new MyraButton(common.Cancel, () => StopListening()));

            var actionsContainer = new VerticalStackPanel();
            actionsContainer.Widgets.Add(normalPanels[slot]);
            actionsContainer.Widgets.Add(editPanels[slot]);

            hotkeyGrid.AddWidget(new MyraLabel(string.Format(common.Slot, slot), MyraLabel.TextStyle.P), slot, 0);
            hotkeyGrid.AddWidget(keyLabels[slot], slot, 2);
            hotkeyGrid.AddWidget(actionsContainer, slot, 4);
        }

        rightCol.Widgets.Add(hotkeyGrid);

        var root = new HorizontalStackPanel { Spacing = 20 };
        root.Widgets.Add(leftCol);
        root.Widgets.Add(rightCol);

        return root;
    }

    private static (SDL.SDL_Keycode key, SDL.SDL_Keymod mod) ParseHotKeyString(string hotkey)
    {
        SDL.SDL_Keycode key = SDL.SDL_Keycode.SDLK_UNKNOWN;
        SDL.SDL_Keymod mod = SDL.SDL_Keymod.SDL_KMOD_NONE;

        if (string.IsNullOrEmpty(hotkey))
            return (key, mod);

        foreach (string part in hotkey.Split('+'))
        {
            switch (part.ToUpperInvariant())
            {
                case "CTRL":  mod |= SDL.SDL_Keymod.SDL_KMOD_CTRL;  break;
                case "SHIFT": mod |= SDL.SDL_Keymod.SDL_KMOD_SHIFT; break;
                case "ALT":   mod |= SDL.SDL_Keymod.SDL_KMOD_ALT;   break;
                default:
                    if (Enum.TryParse<SDL.SDL_Keycode>(part, true, out SDL.SDL_Keycode parsed))
                        key = parsed;
                    break;
            }
        }

        return (key, mod);
    }
}
