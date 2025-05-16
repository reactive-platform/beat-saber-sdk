using System.Linq;
using Reactive.Components;
using Reactive.Yoga;
using UnityEngine;

namespace Reactive.BeatSaber.Components;

public partial class Dropdown<TKey, TParam, TCell> {
    private class DropdownCellWrapper : TableCell<DropdownOption> {
        private TCell _cell = default!;

        protected override void OnInit(DropdownOption item) {
            _cell.Init(item.key, item.param);
        }

        protected override void OnCellStateChange(bool selected) {
            _cell.OnCellStateChange(selected);
        }

        protected override GameObject Construct() {
            return new TCell().Bind(ref _cell).Use(null);
        }

        protected override void OnInitialize() {
            this.WithSizeDelta(0f, 6f);
            _cell.CellAskedToBeSelectedEvent += HandleCellAskedToBeSelected;
        }

        private void HandleCellAskedToBeSelected(TKey key) {
            SelectSelf(true);
        }
    }

    private class DropdownOptionsModal : ModalBase {
        #region Setup

        private const int MaxDisplayedItems = 5;
        private const float ItemSize = 6f;

        private RectTransform _buttonTransform = null!;

        public void ApplyLayout(RectTransform buttonRect) {
            _buttonTransform = buttonRect;

            var width = buttonRect.rect.width;
            var height = Mathf.Clamp(Table.Items.Count, 1, MaxDisplayedItems) * ItemSize + 2;

            this.AsFlexItem(size: new() { x = width, y = height });
        }

        protected override void OnOpen(bool opened) {
            if (!opened) {
                this.WithAnchor(_buttonTransform, RelativePlacement.Center, immediate: true);
            }
        }

        #endregion

        #region Construct

        public Table<DropdownOption, DropdownCellWrapper> Table => _table;

        private Table<DropdownOption, DropdownCellWrapper> _table = null!;

        protected override GameObject Construct() {
            return new Layout {
                Children = {
                    new Background {
                            Children = {
                                new Table<DropdownOption, DropdownCellWrapper>()
                                    .WithListener(
                                        x => x.SelectedIndexes,
                                        _ => CloseInternal()
                                    )
                                    .AsFlexItem(flexGrow: 1f)
                                    .Bind(ref _table)
                            }
                        }
                        .AsBlurBackground()
                        .AsFlexGroup(padding: new() { top = 1f, bottom = 1f })
                        .AsFlexItem(flexGrow: 1f),

                    // Scrollbar
                    new Scrollbar()
                        .AsFlexItem(
                            size: new() { x = 2f, y = 100.pct() },
                            position: new() { right = -4f }
                        )
                        .With(x => Table.Scrollbar = x)
                }
            }.AsFlexGroup(gap: 2f, constrainHorizontal: false, constrainVertical: false).Use();
        }

        #endregion
    }
}