using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Draws fields tagged with <see cref="AutoSetupAttribute"/> and tries to auto setup the field to a component of the corresponding type.
/// </summary>
[CustomPropertyDrawer(typeof(AutoSetupAttribute))]
public class AutoSetupDrawer : PropertyDrawer {

    #region UI Toolkit implementation

    public override VisualElement CreatePropertyGUI(SerializedProperty property) {
        // Create property container element
        if(property.propertyType != SerializedPropertyType.ObjectReference) {
            var info = new Label("Use AutoSetup with object reference fields!");
            var container = new GenericField<System.Type>(property.displayName, info);
            return container;
        }

        // If property is null, try to auto setup to a component of the corresponding type
        if(property.objectReferenceValue == null) {
            var type = fieldInfo.FieldType; // The type of the field this attribute is applied to
            if((property.serializedObject.targetObject as Component).TryGetComponent(type, out var matchingComponent)) {
                property.objectReferenceValue = matchingComponent;
                _ = property.serializedObject.ApplyModifiedProperties();
            }
        }

        var objectField = new ObjectField(property.displayName) {
            objectType = fieldInfo.FieldType,
            value = property.objectReferenceValue
        };
        objectField.BindProperty(property);
        objectField.AddToClassList("unity-base-field__aligned");
        return objectField;
    }

    #endregion

    #region IMGUI implementation

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
    #endregion
}
