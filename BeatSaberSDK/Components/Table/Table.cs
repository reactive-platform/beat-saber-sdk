using JetBrains.Annotations;
using Reactive.Components;

namespace Reactive.BeatSaber.Components;

[PublicAPI]
public class Table<TItem> : Reactive.Components.Basic.Table<TItem> {
    protected override Reactive.Components.Basic.ScrollArea ConstructScrollArea() {
        return new ScrollArea();
    }
}

[PublicAPI]
public class Table<TItem, TCell> : Reactive.Components.Basic.Table<TItem, TCell> where TCell : ITableCell<TItem>, IReactiveComponent, new() {
    protected override Reactive.Components.Basic.ScrollArea ConstructScrollArea() {
        return new ScrollArea();
    }
}