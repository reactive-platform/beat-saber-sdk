using System.Collections.Generic;
using JetBrains.Annotations;
using Reactive.Components;
using UnityEngine;

namespace Reactive.BeatSaber.Components;

[PublicAPI]
public class ImageButton : ColoredButton, IChildrenProvider, ISkewedComponent {
    #region UI Props

    public IColorSet? GradientColors0 {
        get => _gradientColors0;
        set {
            _gradientColors0 = value;
            RefreshGradientState();
            UpdateColor();
            NotifyPropertyChanged();
        }
    }

    public IColorSet? GradientColors1 {
        get => _gradientColors1;
        set {
            _gradientColors1 = value;
            RefreshGradientState();
            UpdateColor();
            NotifyPropertyChanged();
        }
    }

    public float Skew {
        get => _skew;
        set {
            _skew = value;
            ApplySkew(value);
        }
    }

    public new ICollection<ILayoutItem> Children => base.Children;

    private float _skew;
    private IColorSet? _gradientColors0;
    private IColorSet? _gradientColors1;

    private void RefreshGradientState() {
        Image.UseGradient = GradientColors0 != null || GradientColors1 != null;
    }

    protected virtual void ApplySkew(float skew) {
        Image.Skew = skew;
    }

    #endregion

    #region UI Components

    public Image Image { get; private set; } = null!;

    #endregion

    #region Color

    protected override void ApplyColor(Color color) {
        if (Colors != null) {
            Image.Color = color;
        }
        if (GradientColors0 != null) {
            Image.GradientColor0 = GetColor(GradientColors0);
        }
        if (GradientColors1 != null) {
            Image.GradientColor1 = GetColor(GradientColors1);
        }
    }

    protected override void OnInteractableChange(bool interactable) {
        UpdateColor();
    }

    #endregion

    #region Setup

    protected override void Construct(RectTransform rect) {
        //background
        Image = new Image {
            Name = "Background"
        }.WithRectExpand();
        Image.Use(rect);
        //content
        base.Construct(rect);
    }

    protected override void OnButtonStateChange() {
        base.OnButtonStateChange();
        if (IsPressed) {
            GameResources.ButtonClickSignal.Raise();
        }
    }

    #endregion
}