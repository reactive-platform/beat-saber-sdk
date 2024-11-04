using JetBrains.Annotations;
using Reactive;
using Reactive.BeatSaber;
using Reactive.BeatSaber.Components;
using Reactive.Components;
using TMPro;
using UnityEngine;

namespace Reactive.BeatSaber.Components {
    [PublicAPI]
    public class Toggle : ReactiveComponent {
        #region Props

        public bool Active {
            get => _active;
            set => SetActive(value);
        }

        public bool Interactable {
            get => _interactable;
            set {
                _interactable = value;
                _backgroundButton.Interactable = value;
                NotifyPropertyChanged();
            }
        }

        public IColorSet KnobColors { get; set; } = defaultKnobColors;

        private static readonly StateColorSet defaultKnobColors = new() {
            States = {
                GraphicState.Active.WithColor(UIStyle.ControlButtonColorSet.ActiveColor),
                GraphicState.None.WithColor(UIStyle.ControlColorSet.Color),

                GraphicState.NonInteractable
                    .And(GraphicState.Active)
                    .WithColor(UIStyle.ControlButtonColorSet.ActiveColor.ColorWithAlpha(0.7f)),

                GraphicState.NonInteractable
                    .And(GraphicState.None)
                    .WithColor(UIStyle.ControlColorSet.Color.ColorWithAlpha(0.2f))
            }
        };

        private bool _active;
        private bool _interactable = true;

        public void SetActive(bool active, bool animated = true, bool silent = false) {
            _active = active;
            if (animated) {
                _progressValue.Value = active ? 1f : 0f;
            } else {
                _progressValue.SetValueImmediate(active ? 1f : 0f);
            }
            if (!silent) {
                NotifyPropertyChanged(nameof(Active));
            }
        }

        #endregion

        #region Setup

        private AnimatedValue<float> _progressValue = null!;

        private void UpdateAnimations(float progress) {
            LerpPosition(progress);
            LerpStretch(progress);
            LerpKnobColor(progress);
            LerpText(progress);
        }

        protected override void OnInitialize() {
            _progressValue = RememberAnimated(0f, 10.fact());
            this.AsFlexItem(size: new() { x = 18f, y = 6f });
            this.WithEffect(
                _progressValue,
                (_, y) => UpdateAnimations(y)
            );
        }

        #endregion

        #region Animation

        private float _knobMargin = 1f;
        private float _knobWidth = 7.55f;
        private float _knobHeight = 5f;
        private float _horizontalStretchAmount = 0.8f;
        private float _verticalStretchAmount = 0.8f;

        private void LerpText(float switchAmount) {
            _onLabel.Color = Color.Lerp(
                Color.clear,
                UIStyle.TextColorSet.Color,
                switchAmount
            );
            _offLabel.Color = Color.Lerp(
                Color.clear,
                UIStyle.TextColorSet.NotInteractableColor,
                1f - switchAmount
            );
        }

        private void LerpKnobColor(float switchAmount) {
            var state = GraphicState.None.AddIf(GraphicState.NonInteractable, !Interactable);
            var color = Color.Lerp(
                KnobColors.GetColor(state),
                KnobColors.GetColor(state.And(GraphicState.Active)),
                switchAmount
            );
            _knobImage.Color = color;
        }

        private void LerpPosition(float switchAmount) {
            _knobTransform.anchorMin = _knobTransform.anchorMin with { x = switchAmount };
            _knobTransform.anchorMax = _knobTransform.anchorMax with { x = switchAmount };
        }

        private void LerpStretch(float switchAmount) {
            var factor = 1f - Mathf.Abs(switchAmount - 0.5f) * 2f;
            var x = _knobWidth * (1f + _horizontalStretchAmount * factor);
            var y = _knobHeight * (_verticalStretchAmount * -factor) - _knobMargin;
            _knobTransform.sizeDelta = new(x, y);
        }

        #endregion

        #region Construct

        private Image _knobImage = null!;
        private RectTransform _knobTransform = null!;
        private Label _onLabel = null!;
        private Label _offLabel = null!;
        private ImageButton _backgroundButton = null!;

        protected override GameObject Construct() {
            return new ImageButton {
                Image = {
                    Sprite = BeatSaberResources.Sprites.background,
                    PixelsPerUnit = 12f,
                    Material = GameResources.UINoGlowMaterial
                },
                Colors = UIStyle.ControlButtonColorSet,
                OnClick = () => {
                    _active = !_active;
                    Active = _active;
                },
                Children = {
                    //text area
                    new Dummy {
                        Children = {
                            new Label {
                                Text = "I",
                                Alignment = TextAlignmentOptions.Center
                            }.AsFlexItem(size: new() { x = "50%" }).Bind(ref _onLabel),
                            //
                            new Label {
                                Text = "O",
                                Alignment = TextAlignmentOptions.Center
                            }.AsFlexItem(size: new() { x = "50%" }).Bind(ref _offLabel)
                        }
                    }.AsFlexGroup().WithRectExpand(),
                    //knob slide area
                    new Dummy {
                        Children = {
                            //knob
                            new Image {
                                ContentTransform = {
                                    anchorMin = Vector2.zero,
                                    anchorMax = new(0f, 1f),
                                },
                                Sprite = BeatSaberResources.Sprites.background,
                                PixelsPerUnit = 12f,
                                Color = Color.cyan
                            }.Bind(ref _knobTransform).Bind(ref _knobImage)
                        }
                    }.WithRectExpand().WithSizeDelta(-_knobWidth - _knobMargin, 0f)
                }
            }.Bind(ref _backgroundButton).Use();
        }

        #endregion
    }
}