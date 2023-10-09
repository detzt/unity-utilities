using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(OptionalValue<>))]
public class OptionalValuePropertyDrawer : PropertyDrawer {
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
}
