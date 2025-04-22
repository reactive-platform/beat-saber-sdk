using System;
using System.Collections.Generic;
using HMUI;
using JetBrains.Annotations;
using Reactive.Components;
using UnityEngine;

namespace Reactive.BeatSaber.Components {
    /// <summary>
    /// A content-less Beat Saber styled primary button.
    /// </summary>
    [PublicAPI]
    public abstract class BsPrimaryButtonBase : ReactiveComponent, IComponentHolder<ButtonBase>, ISkewedComponent, IInteractableComponent {
        #region UI Props

        public Color Color {
            get => _color;
            set {
                _color = value.ColorWithAlpha(1f);

                Color.RGBToHSV(_color, out var h, out var s, out var v);

                _highlightColor = Color.HSVToRGB(h, s, v * 1.2f);
                _border.Color = _highlightColor;

                RefreshColor();
            }
        }

        public float Skew {
            get => _background.Skew;
            set {
                _background.Skew = value;
                _border.Skew = value;
                OnSkewChanged(value);
            }
        }

        public bool Interactable {
            get => _button.Interactable;
            set {
                _background.Material = value ? GameResources.AnimatedButtonMaterial : GameResources.UINoGlowMaterial;
                _border.Enabled = value;
                _button.Interactable = value;

                RefreshColor();
            }
        }

        public GraphicState GraphicState => GraphicState.None
            .AddIf(GraphicState.Hovered, IsHovered)
            .AddIf(GraphicState.Pressed, IsPressed)
            .AddIf(GraphicState.NonInteractable, !Interactable);

        public bool IsPressed => _button.IsPressed;
        public bool IsHovered => _button.IsHovered;

        public Action? OnClick { get; set; }

        #endregion

        #region Colors

        private Color _highlightColor;
        private Color _color;

        private void RefreshColor() {
            if (Interactable) {
                var baseColor = IsHovered ? _highlightColor : _color;

                _background.GradientColor0 = baseColor;
                _background.GradientColor1 = baseColor.ColorWithAlpha(0.5f);
                _background.Color = Color.white;
                _background.UseGradient = true;
            } else {
                _background.GradientColor0 = Color.white;
                _background.GradientColor1 = Color.white;
                _background.Color = Color.black.ColorWithAlpha(0.25f);
                _background.UseGradient = false;
            }

            OnColorChanged();
        }

        #endregion

        #region Setup

        protected ILayoutController? LayoutController {
            get => _button.LayoutController;
            set => _button.LayoutController = value;
        }

        ButtonBase IComponentHolder<ButtonBase>.Component => _button;

        private Image _border = null!;
        private Image _background = null!;
        private Clickable _button = null!;

        protected override GameObject Construct() {
            return new Clickable {
                OnClick = () => {
                    OnClick?.Invoke();
                    GameResources.ButtonClickSignal.Raise();
                },
                
                Children = {
                    // Background 
                    new Image {
                            Material = GameResources.AnimatedButtonMaterial,
                            Sprite = BeatSaberResources.Sprites.background,
                            PixelsPerUnit = 15f,
                            UseGradient = true,
                            GradientDirection = ImageView.GradientDirection.Vertical
                        }
                        .WithRectExpand()
                        .Bind(ref _background),

                    // Border
                    new Image {
                            Sprite = BeatSaberResources.Sprites.frame,
                            Material = GameResources.AnimatedButtonBorderMaterial,
                            PixelsPerUnit = 15f
                        }
                        .WithRectExpand()
                        .Bind(ref _border)
                }
            }.With(x => {
                    x.WithListener(
                        y => y.IsHovered,
                        _ => RefreshColor()
                    ).WithListener(
                        y => y.IsPressed,
                        _ => RefreshColor()
                    );

                    x.Children.AddRange(ConstructContent());
                }
            ).AsFlexGroup().Bind(ref _button).Use();
        }

        protected abstract IEnumerable<IReactiveComponent> ConstructContent();

        protected virtual void OnSkewChanged(float skew) { }
        protected virtual void OnColorChanged() { }

        protected override void OnInitialize() {
            Skew = BeatSaberStyle.Skew;
            Color = new(0f, 0.5f, 1f);
        }

        #endregion
    }
}