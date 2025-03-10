using HMUI;
using JetBrains.Annotations;
using Reactive.Components;
using Reactive.Yoga;
using TMPro;
using UnityEngine;
using UImage = UnityEngine.UI.Image;

namespace Reactive.BeatSaber.Components;

[PublicAPI]
public static class ComponentExtensions {
    #region Button

    public static T WithLabel<T>(
        this T button,
        string text,
        float fontSize = 4f,
        bool richText = true,
        TextOverflowModes overflow = TextOverflowModes.Overflow
    ) where T : ButtonBase, IChildrenProvider {
        return WithLabel(
            button,
            out _,
            text,
            fontSize,
            richText,
            overflow
        );
    }

    public static T WithLabel<T>(
        this T button,
        out Label label,
        string text,
        float fontSize = 4f,
        bool richText = true,
        TextOverflowModes overflow = TextOverflowModes.Overflow
    ) where T : ButtonBase, IChildrenProvider {
        button.AsFlexGroup(alignItems: Align.Center);
        button.Children.Add(
            new Label {
                Text = text,
                FontSize = fontSize,
                RichText = richText,
                Overflow = overflow
            }.With(
                x => {
                    if (button is not ISkewedComponent skewed) {
                        return;
                    }
                    ((ISkewedComponent)x).Skew = skewed.Skew;
                }
            ).AsFlexItem(
                size: "auto",
                margin: new() { left = 2f, right = 2f }
            ).Export(out label)
        );
        return button;
    }

    public static T WithImage<T>(
        this T button,
        Sprite? sprite,
        Color? color = null,
        float? pixelsPerUnit = null,
        bool preserveAspect = true,
        UImage.Type type = UImage.Type.Simple,
        Optional<Material>? material = default
    ) where T : ButtonBase, IChildrenProvider {
        return WithImage(
            button,
            out _,
            sprite,
            color,
            pixelsPerUnit,
            preserveAspect,
            type,
            material
        );
    }

    public static T WithImage<T>(
        this T button,
        out Image image,
        Sprite? sprite,
        Color? color = null,
        float? pixelsPerUnit = null,
        bool preserveAspect = true,
        UImage.Type type = UImage.Type.Simple,
        Optional<Material>? material = default
    ) where T : ButtonBase, IChildrenProvider {
        button.Children.Add(
            new Image {
                Sprite = sprite,
                Material = material.GetValueOrDefault(GameResources.UINoGlowMaterial),
                Color = color ?? Color.white,
                PixelsPerUnit = pixelsPerUnit ?? 0f,
                ImageType = pixelsPerUnit == null ? type : UImage.Type.Sliced,
                PreserveAspect = preserveAspect
            }.With(
                x => {
                    if (button is not ISkewedComponent skewed) return;
                    x.Skew = skewed.Skew;
                }
            ).AsFlexItem(grow: 1f).Export(out image)
        );
        return button;
    }

    #endregion

    #region Image

    [Pure]
    public static Image InBackground(
        this ILayoutItem comp,
        Optional<Sprite> sprite = default,
        Optional<Material> material = default,
        Color? color = null,
        UImage.Type type = UImage.Type.Sliced,
        float pixelsPerUnit = 10f,
        float skew = 0f,
        ImageView.GradientDirection? gradientDirection = null,
        Color gradientColor0 = default,
        Color gradientColor1 = default
    ) {
        return comp.In<Image>().AsBackground(
            sprite,
            material,
            color,
            type,
            pixelsPerUnit,
            skew,
            gradientDirection,
            gradientColor0,
            gradientColor1
        );
    }

    [Pure]
    public static Image InBlurBackground(
        this ILayoutItem comp,
        float pixelsPerUnit = 12f,
        Color? color = null
    ) {
        return comp.In<Image>().AsBlurBackground(
            pixelsPerUnit,
            color
        );
    }

    public static T AsBlurBackground<T>(this T comp, float pixelsPerUnit = 12f, Color? color = null) where T : Image {
        comp.Sprite ??= BeatSaberResources.Sprites.background;
        comp.Material = GameResources.UIFogBackgroundMaterial;
        comp.Color = color ?? comp.Color;
        comp.PixelsPerUnit = pixelsPerUnit;
        return comp;
    }

    public static T AsBackground<T>(
        this T component,
        Optional<Sprite> sprite = default,
        Optional<Material> material = default,
        Color? color = null,
        UImage.Type type = UImage.Type.Sliced,
        float pixelsPerUnit = 10f,
        float skew = 0f,
        ImageView.GradientDirection? gradientDirection = null,
        Color gradientColor0 = default,
        Color gradientColor1 = default
    ) where T : Image {
        sprite.SetValueIfNotSet(BeatSaberResources.Sprites.background);
        material.SetValueIfNotSet(GameResources.UINoGlowMaterial);
        //adding image
        component.Sprite = sprite;
        component.Material = material;
        component.Color = color ?? Color.white;
        component.ImageType = type;
        component.PixelsPerUnit = pixelsPerUnit;
        component.Skew = skew;
        //applying gradient if needed
        if (gradientDirection.HasValue) {
            component.UseGradient = true;
            component.GradientDirection = gradientDirection.Value;
            component.GradientColor0 = gradientColor0;
            component.GradientColor1 = gradientColor1;
        }

        return component;
    }

    #endregion

    #region NamedRail

    [Pure]
    public static NamedRail InNamedRail(this ILayoutItem comp, string text) {
        return new NamedRail {
            Label = {
                Text = text
            },
            Component = comp
        }.With(
            x => {
                if (comp is not ISkewedComponent skewed) return;
                ((ISkewedComponent)x.Label).Skew = skewed.Skew;
            }
        );
    }

    #endregion

    #region Modal

    public static void Present<T>(this T comp, Transform child, bool animated = true) where T : IModal, IReactiveComponent {
        var screen = child.GetComponentInParent<ViewController>().transform;
        ModalSystem.PresentModal(comp, screen, animated);
    }

    public static T WithModal<T, TModal>(this T comp, TModal modal, bool animated = true)
        where T : ButtonBase where TModal : IModal, IReactiveComponent {
        comp.OnClick += () => modal.Present(comp.ContentTransform, animated);
        return comp;
    }

    #endregion
}