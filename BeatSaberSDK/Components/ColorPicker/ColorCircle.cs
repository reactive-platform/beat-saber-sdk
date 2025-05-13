using System;
using System.Linq;
using HMUI;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Reactive.BeatSaber.Components {
    /// <summary>
    /// A color selector component.
    /// </summary>
    [PublicAPI]
    public class ColorCircle : ReactiveComponent {
        #region Color

        /// <summary>
        /// Represents a color related to the current handle position. Updates each time the handle is moved.
        /// </summary>
        public Color Color {
            get => _color;
            private set {
                _color = value;

                OnColorUpdated?.Invoke(value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Represents a color after releasing the handle. Updates only once you release the handle.
        /// </summary>
        public Color SavedColor {
            get => _savedColor;
            private set {
                _savedColor = value;

                OnColorSaved?.Invoke(value);
                NotifyPropertyChanged();
            }
        }

        public Action<Color>? OnColorSaved { get; set; }
        public Action<Color>? OnColorUpdated { get; set; }

        private Color _color;
        private Color _savedColor;

        public void SetColor(Color color, bool notifyListeners = true) {
#if !COMPILE_EDITOR
            _colorPicker.color = color;
            _pickerButton.SetColor(color);
#endif

            if (notifyListeners) {
                Color = color;
                SavedColor = color;
            }
        }

        #endregion

        #region Construct

#if !COMPILE_EDITOR
        private HSVPanelController _colorPicker = null!;
        private ColorPickerButtonController _pickerButton = null!;

        private static HSVPanelController? _hsvPanelController;

        protected override GameObject Construct() {
            _colorPicker = InstantiateColorPicker();
            _colorPicker.colorDidChangeEvent += HandleColorChanged;

            _pickerButton = _colorPicker.GetComponentInChildren<ColorPickerButtonController>();
            _pickerButton.GetComponent<Touchable>().enabled = false;
            _pickerButton.SetColor(_colorPicker.color);

            return _colorPicker.gameObject;
        }

        private static HSVPanelController InstantiateColorPicker() {
            if (_hsvPanelController == null) {
                _hsvPanelController = Resources.FindObjectsOfTypeAll<HSVPanelController>()
                    .First(x => x.GetComponentInChildren<ColorPickerButtonController>() != null);
            }

            return Object.Instantiate(_hsvPanelController);
        }
#else
        protected override GameObject Construct() {
            return new Label {
                Text = "Ooops, not available in editor :(",
                EnableWrapping = true
            }.Use();
        }
#endif

        protected override void OnInitialize() {
            this.AsFlexItem(size: 54f);
        }

        #endregion

        #region Callbacks

        private void HandleColorChanged(Color color, ColorChangeUIEventType type) {
            Color = color;

#if !COMPILE_EDITOR
            _pickerButton.SetColor(color);
#endif

            if (type is ColorChangeUIEventType.PointerUp) {
                SavedColor = color;
            }
        }

        #endregion
    }
}