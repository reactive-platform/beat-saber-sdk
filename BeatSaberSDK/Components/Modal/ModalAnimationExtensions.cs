using System;
using System.Collections.Generic;
using System.Linq;
using HMUI;
using JetBrains.Annotations;
using Reactive.Components;
using UnityEngine;

namespace Reactive.BeatSaber.Components {
    [PublicAPI]
    public static class ModalAnimationExtensions {
        #region JumpAnimation

        public static T WithJumpAnimation<T>(this T wrapper, AnimationDuration? duration = null) where T : IComponentHolder<IModal> {
            var modal = wrapper.Component;
            var group = new Lazy<CanvasGroup>(() => modal.Content.GetOrAddComponent<CanvasGroup>());

            var modalScale = ValueUtils.AnimatedFloat(0f, duration.GetValueOrDefault(150.ms()));

            modal.OpenAnimation = AnimationUtils.Animation(
                () => modalScale.Value = 1f,
                [modalScale]
            );

            modal.CloseAnimation = AnimationUtils.Animation(
                () => modalScale.Value = 0f,
                [modalScale]
            );

            modalScale.OnStart = _ => {
                group.Evaluate();
            };
            
            modal.Animate(
                modalScale,
                (x, y) => {
                    group.Value.alpha = y;

                    EvaluateJumpCurve(y, out var xScale, out var yScale);
                    x.ContentTransform.localScale = new Vector3(xScale, yScale, 1f);
                }
            );

            return wrapper;
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
            public AlphaModalModule(IModal modal, Func<GameObject> objectAccessor, float targetAlpha, string? id) {
                _modal = modal;
                _objectAccessor = objectAccessor;
                _targetAlpha = targetAlpha;
                this.id = id;
            }

            public readonly string? id;

            private readonly IModal _modal;
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
        /// <param name="wrapper">A modal to animate with.</param>
        /// <param name="objectAccessor">An object accessor. Leave null to attach to the nearest ViewController.</param>
        /// <param name="id">An id of the animation. Use this if you need multiple animations and plan to manipulate them later.</param>
        /// <param name="targetAlpha">A target animation alpha.</param>
        /// <exception cref="InvalidOperationException">Throws when accessor is null and the object is not in a ViewController hierarchy.</exception>
        public static T WithAlphaAnimation<T>(this T wrapper, Func<GameObject>? objectAccessor = null, float targetAlpha = 0.2f, string? id = null) where T : IComponentHolder<IModal> {
            var modal = wrapper.Component;

            objectAccessor ??= () => {
                var controller = modal.Content.GetComponentInParent<ViewController>();

                if (controller == null) {
                    throw new InvalidOperationException("The component either must be in a ViewController hierarchy to use WithAlphaAnimation or provide a custom object accessor");
                }

                return controller.gameObject;
            };

            if (id != null && FindAlphaModule(modal, id) != null) {
                throw new InvalidOperationException("A module with the same id is already registered");
            }

            var module = new AlphaModalModule(modal, objectAccessor, targetAlpha, id);
            modal.BindModule(module);

            return wrapper;
        }

        /// <summary>
        /// Removes an alpha animation from the specified component.
        /// </summary>
        /// <param name="wrapper">A modal to remove the animation from.</param>
        /// <param name="id">An animation id (if present). Leave null to remove all animations of this kind.</param>
        public static T WithoutAlphaAnimation<T>(this T wrapper, string? id = null) where T : IComponentHolder<IModal> {
            var modal = wrapper.Component;

            if (id != null) {
                // Unbinding only a module with the specified id
                var module = FindAlphaModule(modal, id);

                if (module == null) {
                    throw new KeyNotFoundException($"A module with id {id} was not found");
                }

                modal.UnbindModule(module);
            } else {
                // Unbinding all modules of type
                var binder = modal as IReactiveModuleBinder;

                foreach (var module in binder.Modules.OfType<AlphaModalModule>()) {
                    binder.UnbindModule(module);
                }
            }

            return wrapper;
        }

        private static AlphaModalModule? FindAlphaModule(IReactiveModuleBinder binder, string id) {
            return binder.Modules
                .OfType<AlphaModalModule>()
                .FirstOrDefault(x => x.id == id);
        }

        #endregion
    }
}