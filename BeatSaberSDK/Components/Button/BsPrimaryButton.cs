using System.Linq;
using HMUI;
using JetBrains.Annotations;
using Reactive.Components;
using UnityEngine;

namespace Reactive.BeatSaber.Components {
    [PublicAPI]
    public class BsPrimaryButton : ImageButton {
        private static readonly Material buttonMaterial = Resources
            .FindObjectsOfTypeAll<Material>()
            .First(static x => x.name == "AnimatedButton");

        private static readonly Material buttonBorderMaterial = Resources
            .FindObjectsOfTypeAll<Material>()
            .First(static x => x.name == "AnimatedButtonBorder");
        
        private Image _borderImage = null!;

        protected override void ApplySkew(float skew) {
            base.ApplySkew(skew);
            _borderImage.Skew = skew;
        }

        protected override void ApplyColor(Color color) {
            if (Interactable) {
                Image.GradientColor0 = color;
                _borderImage.Color = color;
                color.a /= 2f;
                Image.GradientColor1 = color;
            } else {
                Image.GradientColor0 = Color.white;
                Image.GradientColor1 = Color.white;
            }
        }

        protected override void OnInteractableChange(bool interactable) {
            Image.Color = interactable ? Color.white : GetColor(Colors);
            _borderImage.Enabled = interactable;
            Image.Material = interactable ? buttonMaterial : GameResources.UINoGlowMaterial;
            base.OnInteractableChange(interactable);
        }

        protected override void Construct(RectTransform rect) {
            //border
            new Image {
                Sprite = BeatSaberResources.Sprites.frame,
                Material = buttonBorderMaterial,
                PixelsPerUnit = 15f
            }.WithRectExpand().Bind(ref _borderImage);
            base.Construct(rect);
            //applying later to move it over the content
            _borderImage.Use(rect);
        }

        protected override void OnInitialize() {
            Colors = UIStyle.PrimaryButtonColorSet;
            Image.Material = buttonMaterial;
            Image.Sprite = BeatSaberResources.Sprites.background;
            Image.PixelsPerUnit = 15f;
            Image.UseGradient = true;
            Image.GradientDirection = ImageView.GradientDirection.Vertical;
            Skew = UIStyle.Skew;
        }
    }
}