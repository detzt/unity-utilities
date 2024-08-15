using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Custom property drawer that displays InterfaceReferences in one line.
/// </summary>
[CustomPropertyDrawer(typeof(InterfaceReference<>))]
public class InterfaceReferenceDrawer : PropertyDrawer {

    #region UI Toolkit implementation

    public override VisualElement CreatePropertyGUI(SerializedProperty property) {
        // Create property element
        return new PropertyField(property.FindPropertyRelative(InterfaceReference<int>.NameOfObjectValue), property.displayName) {
            tooltip = property.tooltip
        };
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

        // Draw fields
        _ = EditorGUI.PropertyField(rect, property.FindPropertyRelative(InterfaceReference<int>.NameOfObjectValue), label);

        EditorGUI.EndProperty();
    }
    #endregion
}
