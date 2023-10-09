using UnityEngine;
using UnityEditor;

/// <summary>
/// Draws fields tagged with <see cref="AutoSetupAttribute"/> and tries to auto setup the field to a component of the corresponding type.
/// </summary>
[CustomPropertyDrawer(typeof(AutoSetupAttribute))]
public class AutoSetupDrawer : PropertyDrawer {

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

        // Now draw the property as a Slider or an IntSlider based on whether it's a float or integer.
        if(property.propertyType != SerializedPropertyType.ObjectReference) {
            EditorGUI.LabelField(position, label.text, "Use AutoSetup with object reference fields.");
        } else {
            EditorGUI.ObjectField(position, property, label);

            // If property is null, try to auto setup to a component of the corresponding type
            if(property.objectReferenceValue == null) {
                var type = fieldInfo.FieldType; // The type of the field this attribute is applied to
                if((property.serializedObject.targetObject as Component).TryGetComponent(type, out var matchingComponent))
                    property.objectReferenceValue = matchingComponent;
            }
        }
    }
}
