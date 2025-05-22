using System;
using JetBrains.Annotations;
using Reactive.Components;
using UnityEngine;

namespace Reactive.BeatSaber.Components {
    [PublicAPI]
    public class IconSegmentedControl<TKey> : SegmentedControl<TKey, Sprite, IconKeyedControlCell<TKey>> { }

    [PublicAPI]
    public class IconKeyedControlCell<TKey> : ImageButton, IKeyedControlCell<TKey, Sprite> {
        public TKey Key => _key ?? throw new UninitializedComponentException();
        
        public event Action<TKey>? CellAskedToBeSelectedEvent;

        private TKey? _key;
        private bool _notify;
        
        public void Init(TKey key, Sprite param) {
            Image.Sprite = param;
            _key = key;
        }

        public void OnCellStateChange(bool selected) {
            _notify = false;
            Click(selected);
        }

        protected override void OnInitialize() {
            Latching = true;
            Colors = new SimpleColorSet {
                ActiveColor = BeatSaberStyle.PrimaryButtonColor,
                HoveredColor = BeatSaberStyle.PrimaryButtonColor,
                Color = (Color.white * 0.8f).ColorWithAlpha(0.2f)
            };

            Image.PreserveAspect = true;
            Image.Material = BeatSaberResources.Materials.uiAdditiveGlowMaterial;
        }

        protected override void OnButtonStateChange() {
            base.OnButtonStateChange();
            if (Active && _notify) {
                CellAskedToBeSelectedEvent?.Invoke(Key);
            }
            _notify = true;
        }
    }
}