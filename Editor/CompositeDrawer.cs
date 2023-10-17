using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Base class for all property drawers that draw multiple property fields in one line.
/// Add [CustomPropertyDrawer(typeof(DrawnType))] to subclasses.
/// </summary>
public abstract class CompositeDrawer : PropertyDrawer {

    #region UI Toolkit implementation

    /// <summary>
    /// Retrieves the names of the fields to draw.
    /// The first item is the name of the field, the second is the label to display.
    /// </summary>
    protected abstract List<Couple<string, string>> GetNames();

    public override VisualElement CreatePropertyGUI(SerializedProperty property) {
        // Create property container element
        var input = new VisualElement();
        input.style.flexDirection = FlexDirection.Row;

        // Create property fields
        var names = GetNames();
        foreach(var name in names) {
            input.Add(new PropertyField(property.FindPropertyRelative(name.Item1), name.Item2));
        }

        // Add fields to the container
        var container = new GenericField<Triple<int, int, int>>(property.displayName, input, setupCompositeInput: true);
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
        var names = GetNames();
        int count = names.Count;

        // Disable indentation for inline fields
        var prevIndent = EditorGUI.indentLevel;
        var prevLabelWidth = EditorGUIUtility.labelWidth;
        var labelWidth = EditorStyles.label.CalcSize(new GUIContent("X ")).x;
        EditorGUIUtility.labelWidth = labelWidth;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        var fieldGap = 3f;
        var fieldWidth = (rect.width - ((count - 1) * fieldGap)) / count;

        for(int i = 0; i < count; i++) {
            var fieldRect = new Rect(rect.x + i * (fieldWidth + fieldGap), rect.y, fieldWidth, rect.height);

            // Draw fields
            _ = EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative(names[i].Item1), new GUIContent(names[i].Item2));
        }

        // Restore indent
        EditorGUI.indentLevel = prevIndent;
        EditorGUIUtility.labelWidth = prevLabelWidth;

        EditorGUI.EndProperty();
    }
    #endregion
}
