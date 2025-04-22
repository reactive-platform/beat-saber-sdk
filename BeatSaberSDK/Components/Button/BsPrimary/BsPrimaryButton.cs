using System.Collections.Generic;
using JetBrains.Annotations;
using Reactive.Components;
using TMPro;
using UnityEngine;

namespace Reactive.BeatSaber.Components;

[PublicAPI]
public class BsPrimaryButton : BsPrimaryButtonBase, IComponentHolder<Label> {
    #region Adapter

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

    #endregion

    #region Setup

    Label IComponentHolder<Label>.Component => _label;

    private Label _label = null!;

    protected override IEnumerable<IReactiveComponent> ConstructContent() {
        return [
            new Label()
                .AsFlexItem(size: "auto")
                .Bind(ref _label)
        ];
    }

    protected override void OnInitialize() {
        base.OnInitialize();
        FontStyle |= FontStyles.UpperCase;
        Alignment = TextAlignmentOptions.Capline;
    }

    protected override void OnSkewChanged(float skew) {
        ((ISkewedComponent)_label).Skew = skew;
    }

    protected override void OnColorChanged() {
        var alpha = GraphicState.IsInteractable() ?
            GraphicState.IsHovered() ? 1 : 0.75f
            : 0.25f;

        _label.Color = Color.white.ColorWithAlpha(alpha);
    }

    #endregion
}