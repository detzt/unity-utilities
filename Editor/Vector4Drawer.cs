using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(Vector4))]
public class Vector4Drawer : PropertyDrawer {

    #region UI Toolkit implementation

    public override VisualElement CreatePropertyGUI(SerializedProperty property) {
        // Create property container element
        var input = new VisualElement();

        // Create property fields
        var names = new List<string> {
            nameof(Vector4.x),
            nameof(Vector4.y),
            nameof(Vector4.z),
            nameof(Vector4.w)
        };
        var first = true;
        for(int i = 0; i < 4; i++) {
            var item = new FloatField();
            item.BindProperty(property.FindPropertyRelative(names[i]));
            item.label = names[i].ToUpper();

            item.RemoveFromClassList(BaseField<object>.alignedFieldUssClassName); // prevent auto alignment (class = "unity-base-field__aligned")
            item.AddToClassList(Vector4Field.fieldUssClassName); // flex-grow: 1, flex-basis: 0 (class = "unity-composite-field__field")
            if(first) {
                item.AddToClassList(Vector4Field.firstFieldVariantUssClassName); // no margin-left (class = "unity-composite-field__field--first")
                first = false;
            }

            input.Add(item);
        }

        // Add fields to the container
        var container = new GenericField<Vector4>(property.displayName, input);

        return container;
    }

    #endregion
}
