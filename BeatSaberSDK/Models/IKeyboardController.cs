using System;
using JetBrains.Annotations;

namespace Reactive.BeatSaber.Components {
    [PublicAPI]
    public interface IKeyboardController<in T> where T : IInputFieldController {
        event Action? KeyboardClosedEvent;

        void Setup(T? input);
        void SetActive(bool active);
        void Refresh();
    }
}