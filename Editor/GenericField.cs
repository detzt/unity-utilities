using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// The generic field is a non-abstract version of <see cref="BaseField{TValueType}"/>.
/// It can be filled with any <see cref="VisualElement"/> to create custom inspectors with automatic label alignment.
/// </summary>
public class GenericField<TValueType> : BaseField<TValueType> {

    /// <summary>
    /// Creates a new generic field where the label input are aligned with the built in fields.
    /// </summary>
    /// <param name="label">The text to use for the label.</param>
    /// <param name="input">The <see cref="VisualElement"/> to use as the input.</param>
    /// <param name="setupCompositeInput">Whether to setup the class list for composite input fields.</param>
    public GenericField(string label, VisualElement input, bool setupCompositeInput = false) : base(label, input) {
        styleSheets.Add(Resources.Load<StyleSheet>("EditorStyles"));
        AddToClassList("unity-base-field__aligned"); // align label and input with the built in fields
        AddToClassList("generic-field"); // BEM convention

        if(setupCompositeInput) {
            input.RegisterCallback<GeometryChangedEvent>(evt => {
                SetupClassListForCompositeInput(input);
            });
        }
    }

    /// <summary>
    /// Adjusts the class list of the contained input fields.
    /// Since they are now used inline instead of standalone.
    /// </summary>
    private static void SetupClassListForCompositeInput(VisualElement container) {
        var first = true;
        foreach(var field in container.Children()) {
            field.RemoveFromClassList("unity-base-field__aligned"); // prevent auto alignment
            field.AddToClassList("unity-composite-field__field"); // flex-grow: 1, flex-basis: 0
            field[0].AddToClassList("unity-composite-field__field");
            if(first) {
                field[0].AddToClassList("unity-composite-field__field--first"); // no margin-left
                first = false;
            }
        }
    }
}
