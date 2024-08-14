#if UNITY_2023_2_OR_NEWER

using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// A container that maintains a desired aspect ratio inside of it by adding padding.<br/>
/// Its outer dimensions can be arbitrary and are not affected by the aspect ratio.
/// </summary>
[UxmlElement]
public partial class AspectRatioContainer : VisualElement {

    [UxmlAttribute("width"), Tooltip("The width part of the desired aspect ratio.")]
    public float RatioWidth { get; private set; } = 16f;

    [UxmlAttribute("height"), Tooltip("The height part of the desired aspect ratio.")]
    public float RatioHeight { get; private set; } = 9f;

    public AspectRatioContainer() {
    }

    [EventInterest(typeof(AttachToPanelEvent), typeof(GeometryChangedEvent))]
    protected override void HandleEventBubbleUp(EventBase evt) {
        base.HandleEventBubbleUp(evt);
        if(evt.eventTypeId == AttachToPanelEvent.TypeId() || evt.eventTypeId == GeometryChangedEvent.TypeId()) {
            // Update the padding when the element is attached to a panel or the geometry changes.
            UpdateElements();
        }
    }

    /// <summary>
    /// Update the padding.
    /// </summary>
    public void UpdateElements() {
        if(RatioWidth <= 0.0f || RatioHeight <= 0.0f) {
            style.paddingLeft = 0;
            style.paddingRight = 0;
            style.paddingTop = 0;
            style.paddingBottom = 0;
            Debug.LogError($"[AspectRatioContainer] RatioWidth ({RatioWidth}) and RatioHeight ({RatioHeight}) need to greater than zero.");
            return;
        }

        var designRatio = RatioWidth / RatioHeight;
        var currRatio = resolvedStyle.width / resolvedStyle.height;

        if(designRatio == currRatio) return;

        if(float.IsNaN(resolvedStyle.width) || float.IsNaN(resolvedStyle.height)) return;

        if(currRatio > designRatio) {
            // Wider than desired, add padding to the left and right.
            var w = (resolvedStyle.width - (resolvedStyle.height * designRatio)) * 0.5f;
            style.paddingLeft = w;
            style.paddingRight = w;
            style.paddingTop = 0;
            style.paddingBottom = 0;
        } else {
            // Taller than desired, add padding to the top and bottom.
            var h = (resolvedStyle.height - (resolvedStyle.width / designRatio)) * 0.5f;
            style.paddingTop = h;
            style.paddingBottom = h;
            style.paddingLeft = 0;
            style.paddingRight = 0;
        }
    }
}
#endif
