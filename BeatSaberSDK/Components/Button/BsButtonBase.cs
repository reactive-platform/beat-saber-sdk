using System;
using JetBrains.Annotations;
using Reactive.Components;
using UnityEngine;

namespace Reactive.BeatSaber.Components {
    /// <summary>
    /// Represents a content-less Beat Saber styled button.
    /// </summary>
    [PublicAPI]
    public abstract class BsButtonBase : ReactiveComponent, ISkewedComponent, IInteractableComponent {
        #region UI Props

        public bool ShowUnderline {
            get => _underline.Enabled;
            set => _underline.Enabled = value;
        }

        public float Skew {
            get => _button.Skew;
            set {
                _button.Skew = value;
                _underline.Skew = value;
            }
        }

        public bool Interactable {
            get => _button.Interactable;
            set => _button.Interactable = value;
        }

        public bool IsPressed => _button.IsPressed;

        public bool IsHovered => _button.IsHovered;

        public Action? OnClick { get; set; }

        #endregion

        #region Setup

        private Image _underline = null!;
        private ImageButton _button = null!;

        protected override GameObject Construct() {
            return new ImageButton {
                Colors = new SimpleColorSet {
                    ActiveColor = Color.white.ColorWithAlpha(0.5f),
                    HoveredColor = Color.white.ColorWithAlpha(0.3f),
                    Color = UIStyle.ControlColorSet.Color
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
            }.With(
                x => {
                    new Image {
                            Sprite = BeatSaberResources.Sprites.backgroundUnderline,
                            PixelsPerUnit = 12f,
                            ImageType = UnityEngine.UI.Image.Type.Sliced,
                            Color = Color.white.ColorWithAlpha(0.5f)
                        }
                        .WithRectExpand()
                        .Bind(ref _underline)
                        .Use(x.ContentTransform);

                    ConstructContent()
                        .WithRectExpand()
                        .Use(x.ContentTransform);
                }
            ).Bind(ref _button).Use();
        }

        protected abstract IReactiveComponent ConstructContent();

        protected override void OnInitialize() {
            Skew = UIStyle.Skew;
        }

        #endregion
    }
}