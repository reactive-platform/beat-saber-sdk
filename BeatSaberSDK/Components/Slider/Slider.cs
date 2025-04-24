using System;
using HMUI;
using JetBrains.Annotations;
using Reactive.Components;
using Reactive.Yoga;
using UnityEngine;

namespace Reactive.BeatSaber.Components {
    [PublicAPI]
    public class Slider : SliderComponentBase {
        #region UI Props

        public bool ShowValueText {
            get => _text.Enabled;
            set => _text.Enabled = value;
        }

        public bool ShowButtons {
            get => _showButtons;
            set {
                _showButtons = value;
                _background.Image.Sprite = value ?
                    BeatSaberResources.Sprites.rectangle :
                    BeatSaberResources.Sprites.background;
                _decrementButton.Enabled = value;
                _incrementButton.Enabled = value;
            }
        }

        public bool Interactable {
            get => _interactable;
            set {
                _interactable = value;
                _decrementButton.Interactable = value;
                _incrementButton.Interactable = value;
                _background.Interactable = value;
                _pointerEventsHandler.enabled = value;
                RefreshHandleAndTextColors();
            }
        }

        public Func<float, string>? ValueFormatter {
            get => _valueFormatter;
            set {
                _valueFormatter = value;
                Refresh();
                NotifyPropertyChanged();
            }
        }

        private Func<float, string>? _valueFormatter;
        private bool _showButtons = true;
        private bool _interactable = true;

        #endregion

        #region Input

        private bool CanIncrement => Value < ValueRange.End;
        private bool CanDecrement => Value > ValueRange.Start;

        protected override void Refresh(bool silent = false, bool forceRefreshValue = false) {
            base.Refresh(silent, forceRefreshValue);
            RefreshButtons();
        }

        private void PlaceText(float handlePos) {
            _text.Text = _valueFormatter?.Invoke(Value) ?? $"{Value}";
            var halfPassed = handlePos > MaxHandlePosition / 2f;

            var measuredSize = _text.Measure(int.MaxValue, MeasureMode.AtMost, int.MaxValue, MeasureMode.AtMost);
            var textSize = measuredSize.x / 2f + 1f;
            var textPos = halfPassed ? handlePos - textSize : handlePos + textSize + _handle.rect.width;

            var text = _text.ContentTransform;
            text.localPosition = new(textPos, 0f, 0f);
            text.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, measuredSize.x);
            text.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, measuredSize.y);
        }

        protected override void PlaceHandle(float pos) {
            base.PlaceHandle(pos);
            PlaceText(pos);
        }

        private void RefreshButtons() {
            _incrementButton.Interactable = CanIncrement && _interactable;
            _decrementButton.Interactable = CanDecrement && _interactable;
        }

        #endregion

        #region Construct

        protected override PointerEventsHandler SlidingAreaEventsHandler => _pointerEventsHandler;
        protected override RectTransform SlidingAreaTransform => _slidingArea;
        protected override RectTransform HandleTransform => _handle;

        private RectTransform _slidingArea = null!;
        private RectTransform _handle = null!;
        private Image _handleImage = null!;
        private ImageButton _background = null!;
        private ButtonBase _incrementButton = null!;
        private ButtonBase _decrementButton = null!;
        private Label _text = null!;
        private PointerEventsHandler _pointerEventsHandler = null!;

        protected override GameObject Construct() {
            static BackgroundButton CreateButton(
                bool applyColor1,
                Sprite backgroundSprite,
                float iconRotation,
                Action callback
            ) {
                return new BackgroundButton {
                    Image = {
                        Sprite = backgroundSprite,
                        PixelsPerUnit = 12f,
                        GradientDirection = ImageView.GradientDirection.Horizontal,
                        Material = GameResources.UINoGlowMaterial
                    },
                    OnClick = callback,
                    Children = {
                        //icon
                        new Image {
                            Sprite = GameResources.ArrowIcon,
                            PreserveAspect = true,
                            Color = Color.white.ColorWithAlpha(0.8f),
                            ContentTransform = {
                                localEulerAngles = new(0f, 0f, iconRotation)
                            }
                        }.AsFlexItem(flexGrow: 1f)
                    }
                }.With(
                    x => {
                        var staticColor = BeatSaberStyle.InputColorSet.Color;
                        var animatedSet = new SimpleColorSet {
                            HoveredColor = Color.white.ColorWithAlpha(0.3f),
                            Color = BeatSaberStyle.InputColorSet.Color
                        };

                        var image = x.Image;
                        image.Color = Color.white;

                        if (!applyColor1) {
                            x.GradientColors0 = animatedSet;
                            image.GradientColor1 = staticColor;
                        } else {
                            x.GradientColors1 = animatedSet;
                            image.GradientColor0 = staticColor;
                        }
                    }
                ).AsFlexGroup(padding: 1.5f).AsFlexItem(basis: 6f);
            }

            return new Layout {
                Children = {
                    //dec button
                    CreateButton(
                        false,
                        BeatSaberResources.Sprites.backgroundLeft,
                        270f,
                        HandleDecrementButtonClicked
                    ).Bind(ref _decrementButton),
                    //sliding area bg
                    new BackgroundButton {
                        Image = {
                            Sprite = BeatSaberResources.Sprites.rectangle,
                            PixelsPerUnit = 12f,
                            Material = GameResources.UINoGlowMaterial
                        },
                        Colors = BeatSaberStyle.InputColorSet,
                        Children = {
                            //sliding area
                            new Layout {
                                ContentTransform = {
                                    pivot = new(0f, 0.5f)
                                },
                                Children = {
                                    //text
                                    new Label {
                                        ContentTransform = {
                                            anchorMin = new(0.5f, 0f),
                                            anchorMax = new(0.5f, 1f),
                                            sizeDelta = Vector2.zero
                                        }
                                    }.Bind(ref _text),
                                    //handle
                                    new Image {
                                        ContentTransform = {
                                            anchorMin = new(0.5f, 0f),
                                            anchorMax = new(0.5f, 1f),
                                            sizeDelta = new(1f, 0f),
                                            pivot = new(0f, 0.5f)
                                        },
                                        Sprite = BeatSaberResources.Sprites.background,
                                        PixelsPerUnit = 30f
                                    }.Bind(ref _handle).Bind(ref _handleImage)
                                }
                            }.AsFlexItem(flexGrow: 1f).Bind(ref _slidingArea),
                        }
                    }.WithNativeComponent(out _pointerEventsHandler).AsFlexGroup(
                        padding: 1f
                    ).AsFlexItem(
                        flexGrow: 1f,
                        margin: new() { left = 0.5f, right = 0.5f }
                    ).Bind(ref _background),
                    //inc button
                    CreateButton(
                        true,
                        BeatSaberResources.Sprites.backgroundRight,
                        90f,
                        HandleIncrementButtonClicked
                    ).Bind(ref _incrementButton)
                }
            }.AsFlexGroup().Use();
        }

        protected override void OnInitialize() {
            base.OnInitialize();
            RefreshHandleAndTextColors();
        }

        private void RefreshHandleAndTextColors() {
            _handleImage.Color = Color.white.ColorWithAlpha(_interactable ? 0.8f : 0.3f);
            _text.Color = Color.white.ColorWithAlpha(_interactable ? 1f : 0.5f);
        }

        #endregion

        #region Callbacks

        private void HandleIncrementButtonClicked() {
            Value += ValueStep;
        }

        private void HandleDecrementButtonClicked() {
            Value -= ValueStep;
        }

        #endregion
    }
}