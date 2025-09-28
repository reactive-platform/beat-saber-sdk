using System.Collections.Generic;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using VRUIControls;

namespace Reactive.BeatSaber.Components;

/// <summary>
/// Marks this object to filter off raycasts beyond <see cref="RectMask2D"/> bounds. 
/// </summary>
[PublicAPI]
[RequireComponent(typeof(RectMask2D))]
[HarmonyPatch(typeof(VRGraphicRaycaster), "RaycastCanvas")]
public class RaycastMask2D : MonoBehaviour {
    #region Mask

    public RectMask2D Mask { get; private set; } = null!;

    private RectTransform _rectTransform = null!;

    private void Awake() {
        Mask = GetComponent<RectMask2D>();
        _rectTransform = GetComponent<RectTransform>();
    }

    /// <summary>
    /// Takes a canvas point and checks whether that point within the mask bounds. 
    /// </summary>
    public bool ValidatePoint(Vector2 point) {
        var localPos = _rectTransform.InverseTransformPoint(point);

        return ValidateLocalPoint(localPos);
    }

    /// <summary>
    /// Takes a local point and checks whether that point within the mask bounds. 
    /// </summary>
    public bool ValidateLocalPoint(Vector2 point) {
        return !isActiveAndEnabled || _rectTransform.rect.Contains(point);
    }

    #endregion

    #region Patch

    private static readonly List<RaycastMask2D> masksBuffer = new();

    [UsedImplicitly]
    private static void Postfix(List<VRGraphicRaycaster.VRGraphicRaycastResult> results) {
        var i = 0;
        while (i < results.Count) {
            var raycast = results[i];

            if (raycast.graphic is MaskableGraphic graphic) {
                masksBuffer.Clear();
                graphic.GetComponentsInParent(false, masksBuffer);

                var valid = true;
                foreach (var mask in masksBuffer) {
                    valid = mask.ValidatePoint(raycast.insideRootCanvasPosition);

                    if (!valid) {
                        break;
                    }
                }

                if (!valid) {
                    results.RemoveAt(i);
                    continue;
                }
            }

            i++;
        }
    }

    #endregion
}