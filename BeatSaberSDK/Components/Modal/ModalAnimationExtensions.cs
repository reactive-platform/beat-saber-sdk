using System;
using HMUI;
using JetBrains.Annotations;
using Reactive.Components;
using UnityEngine;

namespace Reactive.BeatSaber.Components {
    [PublicAPI]
    public static class ModalAnimationExtensions {
        #region JumpAnimation

        public static T WithJumpAnimation<T>(this T modal, AnimationDuration? duration = null) where T : ModalBase {
            var group = modal.Content.GetOrAddComponent<CanvasGroup>();

            var modalScale = ValueUtils.AnimatedFloat(0f, duration.GetValueOrDefault(150.ms()));

            modal.OpenAnimation = AnimationUtils.Animation(
                () => modalScale.Value = 1f,
                [modalScale]
            );

            modal.CloseAnimation = AnimationUtils.Animation(
                () => modalScale.Value = 0f,
                [modalScale]
            );

            modal.Animate(
                modalScale,
                (x, y) => {
                    group.alpha = y;

                    EvaluateJumpCurve(y, out var xScale, out var yScale);
                    x.ContentTransform.localScale = new Vector3(xScale, yScale, 1f);
                }
            );

            return modal;
        }

        // Made based on the base-game curve
        private static void EvaluateJumpCurve(float t, out float x, out float y) {
            x = t <= 0.3f ?
                Mathf.Lerp(0.85f, 1.065f, t / 0.3f) :
                Mathf.Lerp(1.065f, 1f, (t - 0.3f) / 0.7f);

            y = t <= 0.47f ?
                Mathf.Lerp(0f, 0.95f, t / 0.47f) :
                Mathf.Lerp(0.95f, 1f, (t - 0.47f) / 0.53f);
        }

        #endregion

        #region AlphaAnimation

        private class AlphaModalModule : IReactiveModule {
            public AlphaModalModule(ModalBase modal, Func<GameObject> objectAccessor, float targetAlpha) {
                _modal = modal;
                _objectAccessor = objectAccessor;
                _targetAlpha = targetAlpha;
            }

            private readonly ModalBase _modal;
            private readonly Func<GameObject> _objectAccessor;
            private readonly float _targetAlpha;

            private CanvasGroup? _group;
            private IAnimation? _animator;
            private bool _reverse;

            public void OnUpdate() {
                if (_animator == null) {
                    return;
                }

                var from = _reverse ? _targetAlpha : 1f;
                var to = _reverse ? 1f : _targetAlpha;

                _group!.alpha = Mathf.Lerp(from, to, _animator.Progress);
            }

            public void OnBind() {
                _modal.ModalClosedEvent += HandleModalClosed;
                _modal.ModalOpenedEvent += HandleModalOpened;
            }

            public void OnUnbind() {
                _modal.ModalClosedEvent -= HandleModalClosed;
                _modal.ModalOpenedEvent -= HandleModalOpened;
            }

            private void HandleModalOpened(IModal _, bool finished) {
                if (!finished) {
                    if (_group == null) {
                        _group = _objectAccessor().GetOrAddComponent<CanvasGroup>();
                    }
                    _group.ignoreParentGroups = true;
                    _animator = _modal.OpenAnimation;
                    _reverse = false;
                } else {
                    _animator = null;
                    _group!.alpha = _targetAlpha;
                }
            }

            private void HandleModalClosed(IModal _, bool finished) {
                if (!finished) {
                    _reverse = true;
                    _animator = _modal.CloseAnimation;
                } else {
                    _animator = null;
                    _group!.alpha = 1f;
                    _group!.ignoreParentGroups = false;
                }
            }
        }

        /// <summary>
        /// Adds an alpha animation to the specified object.
        /// </summary>
        /// <param name="modal">A modal to animate with.</param>
        /// <param name="objectAccessor">An object accessor. Leave null to attach to the nearest ViewController.</param>
        /// <param name="targetAlpha">A target animation alpha.</param>
        /// <exception cref="InvalidOperationException">Throws when accessor is null and the object is not in a ViewController hierarchy.</exception>
        public static T WithAlphaAnimation<T>(this T modal, Func<GameObject>? objectAccessor = null, float targetAlpha = 0.2f) where T : ModalBase {
            objectAccessor ??= () => {
                var controller = modal.Content.GetComponentInParent<ViewController>();

                if (controller == null) {
                    throw new InvalidOperationException("The component either must be in a ViewController hierarchy to use WithAlphaAnimation or provide a custom object accessor");
                }

                return controller.gameObject;
            };

            var module = new AlphaModalModule(modal, objectAccessor, targetAlpha);
            modal.BindModule(module);

            return modal;
        }

        #endregion
    }
}