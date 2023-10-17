using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(MinMax<>))]
public class MinMaxEditor : PropertyDrawer {

    #region UI Toolkit implementation

    public override VisualElement CreatePropertyGUI(SerializedProperty property) {
        // Create property container element
        var input = new VisualElement();
        input.style.flexDirection = FlexDirection.Row;

        // Create property fields
        var names = new List<string> {
            nameof(MinMax<int>.Min),
            nameof(MinMax<int>.Max)
        };
        var titles = new List<string> { "░", " -" };
        for(int i = 0; i < 2; i++) {
            input.Add(new PropertyField(property.FindPropertyRelative(names[i]), titles[i]));
        }

        // Add fields to the container
        var container = new GenericField<MinMax<int>>(property.displayName, input, setupCompositeInput: true);
        return container;
    }

    #endregion

    #region IMGUI implementation

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
    #endregion
}
