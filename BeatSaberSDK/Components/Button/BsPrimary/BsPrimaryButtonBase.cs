using System;
using System.Linq;
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
                _color = value;
                RefreshColor();
            }
        }

        public float Skew {
            get => _button.Skew;
            set {
                _button.Skew = value;
                _border.Skew = value;
            }
        }

        public bool Interactable {
            get => _button.Interactable;
            set {
                _button.Interactable = value;
                RefreshColor();
            }
        }

        public bool IsPressed => _button.IsPressed;

        public bool IsHovered => _button.IsHovered;

        public Action? OnClick { get; set; }

        private Color _color;

        #endregion

        #region Abstraction

        protected virtual void RefreshColor() {
            if (Interactable) {
                _button.Image.GradientColor0 = _color;
                _border.Color = _color;

                var color = _color;
                color.a /= 2f;
                Background.GradientColor1 = color;
            } else {
                Background.GradientColor0 = Color.white;
                Background.GradientColor1 = Color.white;
            }
        }

        protected virtual void OnInteractableChanged(bool interactable) {
            Background.Color = interactable ? Color.white : _button.Colors!.GetColor(_button.GraphicState);
            Background.Material = interactable ? buttonMaterial : GameResources.UINoGlowMaterial;
            _border.Enabled = interactable;
        }

        #endregion

        #region Setup

        private static readonly Material buttonMaterial = Resources
            .FindObjectsOfTypeAll<Material>()
            .First(static x => x.name == "AnimatedButton");

        private static readonly Material buttonBorderMaterial = Resources
            .FindObjectsOfTypeAll<Material>()
            .First(static x => x.name == "AnimatedButtonBorder");

        private Image Background => _button.Image;
        ButtonBase IComponentHolder<ButtonBase>.Component => _button;

        private Image _border = null!;
        private ImageButton _button = null!;

        protected override GameObject Construct() {
            return new ImageButton {
                Image = {
                    Material = buttonMaterial,
                    Sprite = BeatSaberResources.Sprites.background,
                    PixelsPerUnit = 15f,
                    UseGradient = true,
                    GradientDirection = ImageView.GradientDirection.Vertical
                },

                Skew = BeatSaberStyle.Skew,
                Colors = BeatSaberStyle.PrimaryButtonColorSet
            }.With(
                x => {
                    new Image {
                            Sprite = BeatSaberResources.Sprites.frame,
                            Material = buttonBorderMaterial,
                            PixelsPerUnit = 15f
                        }
                        .WithRectExpand()
                        .Bind(ref _border)
                        .Use(x.ContentTransform);

                    ConstructContent()
                        .WithRectExpand()
                        .Use(x.ContentTransform);
                }
            ).Bind(ref _button).Use();
        }

        protected abstract IReactiveComponent ConstructContent();

        #endregion
    }
}