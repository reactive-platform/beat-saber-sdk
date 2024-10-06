using JetBrains.Annotations;
using UnityEngine;

namespace Reactive.BeatSaber.Components {
    [PublicAPI]
    public class Spinner : ReactiveComponent {
        #region Construct

        public Image Image => _image;

        private Image _image = null!;

        protected override GameObject Construct() {
            return new Image {
                PreserveAspect = true,
                Sprite = BeatSaberResources.Sprites.spinnerIcon
            }.Bind(ref _image).Use();
        }

        #endregion

        #region Spinner

        public float RotationSpeed = 50f;

        protected override void OnUpdate() {
            ContentTransform.localEulerAngles -= new Vector3(0f, 0f, Time.deltaTime * RotationSpeed * 10f);
        }

        #endregion
    }
}