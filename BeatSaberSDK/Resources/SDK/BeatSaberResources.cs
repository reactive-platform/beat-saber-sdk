using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

#nullable disable

namespace Reactive.BeatSaber {
    [PublicAPI]
    [CreateAssetMenu(fileName = "BeatSaberSDK_Resources", menuName = "Reactive/BeatSaberSDK/Resources")]
    public class BeatSaberResources : ScriptableObject {
        #region Initialization

        static BeatSaberResources() {
            Init();
        }

        private static BeatSaberResources _instance;

#if COMPILE_EDITOR
    private static void Init() {
        if (_instance != null) return;
        var bundles = AssetDatabase.FindAssets("BeatSaberSDK_Resources");
        var guid = bundles.First();
        var path = AssetDatabase.GUIDToAssetPath(guid);
        _instance = AssetDatabase.LoadAssetAtPath<BeatSaberResources>(path);
    }
#else
        private const string BundlePath = "asset_bundle";

        internal static void Init() {
            if (_instance != null) return;
            var assembly = Assembly.GetExecutingAssembly();
            var path = assembly.GetManifestResourceNames().First(x => x.EndsWith(BundlePath));

            using var stream = assembly.GetManifestResourceStream(path);
            var localAssetBundle = AssetBundle.LoadFromStream(stream);

            if (localAssetBundle == null) {
                throw new Exception("AssetBundle has failed to load");
            }

            _instance = localAssetBundle.LoadAsset<BeatSaberResources>("BeatSaberSDK_Resources");
            localAssetBundle.Unload(false);
        }
#endif

        #endregion

        #region Serialized

        public SpriteCollection sprites;
        public MaterialCollection materials;

        #endregion

        #region Static

        public static SpriteCollection Sprites => _instance!.sprites;

        public static MaterialCollection Materials => _instance!.materials;

        #endregion
    }
}