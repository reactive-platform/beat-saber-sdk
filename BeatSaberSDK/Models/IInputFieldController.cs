using System;
using JetBrains.Annotations;

namespace Reactive.BeatSaber.Components {
    [PublicAPI]
    public interface IInputFieldController {
        bool CanProceed { get; }
        string Text { get; }

        event Action? TextClearedEvent;

        void Append(string text);
        void Truncate(int count);
        bool CanAppend(string text);
        bool CanTruncate(int count);
    }
}