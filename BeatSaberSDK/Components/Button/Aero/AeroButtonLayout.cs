using System;
using JetBrains.Annotations;
using Reactive.Components;

namespace Reactive.BeatSaber.Components;

[PublicAPI]
public class AeroButtonLayout : ComponentLayout<AeroButton> {
    #region Adapter

    public Image Image => Component.Image;

    public IColorSet? GradientColors0 {
        get => Component.GradientColors0;
        set => Component.GradientColors0 = value;
    }

    public IColorSet? GradientColors1 {
        get => Component.GradientColors1;
        set => Component.GradientColors1 = value;
    }

    public IColorSet? Colors {
        get => Component.Colors;
        set => Component.Colors = value;
    }

    public float Skew {
        get => Component.Skew;
        set => Component.Skew = value;
    }

    public bool Interactable {
        get => Component.Interactable;
        set => Component.Interactable = value;
    }

    public bool Latching {
        get => Component.Latching;
        set => Component.Latching = value;
    }

    public bool RaycastTarget {
        get => Component.RaycastTarget;
        set => Component.RaycastTarget = value;
    }

    public bool Active => Component.Active;

    public bool IsHovered => Component.IsHovered;

    public bool IsPressed => Component.IsPressed;

    public Action? OnClick {
        get => Component.OnClick;
        set => Component.OnClick = value;
    }

    public Action<bool>? OnStateChanged {
        get => Component.OnStateChanged;
        set => Component.OnStateChanged = value;
    }

    #endregion

    public AeroButton WrappedButton => Component;
}