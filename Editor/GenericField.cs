using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// The generic field is a non-abstract version of <see cref="BaseField{TValueType}"/>.
/// It can be filled with any <see cref="VisualElement"/> to create custom inspectors with automatic label alignment.
/// </summary>
public class GenericField<TValueType> : BaseField<TValueType> {

    /// <summary>
    /// Creates a new generic field where the label and input are aligned with the built in fields.
    /// </summary>
    /// <param name="label">The text to use for the label.</param>
    /// <param name="input">The <see cref="VisualElement"/> to use as the input.</param>
    /// <param name="setupCompositeInput">Whether to setup the class list for composite input fields.</param>
    public GenericField(string label, VisualElement input, bool setupCompositeInput = false) : base(label, input) {
        styleSheets.Add(Resources.Load<StyleSheet>("EditorStyles"));
        input.AddToClassList("input-container");
        AddToClassList(BaseField<object>.alignedFieldUssClassName); // align label and input with the built in fields (class = "unity-base-field__aligned")
        AddToClassList("generic-field"); // BEM convention

        if(setupCompositeInput) {
#if UNITY_2023_2_OR_NEWER
            RegisterCallbackOnce<GeometryChangedEvent>(evt => {
                // In some cases the input field is not yet fully initialized upon the first GeometryChangedEvent.
                if(input.childCount > 0) {
                    SetupClassListForCompositeInput(input);
                } else {
                    // and a second GeometryChangedEvent has to be awaited.
                    RegisterCallbackOnce<GeometryChangedEvent>(_ => {
                        SetupClassListForCompositeInput(input);
                    });
                }
            });
#else
            // for older versions, the callback is needlessly called on every layout change
            input.RegisterCallback<GeometryChangedEvent>(evt => {
                SetupClassListForCompositeInput(input);
            });
#endif
        }
    }

    /// <summary>
    /// Creates a new generic field where the label and input are aligned with the built in fields.
    /// </summary>
    /// <param name="label">The <see cref="VisualElement"/> to use for the label.</param>
    /// <param name="input">The <see cref="VisualElement"/> to use as the input.</param>
    /// <param name="setupCompositeInput">Whether to setup the class list for composite input fields.</param>
    public GenericField(VisualElement label, VisualElement input, bool setupCompositeInput = false) : this("hidden", input, setupCompositeInput) {
        Add(label);

#if UNITY_2023_2_OR_NEWER
        // this has to be called earlier, before the __aligned fields get modified
        RegisterCallbackOnce<GeometryChangedEvent>(evt => {
            label.SendToBack();

            RemoveInnerAlignment(label);
            label.AddToClassList(Label.ussClassName); // (class = "unity-label")
            label.AddToClassList(BaseField<object>.labelUssClassName); // (class = "unity-base-field__label")

            labelElement.style.display = DisplayStyle.None;
        });
#else
        // this has to be called earlier, before the __aligned fields get modified
        RegisterCallback<GeometryChangedEvent>(evt => {
            if(label.parent.IndexOf(label) > 0) { // only once check
                label.SendToBack();

                RemoveInnerAlignment(label);
                label.AddToClassList(Label.ussClassName); // (class = "unity-label")
                label.AddToClassList(BaseField<object>.labelUssClassName); // (class = "unity-base-field__label")

                labelElement.style.display = DisplayStyle.None;
            }
        });
#endif
        // this has to be called later, after labelElement got its new width calculated
        input.RegisterCallback<GeometryChangedEvent>(evt => {
            label.style.width = labelElement.style.width;
        });
    }

    /// <summary>
    /// Adjusts the class list of the contained input fields.
    /// Since they are now used inline instead of standalone.
    /// </summary>
    private static void SetupClassListForCompositeInput(VisualElement container) {
        var first = true;
        foreach(var field in container.Children()) {
            field.RemoveFromClassList(BaseField<object>.alignedFieldUssClassName); // prevent auto alignment (class = "unity-base-field__aligned")
            field.AddToClassList(Vector3Field.fieldUssClassName); // flex-grow: 1, flex-basis: 0 (class = "unity-composite-field__field")
            field[0]?.AddToClassList(Vector3Field.fieldUssClassName); // (class = "unity-composite-field__field")
            if(first) {
                field[0]?.AddToClassList(Vector3Field.firstFieldVariantUssClassName); // no margin-left (class = "unity-composite-field__field--first")
                first = false;
            }
        }
    }

    /// <summary>
    /// Removes the alignment class from all (nested) children.
    /// </summary>
    private static void RemoveInnerAlignment(VisualElement container) {
        foreach(var field in container.Children()) {
            field.RemoveFromClassList(BaseField<object>.alignedFieldUssClassName); // prevent auto alignment (class = "unity-base-field__aligned")
            RemoveInnerAlignment(field);
        }
    }
}
