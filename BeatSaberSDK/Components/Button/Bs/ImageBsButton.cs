using System.Collections.Generic;
using JetBrains.Annotations;
using Reactive.Components;
using UnityEngine;

namespace Reactive.BeatSaber.Components;

/// <summary>
/// Beat Saber styled button with an image.
/// </summary>
[PublicAPI]
public class ImageBsButton : BsButtonBase, IComponentHolder<Image> {
    #region Adapter

    public Sprite? Sprite {
        get => _image.Sprite;
        set => _image.Sprite = value;
    }

    public Material? Material {
        get => _image.Material;
        set => _image.Material = value;
    }

    public bool PreserveAspect {
        get => _image.PreserveAspect;
        set => _image.PreserveAspect = value;
    }

    public UnityEngine.UI.Image.Type ImageType {
        get => _image.ImageType;
        set => _image.ImageType = value;
    }

    public UnityEngine.UI.Image.FillMethod FillMethod {
        get => _image.FillMethod;
        set => _image.FillMethod = value;
    }

    public float FillAmount {
        get => _image.FillAmount;
        set => _image.FillAmount = value;
    }

    public float PixelsPerUnit {
        get => _image.PixelsPerUnit;
        set => _image.PixelsPerUnit = value;
    }

    #endregion

    #region Setup

    Image IComponentHolder<Image>.Component => _image;

    private Image _image = null!;

    protected override IEnumerable<IReactiveComponent> ConstructContent() {
        return [
            new Image {
                    PreserveAspect = true,
                    Skew = BeatSaberStyle.Skew
                }
                .AsFlexItem(size: "auto")
                .Bind(ref _image)
        ];
    }

    protected override void OnSkewChanged(float skew) {
        _image.Skew = skew;
    }

    protected override void OnGraphicStateChanged() {
        var alpha = GraphicState.IsInteractable() ?
            GraphicState.IsHovered() ? 1 : 0.75f
            : 0.25f;

        _image.Color = Color.white.ColorWithAlpha(alpha);
    }

    #endregion
}