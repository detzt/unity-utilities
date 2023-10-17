using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(OptionalValue<>))]
public class OptionalValuePropertyDrawer : PropertyDrawer {

    #region UI Toolkit implementation

    public override VisualElement CreatePropertyGUI(SerializedProperty property) {
        // Create property container element
        var input = new VisualElement();
        input.AddToClassList("input-container");

        // Create property fields
        var enabledProperty = property.FindPropertyRelative("enabled");
        var valueProperty = property.FindPropertyRelative("value");
        var enabledField = new PropertyField(enabledProperty, "");
        var valueField = new PropertyField(valueProperty, "");
        enabledField.AddToClassList("optional-toggle");
        valueField.AddToClassList("optional-value");

        // Add fields to the container
        input.Add(enabledField);
        input.Add(valueField);
        var container = new GenericField<System.Type>(property.displayName, input);

        // Actual functionality of disabling the value field when the enabled field is false
        valueField.SetEnabled(enabledProperty.boolValue);
        enabledField.RegisterValueChangeCallback(evt => {
            valueField.SetEnabled(evt.changedProperty.boolValue);
        });

        return container;
    }

    #endregion

    #region IMGUI implementation

    private readonly GUIContent whiteSpace = new("   ");

    /// <summary>
    ///   <para>Override this method to make your own IMGUI based GUI for the property.</para>
    /// </summary>
    /// <param name="position">Rectangle on the screen to use for the property GUI.</param>
    /// <param name="property">The SerializedProperty to make the custom GUI for.</param>
    /// <param name="label">The label of this property.</param>
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        var enabledProperty = property.FindPropertyRelative("enabled");
        var valueProperty = property.FindPropertyRelative("value");
        var guiEnabled = GUI.enabled;
        var controlRect = position;
        GUI.enabled = enabledProperty.boolValue;
        var valueRect = controlRect;
        valueRect.x += 30;
        valueRect.width -= 30;
        var labelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth -= 30;
        whiteSpace.tooltip = label.tooltip;
        _ = EditorGUI.PropertyField(valueRect, valueProperty, whiteSpace);
        EditorGUIUtility.labelWidth = labelWidth;
        GUI.enabled = guiEnabled;
        var boolRect = controlRect;
        boolRect.width = EditorGUIUtility.labelWidth - 30f;
        enabledProperty.boolValue = EditorGUI.ToggleLeft(boolRect, property.displayName, enabledProperty.boolValue);
    }
    #endregion
}
