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
            (_) => {
                Spinner.Enabled = false;
            });
        } else {
            Sprite = null;
            Spinner.Enabled = false;
        }
    }

    private Spinner _spinner = null!;
    public Spinner Spinner { 
        get => _spinner;
        private set => _spinner = value;   
    }

    protected override void Construct(RectTransform rect) {
        base.Construct(rect);

        new Spinner().Bind(ref _spinner).WithRectExpand().Use(rect);
    }

    protected override void OnInitialize() {
        base.OnInitialize();

        Sprite = BeatSaberResources.Sprites.transparentPixel;
    }
}