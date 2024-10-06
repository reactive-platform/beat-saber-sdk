using System.Linq;
using Reactive.Components;
using UnityEngine;
using UnityEngine.EventSystems;

#if !COMPILE_EDITOR

namespace Reactive.BeatSaber.Components {
    [RequireComponent(typeof(PointerEventsHandler))]
    internal class VRScrollAdapter : MonoBehaviour {
        private PointerEventsHandler _pointerEventsHandler = null!;
        private IVRPlatformHelper _platformHelper = null!;

        private void Awake() {
            _pointerEventsHandler = GetComponent<PointerEventsHandler>();
            var context = FindObjectsByType<Zenject.Context>(FindObjectsSortMode.InstanceID).First();
            _platformHelper = context.Container.Resolve<IVRPlatformHelper>();
        }

        private void Update() {
            if (!_pointerEventsHandler.IsHovered) return;
            var delta = _platformHelper.GetAnyJoystickMaxAxis();
            var pointerEventData = new PointerEventData(EventSystem.current) {
                scrollDelta = delta
            };
            _pointerEventsHandler.OnScroll(pointerEventData);
        }
    }
}

#endif