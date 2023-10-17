using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(Vector4))]
public class Vector4Editor : PropertyDrawer {

    #region UI Toolkit implementation

    public override VisualElement CreatePropertyGUI(SerializedProperty property) {
        // Create property container element
        var input = new VisualElement();
        input.style.flexDirection = FlexDirection.Row;

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

            item.RemoveFromClassList("unity-base-field__aligned"); // prevent auto alignment
            item.AddToClassList("unity-composite-field__field"); // flex-grow: 1, flex-basis: 0
            if(first) {
                item.AddToClassList("unity-composite-field__field--first"); // no margin-left
                first = false;
            }

            input.Add(item);
        }

        // Add fields to the container
        var container = new GenericField<Couple<int, int>>(property.displayName, input);

        return container;
    }

    #endregion
}
