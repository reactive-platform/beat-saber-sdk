using HMUI;
using JetBrains.Annotations;
using Reactive.Components;
using UnityEngine;

namespace Reactive.BeatSaber.Components {
    [PublicAPI]
    public class AeroButton : ImageButton {
        private static readonly Color hoveredGradientColor = Color.white.ColorWithAlpha(0.5f);
        private static readonly Color defaultGradientColor = Color.white;

        protected override void OnInteractableChange(bool interactable) {
            Image.UseGradient = interactable;
            UpdateColor();
        }

        protected override void OnInitialize() {
            Image.Sprite = BeatSaberResources.Sprites.background;
            Image.ImageType = UnityEngine.UI.Image.Type.Sliced;
            Image.PixelsPerUnit = 12f;
            Image.GradientDirection = ImageView.GradientDirection.Vertical;
            Image.GradientColor0 = Color.white;
            Colors = new SimpleColorSet {
                HoveredColor = BeatSaberStyle.InputColorSet.HoveredColor.ColorWithAlpha(1f),
                Color = BeatSaberStyle.InputColorSet.Color,
                NotInteractableColor = BeatSaberStyle.InputColorSet.NotInteractableColor
            };
            GradientColors1 = new SimpleColorSet {
                Color = defaultGradientColor,
                HoveredColor = hoveredGradientColor
            };
        }
    }
}