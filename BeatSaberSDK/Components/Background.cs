using HMUI;
using JetBrains.Annotations;
using Reactive.Components;
using UnityEngine;

namespace Reactive.BeatSaber.Components;

/// <summary>
/// Layout wrapper for <see cref="Image"/>.
/// </summary>
[PublicAPI]
public class Background : ComponentLayout<Image> {
    #region Adapter

    public Sprite? Sprite {
        get => Component.Sprite;
        set => Component.Sprite = value;
    }

    public Color Color {
        get => Component.Color;
        set => Component.Color = value;
    }

    public Color GradientColor0 {
        get => Component.GradientColor0;
        set => Component.GradientColor0 = value;
    }

    public Color GradientColor1 {
        get => Component.GradientColor1;
        set => Component.GradientColor1 = value;
    }

    public bool UseGradient {
        get => Component.UseGradient;
        set => Component.UseGradient = value;
    }

    public ImageView.GradientDirection GradientDirection {
        get => Component.GradientDirection;
        set => Component.GradientDirection = value;
    }

    public Material? Material {
        get => Component.Material;
        set => Component.Material = value;
    }

    public bool PreserveAspect {
        get => Component.PreserveAspect;
        set => Component.PreserveAspect = value;
    }

    public UnityEngine.UI.Image.Type ImageType {
        get => Component.ImageType;
        set => Component.ImageType = value;
    }

    public UnityEngine.UI.Image.FillMethod FillMethod {
        get => Component.FillMethod;
        set => Component.FillMethod = value;
    }

    public float FillAmount {
        get => Component.FillAmount;
        set => Component.FillAmount = value;
    }

    public float PixelsPerUnit {
        get => Component.PixelsPerUnit;
        set => Component.PixelsPerUnit = value;
    }

    public float Skew {
        get => Component.Skew;
        set => Component.Skew = value;
    }

    public bool RaycastTarget {
        get => Component.RaycastTarget;
        set => Component.RaycastTarget = value;
    }

    #endregion

    public Image WrappedImage => Component;

    protected override void OnInitialize() {
        RoutePropertyChanged(WrappedImage, null);
        base.OnInitialize();
    }
}