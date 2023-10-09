using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Couple<,>))]
public class CoupleEditor : PropertyDrawer {
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
