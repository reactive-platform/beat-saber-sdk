using System.Reflection;
using HarmonyLib;
using IPA;
using JetBrains.Annotations;

namespace BeatSaberSDK {
    [Plugin(RuntimeOptions.SingleStartInit)]
    [UsedImplicitly]
    public class Plugin {
        [Init, UsedImplicitly]
        public Plugin() { }

        private static Harmony _harmony = null!;
        
        [OnStart, UsedImplicitly]
        public void OnApplicationStart() {
            _harmony = new Harmony("Reactive.BeatSaberSDK");
            _harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        [OnExit, UsedImplicitly]
        public void OnApplicationQuit() {
            _harmony.UnpatchSelf();
        }
    }
}