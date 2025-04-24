using System;
using JetBrains.Annotations;
using Reactive.Components;
using Reactive.Yoga;
using UnityEngine;

namespace Reactive.BeatSaber.Components {
    /// <summary>
    /// Scrollbar for ReactiveComponent lists
    /// </summary>
    [PublicAPI]
    public class Scrollbar : ReactiveComponent, IScrollbar {
        #region Impl

        float IScrollbar.PageHeight {
            set {
                _normalizedPageHeight = Mathf.Clamp01(value);
                RefreshHandle();
            }
        }

        float IScrollbar.Progress {
            set {
                _progress = Mathf.Clamp01(value);
                RefreshHandle();
            }
        }

        bool IScrollbar.CanScrollUp {
            set => _upButton.Interactable = value;
        }

        bool IScrollbar.CanScrollDown {
            set => _downButton.Interactable = value;
        }

        public event Action? ScrollBackwardButtonPressedEvent;
        public event Action? ScrollForwardButtonPressedEvent;

        void IScrollbar.SetActive(bool active) {
            Enabled = active;
        }

        #endregion

        #region Handle

        private float _padding = 0.25f;
        private float _progress;
        private float _normalizedPageHeight = 1f;

        private void RefreshHandle() {
            var num = _handleContainerRect.rect.size.y - 2f * _padding;
            _handleRect.sizeDelta = new Vector2(0f, _normalizedPageHeight * num);
            _handleRect.anchoredPosition = new Vector2(0f, -_progress * (1f - _normalizedPageHeight) * num - _padding);
        }

        #endregion

        #region Construct

        private RectTransform _handleContainerRect = null!;
        private RectTransform _handleRect = null!;
        private ButtonBase _upButton = null!;
        private ButtonBase _downButton = null!;

        protected override GameObject Construct() {
            static BackgroundButton CreateButton(float rotation, Action callback) {
                return new BackgroundButton {
                    Image = {
                        Sprite = BeatSaberResources.Sprites.transparentPixel,
                        Material = null
                    },
                    OnClick = callback,

                    Children = {
                        new Background {
                            ContentTransform = {
                                localEulerAngles = new(0f, 0f, rotation)
                            },

                            Sprite = GameResources.ArrowIcon,
                            PreserveAspect = true,
                            Material = GameResources.UINoGlowMaterial
                        }.Export(out Image image).AsFlexItem(size: 4f)
                    }
                }.AsFlexItem(flexGrow: 1f).Export(out ColoredButton button).With(
                    y => {
                        y.WrappedButton.WithListener(
                            x => x.Interactable,
                            _ => RefreshImage()
                        ).WithListener(
                            x => x.IsHovered,
                            _ => RefreshImage()
                        );
                    }
                );

                void RefreshImage() {
                    var hovered = button.IsHovered;
                    image.ContentTransform.localScale = hovered ? Vector3.one * 1.2f : Vector3.one;
                    image.Color = button.Interactable ?
                        hovered ? Color.white : Color.white.ColorWithAlpha(0.5f) :
                        Color.black.ColorWithAlpha(0.5f);
                }
            }

            return new Layout {
                Children = {
                    //handle container
                    new Background {
                        Sprite = BeatSaberResources.Sprites.background,
                        PixelsPerUnit = 20f,
                        Color = Color.black.ColorWithAlpha(0.5f),

                        Children = {
                            //handle
                            new Image {
                                ContentTransform = {
                                    anchorMin = new(0f, 1f),
                                    anchorMax = new(1f, 1f),
                                    pivot = new(0.5f, 1f)
                                },
                                Sprite = GameResources.VerticalIndicatorIcon,
                                Color = Color.white.ColorWithAlpha(0.5f),
                                ImageType = UnityEngine.UI.Image.Type.Sliced
                            }.Bind(ref _handleRect)
                        }
                    }.AsFlexItem(
                        flexGrow: 1f,
                        margin: new() { left = "15%", right = "15%", top = 4f, bottom = 4f }
                    ).Bind(ref _handleContainerRect),
                    //
                    new Layout {
                        Children = {
                            //up button
                            CreateButton(180f, HandleUpButtonClicked)
                                .AsFlexGroup(alignItems: Align.FlexStart)
                                .Bind(ref _upButton),
                            //down button
                            CreateButton(0f, HandleDownButtonClicked)
                                .AsFlexGroup(alignItems: Align.FlexEnd)
                                .Bind(ref _downButton)
                        }
                    }.AsFlexGroup(direction: FlexDirection.Column).WithRectExpand()
                }
            }.AsFlexGroup(FlexDirection.Column).Use();
        }

        protected override void OnInitialize() {
            this.AsFlexItem(size: new() { x = 2f });
            WithinLayoutIfDisabled = true;
            RefreshHandle();
        }

        #endregion

        #region Callbacks

        private void HandleUpButtonClicked() {
            ScrollBackwardButtonPressedEvent?.Invoke();
        }

        private void HandleDownButtonClicked() {
            ScrollForwardButtonPressedEvent?.Invoke();
        }

        #endregion
    }
}