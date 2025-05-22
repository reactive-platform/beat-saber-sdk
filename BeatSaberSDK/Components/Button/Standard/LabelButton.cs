using System;
using HMUI;
using JetBrains.Annotations;
using Reactive.Components;
using Reactive.Yoga;
using TMPro;
using UnityEngine;

namespace Reactive.BeatSaber.Components {
    [PublicAPI]
    public class LabelButton : ColoredButton, IComponentHolder<Label>, ISkewedComponent, ILeafLayoutItem {
        #region Label Adapter

        public string Text {
            get => _label.Text;
            set => _label.Text = value;
        }

        public bool RichText {
            get => _label.RichText;
            set => _label.RichText = value;
        }

        public float FontSize {
            get => _label.FontSize;
            set => _label.FontSize = value;
        }

        public float FontSizeMin {
            get => _label.FontSizeMin;
            set => _label.FontSizeMin = value;
        }

        public float FontSizeMax {
            get => _label.FontSizeMax;
            set => _label.FontSizeMax = value;
        }

        public bool EnableAutoSizing {
            get => _label.EnableAutoSizing;
            set => _label.EnableAutoSizing = value;
        }

        public FontStyles FontStyle {
            get => _label.FontStyle;
            set => _label.FontStyle = value;
        }

        public TMP_FontAsset Font {
            get => _label.Font;
            set => _label.Font = value;
        }

        public Material Material {
            get => _label.Material;
            set => _label.Material = value;
        }

        public bool EnableWrapping {
            get => _label.EnableWrapping;
            set => _label.EnableWrapping = value;
        }

        public TextOverflowModes Overflow {
            get => _label.Overflow;
            set => _label.Overflow = value;
        }

        public TextAlignmentOptions Alignment {
            get => _label.Alignment;
            set => _label.Alignment = value;
        }

        float ISkewedComponent.Skew {
            get => ((ISkewedComponent)_label).Skew;
            set => ((ISkewedComponent)_label).Skew = value;
        }

        public Label WrappedLabel => _label;

        #endregion

        #region Color

        protected override void ApplyColor(Color color) {
            if (Colors != null) {
                _label.Color = color;
            }
        }

        protected override void OnInteractableChange(bool interactable) {
            UpdateColor();
        }

        #endregion

        #region Leaf Adapter

        public event Action<ILeafLayoutItem>? LeafLayoutUpdatedEvent {
            add => _label.LeafLayoutUpdatedEvent += value;
            remove => _label.LeafLayoutUpdatedEvent -= value;
        }

        public Vector2 Measure(float width, MeasureMode widthMode, float height, MeasureMode heightMode) {
            return _label.Measure(width, widthMode, height, heightMode);
        }

        #endregion

        #region Setup

        Label IComponentHolder<Label>.Component => _label;

        private Label _label = null!;

        // Unfortunately text does not work with pointer events, so we have to wrap it into another game object
        protected override void Construct(RectTransform rect) {
            new Label {
                    Name = "Label"
                }
                .WithRectExpand()
                .Bind(ref _label)
                .Use(rect);

            // Adding touchable to allow raycasts
            rect.gameObject.AddComponent<Touchable>();
            base.Construct(rect);
        }

        protected override void OnInitialize() {
            RoutePropertyChanged(_label, null);
        }

        #endregion
    }
}