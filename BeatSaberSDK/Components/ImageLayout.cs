using JetBrains.Annotations;
using UnityEngine;

namespace Reactive.BeatSaber.Components;

/// <summary>
/// A layout with an image. Leaf functions are not available here.
/// </summary>
[PublicAPI]
public class ImageLayout : Layout {
    public Image Image => _image;

    private Image _image = null!;
    
    protected override GameObject Construct() {
        // ExposeLayoutItem is called for the main component only,
        // so we shouldn't care about image implementing an ILeafLayoutItem
        return new Image().Bind(ref _image).Use();
    }
}