using JetBrains.Annotations;

namespace Reactive.BeatSaber.Components;

/// <summary>
/// Beat Saber styled button with a label.
/// </summary>
[PublicAPI]
public class BsButton : BsButtonBase {
    public Label Label => _label;

    private Label _label = null!;
    
    protected override IReactiveComponent ConstructContent() {
        return new Label().Bind(ref _label);
    }
}