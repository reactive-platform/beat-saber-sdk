using JetBrains.Annotations;
using Reactive.Components;
using UnityEngine;

namespace Reactive.BeatSaber.Components {
    [PublicAPI]
    public class BsButton : ImageButton {
        #region UI Props

        public bool ShowUnderline {
            get => _underline.Enabled;
            set => _underline.Enabled = value;
        }

        #endregion

        #region Setup

        private static readonly Color inactiveColor = Color.white;
        private static readonly Color activeColor = Color.white.ColorWithAlpha(0.5f);

        private Image _underline = null!;

        protected override void ApplySkew(float skew) {
            base.ApplySkew(skew);
            _underline.Skew = skew;
        }

        protected override void Construct(RectTransform rect) {
            //underline
            new Image {
                Sprite = BeatSaberResources.Sprites.backgroundUnderline,
                PixelsPerUnit = 12f,
                ImageType = UnityEngine.UI.Image.Type.Sliced,
                Color = Color.white.ColorWithAlpha(0.5f)
            }.WithRectExpand().Bind(ref _underline).Use(rect);
            base.Construct(rect);
        }

        protected override void OnInitialize() {
            Colors = new SimpleColorSet {
                ActiveColor = Color.white.ColorWithAlpha(0.5f),
                HoveredColor = Color.white.ColorWithAlpha(0.3f),
                Color = UIStyle.ControlColorSet.Color
            };
            GradientColors1 = new SimpleColorSet {
                HoveredColor = activeColor,
                Color = inactiveColor
            };
            Image.GradientColor0 = Color.white;
            Image.Sprite = BeatSaberResources.Sprites.background;
            Image.PixelsPerUnit = 12f;
            Image.UseGradient = true;
            Skew = UIStyle.Skew;
        }

        #endregion
    }
}