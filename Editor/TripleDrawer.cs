using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(Triple<,,>))]
public class TripleEditor : PropertyDrawer {

    public override VisualElement CreatePropertyGUI(SerializedProperty property) {
        // Create property container element
        var container = new VisualElement();
        container.style.flexDirection = FlexDirection.Row;

        // Create property fields
        var item1 = new PropertyField(property.FindPropertyRelative(nameof(Triple<int, int, int>.Item1)), property.displayName);
        var item2 = new PropertyField(property.FindPropertyRelative(nameof(Triple<int, int, int>.Item2)), "░ ");
        var item3 = new PropertyField(property.FindPropertyRelative(nameof(Triple<int, int, int>.Item3)), "░ ");
        item1.style.flexGrow = 3f; // First item includes the label, so it should be wider
        item2.style.flexGrow = 1f;
        item3.style.flexGrow = 1f;
        item1.style.flexBasis = 0f; // Set a fixed base width independent of the property's value
        item2.style.flexBasis = 0f;
        item3.style.flexBasis = 0f;
        item2.generateVisualContent += RemoveLabelResizing; // remove the alignment class from the second field once it is drawn
        item3.generateVisualContent += RemoveLabelResizing;

        // Add fields to the container
        container.Add(item1);
        container.Add(item2);
        container.Add(item3);

        return container;
    }

    /// <summary>
    /// Removes the auto alignment of the label from the given PropertyField (callback)
    /// </summary>
    /// <param name="ctx">The callback parameter from <see cref="PropertyField.generateVisualContent"/></param>
    private void RemoveLabelResizing(MeshGenerationContext ctx) {
        var field = ctx.visualElement.Children().First();
        field.RemoveFromClassList("unity-base-field__aligned"); // prevent auto alignment
        field.Children().First().style.minWidth = 12f;
        field.style.marginLeft = 5f;
    }

    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label) {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        _ = EditorGUI.BeginProperty(rect, label, property);

        // Draw label
        rect = EditorGUI.PrefixLabel(rect, GUIUtility.GetControlID(FocusType.Passive), label);

        // Disable indentation for inline fields
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        var fieldGap = 8f;
        var fieldWidth = (rect.width - 2 * fieldGap) / 3f;
        var rect1 = new Rect(rect.x, rect.y, fieldWidth, rect.height);
        var rect2 = new Rect(rect.x + fieldWidth + fieldGap, rect.y, fieldWidth, rect.height);
        var rect3 = new Rect(rect.x + 2 * (fieldWidth + fieldGap), rect.y, fieldWidth, rect.height);

        // Draw fields - pass GUIContent.none to each so they are drawn without labels
        _ = EditorGUI.PropertyField(rect1, property.FindPropertyRelative(nameof(Triple<int, int, int>.Item1)), GUIContent.none);
        _ = EditorGUI.PropertyField(rect2, property.FindPropertyRelative(nameof(Triple<int, int, int>.Item2)), GUIContent.none);
        _ = EditorGUI.PropertyField(rect3, property.FindPropertyRelative(nameof(Triple<int, int, int>.Item3)), GUIContent.none);

        // Restore indent
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}
