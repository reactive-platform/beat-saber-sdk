using System;
using JetBrains.Annotations;
using Reactive.Components;
using TMPro;
using UnityEngine;

namespace Reactive.BeatSaber.Components {
    [PublicAPI]
    public class TextDropdown<TKey> : Dropdown<TKey, string, TextDropdown<TKey>.ComponentCell> {
        #region Cell

        public class ComponentCell : KeyedControlCell<TKey, string>, ISkewedComponent, IPreviewableCell {
            #region Setup

            public bool UsedAsPreview {
                set {
                    _button.RaycastTarget = !value;
                    _label.RaycastTarget = !value;
                }
            }
            
            public float Skew {
                get => throw new NotImplementedException();
                set {
                    _label.FontStyle = value > 0f ? FontStyles.Italic : FontStyles.Normal;
                    _button.Image.Skew = value;
                }
            }

            public override void OnInit(TKey item, string text) {
                _label.Text = text;
            }

            #endregion

            #region Construct

            private Label _label = null!;
            private ImageButton _button = null!;

            protected override GameObject Construct() {
                return new BackgroundButton {
                    Image = {
                        Sprite = BeatSaberResources.Sprites.rectangle,
                        Material = GameResources.UINoGlowMaterial
                    },
                    Colors = new StateColorSet {
                        States = {
                            GraphicState.Hovered.WithColor(BeatSaberStyle.ControlColorSet.HoveredColor),
                            GraphicState.None.WithColor(Color.clear)
                        }
                    },
                    OnClick = SelectSelf,
                    Children = {
                        new Label().WithRectExpand().Bind(ref _label)
                    }
                }.Bind(ref _button).Use();
            }

            #endregion

            #region Callbacks

            public override void OnCellStateChange(bool selected) {
                _label.Color = selected ? BeatSaberStyle.SelectedTextColor : BeatSaberStyle.TextColor;
            }

            #endregion
        }

        #endregion
    }
}