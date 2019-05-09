﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.TextureAtlases;

namespace Blish_HUD.Controls {

    public class ContextMenuStripItem : Control, ICheckable {

        private const int BULLET_SIZE        = 18;
        private const int HORIZONTAL_PADDING = 6;

        private const int TEXT_LEFTPADDING = HORIZONTAL_PADDING + BULLET_SIZE + HORIZONTAL_PADDING;

        public event EventHandler<CheckChangedEvent> CheckedChanged;
        protected virtual void OnCheckedChanged(CheckChangedEvent e) {
            this.CheckedChanged?.Invoke(this, e);
        }

        private static Texture2D _bulletSprite;
        private static Texture2D _arrowSprite;

        private string _text;
        public string Text {
            get => _text;
            set {
                if (_text == value) return;

                _text = value;
                UpdateControlWidth(_text);
                OnPropertyChanged();
            }
        }

        private ContextMenuStrip _submenu;
        public ContextMenuStrip Submenu {
            get => _submenu;
            set {
                if (_submenu == value) return;

                _submenu = value;
                UpdateControlWidth(_text);
                OnPropertyChanged();
            }
        }

        private bool _canCheck = false;
        public bool CanCheck {
            get => _canCheck;
            set {
                if (_canCheck == value) return;

                _canCheck = value;
                OnPropertyChanged();
            }
        }

        private bool _checked = false;
        public bool Checked {
            get => _checked;
            set {
                if (_checked == value) return;

                _checked = value;
                OnPropertyChanged();
                OnCheckedChanged(new CheckChangedEvent(_checked));
            }
        }

        #region Checkbox Features

        // Basically just copying the checkbox implementation for now

        private static List<TextureRegion2D> _cbRegions;

        private static void LoadCheckboxSprites() {
            if (_cbRegions != null) return;

            _cbRegions = new List<TextureRegion2D>();

            _cbRegions.AddRange(
                                new TextureRegion2D[] {
                                    ControlAtlas.GetRegion("checkbox/cb-unchecked"),
                                    ControlAtlas.GetRegion("checkbox/cb-unchecked-active"),
                                    ControlAtlas.GetRegion("checkbox/cb-unchecked-disabled"),
                                    ControlAtlas.GetRegion("checkbox/cb-checked"),
                                    ControlAtlas.GetRegion("checkbox/cb-checked-active"),
                                    ControlAtlas.GetRegion("checkbox/cb-checked-disabled"),
                                }
                               );
        }

        #endregion
        
        public ContextMenuStripItem() {
            _bulletSprite = _bulletSprite ?? Content.GetTexture("155038");
            _arrowSprite = _arrowSprite ?? Content.GetTexture("context-menu-strip-submenu");

            LoadCheckboxSprites();

            this.EffectBehind = new Effects.ScrollingHighlightEffect(this);
        }

        public override void RecalculateLayout() {
            if (this.EffectBehind != null)
                this.EffectBehind.Size = _size.ToVector2();
        }

        private void UpdateControlWidth(string newText) {
            var textSize = Content.DefaultFont14.MeasureString(newText);
            int nWidth   = (int)textSize.Width + TEXT_LEFTPADDING + TEXT_LEFTPADDING;

            if (this.Parent == null) {
                this.Width = nWidth;
            } else {
                this.Parent.Width = Math.Max(nWidth, this.Parent.Width);
            }
        }

        protected override void OnClick(MouseEventArgs e) {
            if (this.Enabled && this.CanCheck)
                this.Checked = !this.Checked;

            base.OnClick(e);
        }

        protected override void OnMouseEntered(MouseEventArgs e) {
            this.Submenu?.Show(this);

            base.OnMouseEntered(e);
        }

        protected override void OnMouseLeft(MouseEventArgs e) {
            if (this.RelativeMousePosition.X < this.Left)
                this.Submenu?.Hide();

            base.OnMouseLeft(e);
        }

        protected override CaptureType CapturesInput() {
            return CaptureType.Mouse;
        }

        protected override void Paint(SpriteBatch spriteBatch, Rectangle bounds) {
            if (_canCheck) {
                string state = _checked ? "-checked" : "-unchecked";

                string extension = "";
                extension = this.MouseOver ? "-active" : extension;
                extension = !this.Enabled ? "-disabled" : extension;

                spriteBatch.DrawOnCtrl(this,
                                 _cbRegions.First(cb => cb.Name == $"checkbox/cb{state}{extension}"),
                                 new Rectangle(
                                               HORIZONTAL_PADDING + BULLET_SIZE / 2 - 16,
                                               _size.Y / 2 - 16,
                                               32,
                                               32
                                              ),
                                 StandardColors.Default);

            } else {
                spriteBatch.DrawOnCtrl(this,
                                 _bulletSprite,
                                 new Rectangle(
                                               HORIZONTAL_PADDING,
                                               _size.Y / 2 - BULLET_SIZE / 2,
                                               BULLET_SIZE,
                                               BULLET_SIZE
                                              ),
                                 this.MouseOver ? StandardColors.Tinted : StandardColors.Default);
            }

            // Draw shadow
            spriteBatch.DrawStringOnCtrl(this,
                                           _text,
                                           Content.DefaultFont14,
                                           new Rectangle(TEXT_LEFTPADDING + 1,
                                                         0 + 1,
                                                         _size.X - TEXT_LEFTPADDING - HORIZONTAL_PADDING,
                                                         _size.Y),
                                           StandardColors.Shadow);

            spriteBatch.DrawStringOnCtrl(this,
                                           _text,
                                           Content.DefaultFont14,
                                           new Rectangle(TEXT_LEFTPADDING,
                                                         0,
                                                         _size.X - TEXT_LEFTPADDING - HORIZONTAL_PADDING,
                                                         _size.Y),
                                           _enabled ? StandardColors.Default : StandardColors.DisabledText);

            // Indicate submenu, if there is one
            if (_submenu != null) {
                spriteBatch.DrawOnCtrl(this,
                                 _arrowSprite,
                                 new Rectangle(_size.X - HORIZONTAL_PADDING - _arrowSprite.Width,
                                               _size.Y / 2 - _arrowSprite.Height / 2,
                                               _arrowSprite.Width,
                                               _arrowSprite.Height),
                                 this.MouseOver ? StandardColors.Tinted : StandardColors.Default);
            }
        }

    }

}