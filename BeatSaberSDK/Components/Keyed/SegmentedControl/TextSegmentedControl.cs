
using JetBrains.Annotations;
using Reactive.Components;

namespace Reactive.BeatSaber.Components {
    [PublicAPI]
    public class TextSegmentedControl<TKey> : SegmentedControl<TKey, string, TextKeyedControlCell<TKey>> { }
}