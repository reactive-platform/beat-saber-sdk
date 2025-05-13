using JetBrains.Annotations;
using Reactive.Components;
using Reactive.Yoga;
using UnityEngine;

namespace Reactive.BeatSaber.Components {
    /// <summary>
    /// A color selector wrapped in a modal.
    /// </summary>
    [PublicAPI]
    public class ColorCircleModal : ModalBase, IComponentHolder<ColorCircle> {
        #region UI Props

        public string Title {
            get => _dialogHeader.Text;
            set => _dialogHeader.Text = value;
        }

        public ColorCircle ColorCircle => _colorCircle;

        ColorCircle IComponentHolder<ColorCircle>.Component => ColorCircle;

        #endregion

        #region Construct

        private DialogHeader _dialogHeader = null!;
        private ColorCircle _colorCircle = null!;

        protected override GameObject Construct() {
            _colorCircle = new();

            return new Background {
                Children = {
                    new DialogHeader {
                            Text = "Select Color"
                        }
                        .AsFlexItem(basis: 6f)
                        .Bind(ref _dialogHeader),

                    // Content
                    ColorCircle.AsFlexItem(size: 54f)
                }
            }.AsBlurBackground().AsFlexGroup(
                direction: FlexDirection.Column,
                constrainHorizontal: false,
                constrainVertical: false
            ).Use();
        }

        protected override void OnInitialize() {
            RoutePropertyChanged(ColorCircle, nameof(ColorCircle));
            this.WithJumpAnimation();
            base.OnInitialize();
        }

        #endregion
    }
}