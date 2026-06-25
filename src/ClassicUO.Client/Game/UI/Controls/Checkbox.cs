// SPDX-License-Identifier: BSD-2-Clause

using ClassicUO.Assets;
using ClassicUO.Input;
using ClassicUO.Renderer;
using ClassicUO.Utility;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace ClassicUO.Game.UI.Controls
{
    public class Checkbox : Control
    {
        private bool _isChecked;
        private readonly RenderedText _text;
        private readonly TextBox _ttfText;
        private readonly bool _useTTF;
        private readonly ushort _inactive, _active;

        public Checkbox(
            ushort inactive,
            ushort active,
            string text = "",
            byte font = 0,
            ushort color = 0,
            bool isunicode = true,
            int maxWidth = 0,
            bool useTTF = false
        )
        {
            _inactive = inactive;
            _active = active;
            _useTTF = useTTF;

            ref readonly SpriteInfo gumpInfoInactive = ref Client.Game.UO.Gumps.GetGump(inactive);
            ref readonly SpriteInfo gumpInfoActive = ref Client.Game.UO.Gumps.GetGump(active);

            if (gumpInfoInactive.Texture == null || gumpInfoActive.Texture == null)
            {
                Dispose();

                return;
            }

            Width = gumpInfoInactive.UV.Width;

            if (useTTF)
            {
                int ttfHue = color == 0 ? 0xFFFF : color;
                _ttfText = TextBox.GetOne(
                    text,
                    TrueTypeLoader.EMBEDDED_FONT,
                    14,
                    ttfHue,
                    TextBox.RTLOptions.Default(maxWidth > 0 ? maxWidth : null)
                );
                Width += _ttfText.Width + 2;
                Height = Math.Max(gumpInfoInactive.UV.Width, _ttfText.Height);
            }
            else
            {
                _text = RenderedText.Create(text, color, font, isunicode, maxWidth: maxWidth);
                Width += _text.Width;
                Height = Math.Max(gumpInfoInactive.UV.Width, _text.Height);
            }

            CanMove = false;
            AcceptMouseInput = true;
        }

        public Checkbox(List<string> parts, string[] lines)
            : this(ushort.Parse(parts[3]), ushort.Parse(parts[4]))
        {
            X = int.Parse(parts[1]);
            Y = int.Parse(parts[2]);
            IsChecked = parts[5] == "1";
            LocalSerial = SerialHelper.Parse(parts[6]);
            IsFromServer = true;
        }

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    OnCheckedChanged();
                }
            }
        }

        public override ClickPriority Priority => ClickPriority.High;

        public string Text => _useTTF ? (_ttfText?.Text ?? string.Empty) : (_text?.Text ?? string.Empty);

        public event EventHandler ValueChanged;

        public override bool Draw(UltimaBatcher2D batcher, int x, int y)
        {
            if (IsDisposed)
            {
                return false;
            }

            bool ok = base.Draw(batcher, x, y);

            ref readonly SpriteInfo gumpInfo = ref Client.Game.UO.Gumps.GetGump(
                IsChecked ? _active : _inactive
            );

            batcher.Draw(
                gumpInfo.Texture,
                new Vector2(x, y),
                gumpInfo.UV,
                ShaderHueTranslator.GetHueVector(0)
            );

            if (_useTTF)
            {
                _ttfText?.Draw(batcher, x + gumpInfo.UV.Width + 2, y);
            }
            else
            {
                _text?.Draw(batcher, x + gumpInfo.UV.Width + 2, y);
            }

            return ok;
        }

        protected virtual void OnCheckedChanged() => ValueChanged.Raise(this);

        public override void OnMouseUp(int x, int y, MouseButtonType button)
        {
            if (button == MouseButtonType.Left && MouseIsOver)
            {
                IsChecked = !IsChecked;
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            _text?.Destroy();
            _ttfText?.Dispose();
        }
    }
}
