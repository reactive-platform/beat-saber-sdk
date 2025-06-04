using System.Collections;
using System.Threading;
using BeatSaberSDK;
using JetBrains.Annotations;
using Reactive.Yoga;
using UnityEngine;

namespace Reactive.BeatSaber.Components;

[PublicAPI]
public class WebImage : Image {
    private string? _src = null;

    public string? Src {
        get => _src;
        set {
            _src = value;
            UpdateImage();
        }
    }

    private CancellationTokenSource? _tokenSource = null;
    private AnimatedImage? _animatedImage = null;

    private void UpdateImage() {
        Spinner.Enabled = true;
        StopAllCoroutines();

        if (_src != null) {
            _tokenSource?.Cancel();
            _tokenSource = new CancellationTokenSource();
            StartCoroutine(LoadImage());
        } else {
            Sprite = null;
            Spinner.Enabled = false;
        }
    }

    protected override void OnUpdate() {
        if (_animatedImage != null) {
            _animatedImage.OnUpdate(Time.deltaTime);
        }
    }

    private IEnumerator LoadImage() {
        var loadTask = ImageUtils.TryDownload(_src, OnImageLoadSuccess, OnImageLoadFailed, _tokenSource.Token);
        yield return loadTask;
    }

    private void OnImageLoadSuccess(AnimatedImage image) {
        Sprite = image.Sprite;
        Spinner.Enabled = false;
        _animatedImage = image;
    }

    private void OnImageLoadFailed(string reason) {
        Sprite = null;
        _animatedImage = null;
    }

    public Spinner Spinner = null!;

    protected override void Construct(RectTransform rect) {
        base.Construct(rect);

        new Spinner().Bind(ref Spinner).WithRectExpand().Use(rect);
    }
}