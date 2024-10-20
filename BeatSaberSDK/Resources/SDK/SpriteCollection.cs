using UnityEngine;

#nullable disable

namespace Reactive.BeatSaber {
    [CreateAssetMenu(fileName = "SpriteCollection", menuName = "Reactive/BeatSaberSDK/SpriteCollection")]
    public class SpriteCollection : ScriptableObject {
        [Space] [Header("Backgrounds")]
        public Sprite rectangle;
        public Sprite background;
        public Sprite backgroundLeft;
        public Sprite backgroundRight;
        public Sprite backgroundBottom;
        public Sprite backgroundTop;
        public Sprite backgroundUnderline;
        /* Icons */
        [Space] [Header("Icons")]
        public Sprite spinnerIcon;
        public Sprite crossIcon;
        /* Other */
        [Space] [Header("Misc")]
        public Sprite glare;
        public Sprite frame;
        public Sprite transparentPixel;
    }
}