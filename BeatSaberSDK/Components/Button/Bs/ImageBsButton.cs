using JetBrains.Annotations;

namespace Reactive.BeatSaber.Components;

/// <summary>
/// Beat Saber styled button with an image.
/// </summary>
[PublicAPI]
public class ImageBsButton : BsButtonBase {
    public Image Image => _image;

    private Image _image = null!;
    
    protected override IReactiveComponent ConstructContent() {
        return new Image().Bind(ref _image);
    }
}