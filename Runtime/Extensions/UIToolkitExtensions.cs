using UnityEngine.UIElements;

/// <summary>
/// Extension methods for <see cref="VisualElement"/> and other UIToolkit classes.
/// </summary>
public static class UIToolkitExtensions {
    /// <summary>
    /// Set whether the element is hidden or not.<br/>
    /// Similar to <see cref="UnityEngine.GameObject.SetActive(bool)"/>
    /// </summary>
    /// <param name="element">The element to hide or show</param>
    /// <param name="value">Whether the element should be visible</param>
    public static void SetActive(this VisualElement element, bool value) {
        element.EnableInClassList("hidden", !value);
    }
}
