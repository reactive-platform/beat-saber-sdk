using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Reactive.Components;
using UnityEngine;

namespace Reactive.BeatSaber.Components {
    /// <typeparam name="TKey">An item key</typeparam>
    /// <typeparam name="TParam">A param to be passed with key to provide additional info</typeparam>
    /// <typeparam name="TCell">A cell component</typeparam>
    [PublicAPI]
    public partial class Dropdown<TKey, TParam, TCell> : ReactiveComponent, ISkewedComponent, IKeyedControl<TKey, TParam>
        where TCell : IReactiveComponent, ILayoutItem, ISkewedComponent, IPreviewableCell, IKeyedControlCell<TKey, TParam>, new() {
        private struct DropdownOption : IEquatable<DropdownOption> {
            public TKey key;
            public TParam param;

            public override int GetHashCode() {
                return key?.GetHashCode() ?? 0;
            }

            public override bool Equals(object? obj) {
                return obj is DropdownOption opt && opt.key!.Equals(key);
            }

            public bool Equals(DropdownOption other) {
                return key?.Equals(other.key) ?? false;
            }
        }

        #region Dropdown

        public IDictionary<TKey, TParam> Items => _items;

        public TKey SelectedKey {
            get => _selectedKey.Value ?? throw new InvalidOperationException("Key cannot be acquired when Items is empty");
            private set {
                _selectedKey = value;

                SelectedKeyChangedEvent?.Invoke(value);
                NotifyPropertyChanged();
            }
        }

        public event Action<TKey>? SelectedKeyChangedEvent;

        private readonly ObservableDictionary<TKey, TParam> _items = new();
        private readonly HashSet<DropdownOption> _options = new();
        private Optional<TKey> _selectedKey;

        public void Select(TKey key) {
            if (_modalOpened) {
                Table.ClearSelection();
                Table.Select(new DropdownOption { key = key });
            }

            SelectedKey = key;

            _previewCell.Init(_selectedKey!, _items[_selectedKey!]);
        }

        private void RefreshSelection() {
            if (_selectedKey.HasValue || Items.Count <= 0) {
                return;
            }

            Select(Items.Keys.First());
        }

        #endregion

        #region Impl

        public float Skew {
            get => _skew;
            set {
                _skew = value;
                _button.Image.Skew = value;
                _previewCell.Skew = value;
            }
        }

        public bool Interactable {
            get => _interactable;
            set {
                _interactable = value;
                _canvasGroup.alpha = value ? 1f : 0.25f;
                _button.Interactable = value;
            }
        }

        private float _skew;
        private bool _interactable = true;

        #endregion

        #region Construct

        private Table<DropdownOption, DropdownCellWrapper> Table => _modal.Modal.Table;

        private bool _modalOpened;
        private SharedDropdownOptionsModal _modal = null!;

        private ImageButton _button = null!;
        private TCell _previewCell = default!;
        private CanvasGroup _canvasGroup = null!;

        protected override GameObject Construct() {
            new SharedDropdownOptionsModal()
                .With(x => x.BuildImmediate())
                .WithOpenListener(HandleModalOpened)
                .WithCloseListener(HandleModalClosed)
                .WithBeforeOpenListener(HandleBeforeModalOpened)
                .Bind(ref _modal);

            return new BackgroundButton {
                    Image = {
                        Sprite = BeatSaberResources.Sprites.background,
                        PixelsPerUnit = 12f,
                        Material = GameResources.UINoGlowMaterial
                    },
                    Colors = BeatSaberStyle.ControlColorSet,

                    OnClick = () => {
                        if (Items.Count == 0) {
                            return;
                        }

                        _modal.Present(ContentTransform);
                    },

                    Children = {
                        new TCell {
                                UsedAsPreview = true
                            }
                            .AsFlexItem(flexGrow: 1f)
                            .Bind(ref _previewCell),

                        // Icon
                        new Image {
                            Sprite = GameResources.ArrowIcon,
                            Color = Color.white.ColorWithAlpha(0.8f),
                            PreserveAspect = true
                        }.AsFlexItem(size: new() { x = 3f })
                    }
                }
                .WithNativeComponent(out _canvasGroup)
                .AsFlexGroup(padding: new() { left = 2f, right = 2f })
                .Bind(ref _button)
                .Use();
        }

        protected override void OnInitialize() {
            this.AsFlexItem(size: new() { x = 36f, y = 6f });
            Skew = BeatSaberStyle.Skew;

            _items.ItemAddedEvent += HandleItemAdded;
            _items.ItemRemovedEvent += HandleItemRemoved;
            _items.AllItemsRemovedEvent += HandleAllItemsRemoved;
        }

        #endregion

        #region Callbacks

        private void HandleBeforeModalOpened(IModal modal) {
            var key = new DropdownOption { key = _selectedKey.Value! };
            Table.Items.Clear();
            Table.Items.AddRange(_options);

            Table.Refresh();
            Table.Select(key);
            
            _modal.Modal.ApplyLayout(ContentTransform);
        }

        private void HandleModalOpened(IModal modal, bool finished) {
            if (finished) {
                return;
            }

            Table.WithListener(x => x.SelectedIndexes, HandleSelectedIndexesUpdated);
            _modalOpened = true;
        }

        private void HandleModalClosed(IModal modal, bool finished) {
            if (finished) {
                return;
            }

            Table.WithoutListener(x => x.SelectedIndexes, HandleSelectedIndexesUpdated);
            _modalOpened = false;
        }

        private void HandleSelectedIndexesUpdated(IReadOnlyCollection<int> indexes) {
            if (indexes.Count == 0) {
                return;
            }

            var index = indexes.First();
            var item = Table.FilteredItems[index];

            SelectedKey = item.key;

            _previewCell.Init(item.key, item.param);
        }

        private void HandleItemAdded(TKey key, TParam param) {
            var option = new DropdownOption { key = key, param = param };

            _options.Add(option);

            if (_modalOpened) {
                Table.Items.Add(option);
                Table.Refresh(false);
            }

            NotifyPropertyChanged(nameof(Items));
            RefreshSelection();
        }

        private void HandleItemRemoved(TKey key, TParam param) {
            var option = new DropdownOption { key = key };

            _options.Remove(option);

            if (_modalOpened) {
                Table.Items.Remove(option);
                Table.Refresh();
            }

            NotifyPropertyChanged(nameof(Items));
            RefreshSelection();
        }

        private void HandleAllItemsRemoved() {
            _options.Clear();

            if (_modalOpened) {
                Table.Items.Clear();
                Table.Refresh();
            }

            NotifyPropertyChanged(nameof(Items));
            RefreshSelection();
        }

        #endregion
    }
}