using System;
using JetBrains.Annotations;
using Reactive.Components;
using UnityEngine;

namespace Reactive.BeatSaber.Components {
    [PublicAPI]
    public static class ModalAnimationExtensions {
        #region JumpAnimation

        public static T WithJumpAnimation<T>(this T modal, AnimationDuration? duration = null) where T : ModalBase {
            var group = modal.Content.GetOrAddComponent<CanvasGroup>();
            var dur = duration.GetValueOrDefault(150.ms());
            
            modal.OpenAnimator = ValueUtils.Animate<ModalBase>(
                modal,
                (x, y) => {
                    Animate(x, y);
                    group.alpha = y;
                },
                dur
            );
            modal.CloseAnimator = ValueUtils.Animate<ModalBase>(
                modal,
                (x, y) => {
                    Animate(x, 1f - y);
                    group.alpha = 1f - y;
                },
                dur
            );
            return modal;

            static void Animate(ModalBase x, float t) {
                // X scale curve
                var xScale = t <= 0.3f ?
                    Mathf.Lerp(0.85f, 1.065f, t / 0.3f) :      // Linear interpolation from 0.85 to 1.065
                    Mathf.Lerp(1.065f, 1f, (t - 0.3f) / 0.7f); // Linear interpolation from 1.065 to 1

                // Y scale curve
                var yScale = t <= 0.47f ?
                    Mathf.Lerp(0f, 0.95f, t / 0.47f) :          // Linear interpolation from 0 to 0.95
                    Mathf.Lerp(0.95f, 1f, (t - 0.47f) / 0.53f); // Linear interpolation from 0.95 to 1

                x.ContentTransform.localScale = new Vector3(xScale, yScale, 1f);
            }
        }

        #endregion

        #region AlphaAnimation

        private class AlphaModalModule : IReactiveModule {
            public AlphaModalModule(ModalBase modal, Func<GameObject> objectAccessor, float targetAlpha) {
                _modal = modal;
                _objectAccessor = objectAccessor;
                _targetAlpha = targetAlpha;
                _modal.ModalClosedEvent += HandleModalClosed;
                _modal.ModalOpenedEvent += HandleModalOpened;
            }

            private readonly ModalBase _modal;
            private readonly Func<GameObject> _objectAccessor;
            private readonly float _targetAlpha;

            private CanvasGroup? _group;
            private IObjectAnimator<ModalBase>? _animator;
            private bool _reverse;

            public void OnUpdate() {
                if (_animator == null) return;
                _group!.alpha = Mathf.Lerp(
                    _reverse ? _targetAlpha : 1f,
                    _reverse ? 1f : _targetAlpha,
                    _animator.Progress
                );
            }

            public void OnDestroy() {
                _modal.ModalClosedEvent -= HandleModalClosed;
                _modal.ModalOpenedEvent -= HandleModalOpened;
            }

            private void HandleModalOpened(IModal _, bool finished) {
                if (!finished) {
                    if (_group == null) {
                        _group = _objectAccessor().GetOrAddComponent<CanvasGroup>();
                    }
                    _group.ignoreParentGroups = true;
                    _animator = _modal.OpenAnimator;
                    _reverse = false;
                } else {
                    _animator = null;
                    _group!.alpha = _targetAlpha;
                }
            }

            private void HandleModalClosed(IModal _, bool finished) {
                if (!finished) {
                    _reverse = true;
                    _animator = _modal.CloseAnimator;
                } else {
                    _animator = null;
                    _group!.alpha = 1f;
                    _group!.ignoreParentGroups = false;
                }
            }
        }

        public static T WithAlphaAnimation<T>(
            this T modal,
            Func<GameObject> objectAccessor,
            float targetAlpha = 0.2f
        ) where T : ModalBase {
            var module = new AlphaModalModule(modal, objectAccessor, targetAlpha);
            modal.BindModule(module);
            return modal;
        }

        #endregion
    }
}