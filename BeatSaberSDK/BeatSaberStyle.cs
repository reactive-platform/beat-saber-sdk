using JetBrains.Annotations;
using Reactive.Components;
using UnityEngine;

namespace Reactive.BeatSaber {
    /// <summary>
    /// Provides default colors and measurements for Beat Saber components.
    /// </summary>
    [PublicAPI]
    public static class BeatSaberStyle {
        public static readonly float Skew = 0.18f;

        public static SimpleColorSet InputColorSet => new() {
            HoveredColor = Color.magenta.ColorWithAlpha(0.5f),
            Color = Color.black.ColorWithAlpha(0.5f),
            NotInteractableColor = Color.black.ColorWithAlpha(0.2f)
        };

        public static SimpleColorSet ControlColorSet => new() {
            HoveredColor = Color.white.ColorWithAlpha(0.2f),
            Color = Color.black.ColorWithAlpha(0.5f),
            NotInteractableColor = Color.black.ColorWithAlpha(0.25f)
        };

        public static SimpleColorSet ControlButtonColorSet => new() {
            ActiveColor = new(0f, 0.75f, 1f, 1f),
            HoveredColor = Color.white.ColorWithAlpha(0.2f),
            Color = Color.black.ColorWithAlpha(0.5f),
            NotInteractableColor = Color.black.ColorWithAlpha(0.35f)
        };

        public static SimpleColorSet TextColorSet => new() {
            ActiveColor = new(0f, 0.75f, 1f, 1f),
            NotInteractableColor = Color.white.ColorWithAlpha(0.2f),
            HoveredColor = Color.white * 0.9f,
            Color = Color.white
        };

        public static readonly Color PrimaryButtonColor = new(0, 0.5f, 1f);

        public static readonly Color TextColor = Color.white;
        public static readonly Color SelectedTextColor = new(0f, 0.75f, 1f, 1f);
        public static readonly Color InactiveTextColor = Color.white.ColorWithAlpha(0.2f);
        public static readonly Color SecondaryTextColor = Color.white * 0.9f;
    }
}