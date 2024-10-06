using JetBrains.Annotations;

namespace Reactive.BeatSaber.Components;

[PublicAPI]
public interface ISkewedComponent {
    float Skew { get; set; }
}