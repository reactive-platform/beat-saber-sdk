using JetBrains.Annotations;
using Reactive.Components;
using Reactive.Yoga;
using UnityEngine;

namespace Reactive.BeatSaber.Components {
    [PublicAPI]
    public class DialogHeader : ReactiveComponent, IComponentHolder<Label>, IGraphic {
        public string Text {
            get => _label.Text;
            set => _label.Text = value;
        }

        Label IComponentHolder<Label>.Component => _label;
        
        private Label _label = null!;

        protected override GameObject Construct() {
            return new Background {
                Sprite = BeatSaberResources.Sprites.backgroundTop,
                Color = (Color.white * 0.9f).ColorWithAlpha(1f),

                Children = {
                    new Label()
                        .AsFlexItem()
                        .Bind(ref _label)
                }
            }.AsFlexGroup(justifyContent: Justify.Center).AsBlurBackground().Use();
        }
    }
}