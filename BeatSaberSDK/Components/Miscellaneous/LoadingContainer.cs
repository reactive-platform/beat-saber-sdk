using JetBrains.Annotations;
using Reactive.Components;
using UnityEngine;

namespace Reactive.BeatSaber.Components {
    [PublicAPI]
    public class LoadingContainer : ReactiveComponent {
        public ILayoutItem? Component {
            get => _component;
            set {
                if (_component != null) {
                    _container.Children.Remove(_component);
                }
                _component = value;
                if (_component != null) {
                    _component.WithRectExpand();
                    _container.Children.Add(_component);
                }
            }
        }

        public bool Loading {
            get => _loading;
            set {
                _loading = value;
                _spinner.Enabled = value;
                _containerGroup.alpha = value ? 0.2f : 1f;
                _containerGroup.interactable = !value;
            }
        }

        private ILayoutItem? _component;
        private bool _loading;

        private CanvasGroup _containerGroup = null!;
        private Layout _container = null!;
        private Layout _spinner = null!;

        protected override GameObject Construct() {
            return new Layout {
                Children = {
                    new Layout()
                        .AsFlexGroup()
                        .AsFlexItem(flexGrow: 1f)
                        .WithNativeComponent(out _containerGroup)
                        .WithRectExpand()
                        .Bind(ref _container),
                    //spinner container
                    new Layout {
                        Enabled = false,
                        Children = {
                            new Spinner().AsFlexItem(
                                minSize: new() { x = 2f },
                                maxSize: new() { x = 10f },
                                flexGrow: 1f
                            )
                        }
                    }.AsFlexGroup(padding: 1f).WithRectExpand().Bind(ref _spinner)
                }
            }.AsFlexGroup().Use();
        }

        protected override void OnInitialize() {
            this.AsFlexItem();
        }
    }
}