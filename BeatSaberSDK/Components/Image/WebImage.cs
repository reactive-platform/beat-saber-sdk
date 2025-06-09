using JetBrains.Annotations;
using Reactive.Components;
using UnityEngine;

namespace Reactive.BeatSaber.Components;

[PublicAPI]
public class WebImage : Image {
    private string? _src = null;

    public string? Src {
        get => _src;
        set {
            if (value != _src) {
                _src = value;
                UpdateImage();
            }
        }
    }

    private void UpdateImage() {
        if (_src != null) {
            this.WithWebSource(_src, () => {
                Spinner.Enabled = true;
            },
            (bool loaded) => {
                Spinner.Enabled = false;
            });
        } else {
            Sprite = null;
            Spinner.Enabled = false;
        }
    }

    public Spinner Spinner = null!;

    protected override void Construct(RectTransform rect) {
        base.Construct(rect);

        Sprite = BeatSaberResources.Sprites.transparentPixel;

        new Spinner().Bind(ref Spinner).WithRectExpand().Use(rect);
    }
}