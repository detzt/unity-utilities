using System.Linq;
using UnityEngine.UIElements;

/// <summary>
/// Collection of Editor utility functions
/// </summary>
public static class EditorUtils {

    /// <summary>
    /// Removes the auto alignment of the label from the given PropertyField (callback)
    /// </summary>
    /// <param name="ctx">The callback parameter from <see cref="PropertyField.generateVisualContent"/></param>
    public static void RemoveLabelResizing(MeshGenerationContext ctx) {
        var field = ctx.visualElement.Children().First();
        field.RemoveFromClassList("unity-base-field__aligned"); // prevent auto alignment
        field.Children().First().style.minWidth = 12f;
        field.style.marginLeft = 5f;
    }
}
