using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(Couple<,>))]
public class CoupleEditor : PropertyDrawer {

    /// <summary>
    /// UI Toolkit implementation
    /// </summary>
    public override VisualElement CreatePropertyGUI(SerializedProperty property) {
        // Create property container element
        var container = new VisualElement();
        container.style.flexDirection = FlexDirection.Row;

        // Create property fields
        var item1 = new PropertyField(property.FindPropertyRelative(nameof(Couple<int, int>.Item1)), property.displayName);
        var item2 = new PropertyField(property.FindPropertyRelative(nameof(Couple<int, int>.Item2)), "░ ");
        item1.style.flexGrow = 2f; // First item includes the label, so it should be wider
        item2.style.flexGrow = 1f;
        item1.style.flexBasis = 0f; // Set a fixed base width independent of the property's value
        item2.style.flexBasis = 0f;
        item2.generateVisualContent += EditorUtils.RemoveLabelResizing; // remove the alignment class from the second field once it is drawn

        // Add fields to the container
        container.Add(item1);
        container.Add(item2);

        return container;
    }

    /// <summary>
    /// Fallback to the IMGUI version when used within a custom IMGUI editor
    /// </summary>
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
        var fieldWidth = (rect.width - fieldGap) / 2f;
        var rect1 = new Rect(rect.x, rect.y, fieldWidth, rect.height);
        var rect2 = new Rect(rect.x + fieldWidth + fieldGap, rect.y, fieldWidth, rect.height);

        // Draw fields - pass GUIContent.none to each so they are drawn without labels
        _ = EditorGUI.PropertyField(rect1, property.FindPropertyRelative(nameof(Couple<int, int>.Item1)), GUIContent.none);
        _ = EditorGUI.PropertyField(rect2, property.FindPropertyRelative(nameof(Couple<int, int>.Item2)), GUIContent.none);

        // Restore indent
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}
