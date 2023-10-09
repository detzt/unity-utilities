using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(MinMax<>))]
public class MinMaxEditor : PropertyDrawer {
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label) {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        _ = EditorGUI.BeginProperty(rect, label, property);

        // Draw label
        rect = EditorGUI.PrefixLabel(rect, GUIUtility.GetControlID(FocusType.Passive), label);

        // Disable indentation for inline fields
        var prevIndent = EditorGUI.indentLevel;
        var prevLabelWidth = EditorGUIUtility.labelWidth;
        var labelWidth = EditorStyles.label.CalcSize(new GUIContent("X ")).x;
        EditorGUIUtility.labelWidth = labelWidth;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        var fieldWidth = rect.width / 2f;
        var rect1 = new Rect(rect.x, rect.y, fieldWidth, rect.height);
        var rect2 = new Rect(rect.x + fieldWidth, rect.y, fieldWidth, rect.height);

        // Draw fields - pass GUIContent.none to each so they are drawn without labels
        _ = EditorGUI.PropertyField(rect1, property.FindPropertyRelative(nameof(MinMax<int>.Min)), new GUIContent("░ "));
        _ = EditorGUI.PropertyField(rect2, property.FindPropertyRelative(nameof(MinMax<int>.Max)), new GUIContent(" -"));

        // Restore indent
        EditorGUI.indentLevel = prevIndent;
        EditorGUIUtility.labelWidth = prevLabelWidth;

        EditorGUI.EndProperty();
    }
}
