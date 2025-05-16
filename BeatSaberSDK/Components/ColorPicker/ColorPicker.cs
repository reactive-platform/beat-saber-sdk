using JetBrains.Annotations;
using Reactive.Components;
using Reactive.Yoga;
using UnityEngine;

namespace Reactive.BeatSaber.Components {
    /// <summary>
    /// A color picker component.
    /// </summary>
    [PublicAPI]
    public class ColorPicker : ReactiveComponent, IComponentHolder<IModal> {
        #region UI Props

        public Color Color {
            get => _color;
            set {
                _color = value;
                _colorSampleImage.Color = value;

                if (_modalOpened) {
                    _circleModal.Modal.ColorCircle.SetColor(value, false);
                }

                NotifyPropertyChanged();
            }
        }

        public RelativePlacement CirclePlacement { get; set; } = RelativePlacement.Center;

        private Color _color;
        private bool _modalOpened;

        #endregion

        #region Construct

        IModal IComponentHolder<IModal>.Component => _circleModal;

        private SharedModal<ColorCircleModal> _circleModal = null!;
        private Image _colorSampleImage = null!;

        protected override GameObject Construct() {
            return new AeroButtonLayout {
                Children = {
                    // Icon
                    new Image {
                        Sprite = GameResources.EditIcon,
                        PreserveAspect = true,
                        Skew = BeatSaberStyle.Skew,
                        Color = BeatSaberStyle.SecondaryTextColor
                    }.AsFlexItem(
                        size: new() { x = 4f, y = "auto" }
                    ),

                    // Color sample
                    new Image {
                        Sprite = GameResources.CircleIcon,
                        PreserveAspect = true
                    }.AsFlexItem(
                        size: new() { x = 4f, y = "auto" }
                    ).Bind(ref _colorSampleImage),

                    // Color circle
                    new SharedModal<ColorCircleModal>()
                        .WithAnchor(
                            this,
                            Lazy(() => CirclePlacement, false),
                            unbindOnceOpened: false
                        )
                        .WithOpenListener(HandleModalOpened)
                        .WithCloseListener(HandleModalClosed)
                        .Bind(ref _circleModal)
                }
            }.AsFlexGroup(
                justifyContent: Justify.FlexStart,
                padding: new() { left = 2f, top = 1f, right = 2f, bottom = 1f },
                gap: 1f
            ).WithModal(_circleModal).Use();
        }

        #endregion

        #region Callbacks

        private void HandleModalOpened(IModal modal, bool finished) {
            if (finished) {
                return;
            }

            _modalOpened = true;
            _circleModal.Modal.WithListener(
                x => x.ColorCircle.SavedColor,
                HandleColorChanged
            );
            Color = _color;
        }

        private void HandleModalClosed(IModal modal, bool finished) {
            if (finished) {
                return;
            }

            _modalOpened = false;
            _circleModal.Modal.WithoutListener(
                x => x.ColorCircle.SavedColor,
                HandleColorChanged
            );
        }

        private void HandleColorChanged(Color color) {
            _colorSampleImage.Color = color;
            _color = color;
            NotifyPropertyChanged(nameof(Color));
        }

        #endregion
    }
}