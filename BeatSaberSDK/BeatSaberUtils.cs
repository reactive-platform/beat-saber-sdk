using System;
using System.Linq;
using JetBrains.Annotations;

namespace Reactive.BeatSaber;

[PublicAPI]
public static class BeatSaberUtils {
    public static bool UsesFPFC {
        get {
            _usesFpfc ??= Environment.GetCommandLineArgs().Contains("fpfc");
            return _usesFpfc.Value;
        }
    }

    private static bool? _usesFpfc;
}