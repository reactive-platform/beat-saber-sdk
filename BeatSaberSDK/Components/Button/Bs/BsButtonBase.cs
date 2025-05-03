using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Reactive.Components;
using Reactive.Yoga;
using UnityEngine;

namespace Reactive.BeatSaber.Components {
    /// <summary>
    /// A content-less Beat Saber styled button.
    /// </summary>
    [PublicAPI]
    public abstract class BsButtonBase : ReactiveComponent, IComponentHolder<ButtonBase>, ISkewedComponent, IInteractableComponent {
        #region UI Props

        public bool ShowUnderline {
            get => _showUnderline;
            set {
                _showUnderline = value;
                _underline.Enabled = value && Interactable;
            }
        }

        public float Skew {
            get => _button.Skew;
            set {
                _button.Skew = value;
                _underline.Skew = value;
                OnSkewChanged(value);
            }
        }

        public bool Interactable {
            get => _button.Interactable;
            set {
                _button.Interactable = value;
                _underline.Enabled = _showUnderline && value;
                OnGraphicStateChanged();
            }
        }

        public GraphicState GraphicState => _button.WrappedButton.GraphicState;

        public bool IsPressed => _button.IsPressed;
        public bool IsHovered => _button.IsHovered;

        public Action? OnClick { get; set; }

        private bool _showUnderline;

        #endregion

        #region Setup

        protected ILayoutController? LayoutController {
            get => _button.LayoutController;
            set => _button.LayoutController = value;
        }

        ButtonBase IComponentHolder<ButtonBase>.Component => _button.WrappedButton;

        private Image _underline = null!;
        private BackgroundButton _button = null!;

        protected override GameObject Construct() {
            return new BackgroundButton {
                Colors = new SimpleColorSet {
                    ActiveColor = Color.white.ColorWithAlpha(0.5f),
                    HoveredColor = Color.white.ColorWithAlpha(0.3f),
                    Color = BeatSaberStyle.ControlColorSet.Color,
                    NotInteractableColor = BeatSaberStyle.ControlColorSet.NotInteractableColor
                },

                GradientColors1 = new SimpleColorSet {
                    HoveredColor = Color.white,
                    Color = Color.white.ColorWithAlpha(0.5f)
                },

                Image = {
                    GradientColor0 = Color.white,
                    UseGradient = true,
                    Sprite = BeatSaberResources.Sprites.background,
                    PixelsPerUnit = 12f
                },

                OnClick = () => OnClick?.Invoke()
            }.With(x => {
                    x.WrappedButton
                        .WithListener(
                            y => y.IsHovered,
                            _ => OnGraphicStateChanged()
                        ).WithListener(
                            y => y.IsPressed,
                            _ => OnGraphicStateChanged()
                        );

                    new Image {
                            Sprite = BeatSaberResources.Sprites.backgroundUnderline,
                            PixelsPerUnit = 12f,
                            ImageType = UnityEngine.UI.Image.Type.Sliced,
                            Color = Color.white.ColorWithAlpha(0.5f)
                        }
                        .WithRectExpand()
                        .Bind(ref _underline)
                        .Use(x.ContentTransform);

                    x.Children.AddRange(ConstructContent());
                }
            ).AsFlexGroup(
                justifyContent: Justify.SpaceAround,
                padding: new() { left = 1f, right = 1f }
            ).Bind(ref _button).Use();
        }

        protected abstract IEnumerable<IReactiveComponent> ConstructContent();

        protected virtual void OnSkewChanged(float skew) { }
        protected virtual void OnGraphicStateChanged() { }

        protected override void OnInitialize() {
            Skew = BeatSaberStyle.Skew;
            ShowUnderline = true;

            OnGraphicStateChanged();
        }

        #endregion
    }
}