using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(MinMaxSliderAttribute))]
public class MinMaxSliderDrawer : PropertyDrawer {

    #region UI Toolkit implementation

    public override VisualElement CreatePropertyGUI(SerializedProperty property) {
        // read the serialized data
        var minMaxSliderAttribute = (MinMaxSliderAttribute)attribute;
        SerializedProperty minProperty = property.FindPropertyRelative(nameof(MinMax<float>.Min));
        SerializedProperty maxProperty = property.FindPropertyRelative(nameof(MinMax<float>.Max));

        // Create property container element
        var input = new VisualElement();

        // property fields
        var slider = new MinMaxSlider(minProperty.floatValue, maxProperty.floatValue, minMaxSliderAttribute.MinValue, minMaxSliderAttribute.MaxValue);
        var min = new PropertyField(minProperty, "");
        var max = new PropertyField(maxProperty, "");
        slider.style.flexGrow = 1f;
        min.style.flexBasis = 50f; // Set a fixed base width independent of the property's value
        max.style.flexBasis = 50f;

        // Add fields to the container
        input.Add(min);
        input.Add(slider);
        input.Add(max);
        var container = new GenericField<MinMax<float>>(property.displayName, input);

        // react to a change from the slider
        _ = slider.RegisterValueChangedCallback(evt => {
            // except when triggered by the float fields (code below)
            if(minProperty.floatValue == evt.newValue.x && maxProperty.floatValue == evt.newValue.y) return;

            // update the serialized float fields
            minProperty.floatValue = evt.newValue.x;
            maxProperty.floatValue = evt.newValue.y;
            var res = maxProperty.serializedObject.ApplyModifiedProperties();
        });

        // react to a change from the min float field
        min.RegisterValueChangeCallback(evt => {
            // clamp the min value between the slider limits and also the max value
            var clamped = Mathf.Clamp(evt.changedProperty.floatValue, slider.lowLimit, Mathf.Min(slider.highLimit, maxProperty.floatValue));
            if(clamped != minProperty.floatValue) {
                minProperty.floatValue = clamped;
                _ = minProperty.serializedObject.ApplyModifiedProperties();
            }

            // update the slider
            slider.value = new Vector2(minProperty.floatValue, maxProperty.floatValue);
        });

        // react to a change from the max float field
        max.RegisterValueChangeCallback(evt => {
            // clamp the max value between the slider limits and also the min value
            var clamped = Mathf.Clamp(evt.changedProperty.floatValue, Mathf.Max(slider.lowLimit, minProperty.floatValue), slider.highLimit);
            if(clamped != maxProperty.floatValue) {
                maxProperty.floatValue = clamped;
                _ = maxProperty.serializedObject.ApplyModifiedProperties();
            }

            // update the slider
            slider.value = new Vector2(minProperty.floatValue, maxProperty.floatValue);
        });

        return container;
    }

    #endregion

    #region IMGUI implementation

    /// <summary>
    /// Fallback to the IMGUI version when used within a custom IMGUI editor
    /// </summary>
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return base.GetPropertyHeight(property.FindPropertyRelative(nameof(MinMax<float>.Min)), label);
    }

    /// <summary>
    /// Fallback to the IMGUI version when used within a custom IMGUI editor
    /// </summary>
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label) {
        _ = EditorGUI.BeginProperty(rect, label, property);

        var minMaxSliderAttribute = (MinMaxSliderAttribute)attribute;

        _ = EditorGUI.BeginProperty(rect, label, property);

        Rect indentRect = EditorGUI.IndentedRect(rect);
        float indentLength = indentRect.x - rect.x;
        const float horizontalSpacing = 2.0f;
        float labelWidth = EditorGUIUtility.labelWidth + horizontalSpacing;
        float floatFieldWidth = EditorGUIUtility.fieldWidth;
        float sliderWidth = rect.width - labelWidth - 2.0f * floatFieldWidth;
        float sliderPadding = 5.0f;

        var labelRect = new Rect(
            rect.x,
            rect.y,
            labelWidth,
            rect.height);

        var sliderRect = new Rect(
            rect.x + labelWidth + floatFieldWidth + sliderPadding - indentLength,
            rect.y,
            sliderWidth - 2.0f * sliderPadding + indentLength,
            rect.height);

        var minFloatFieldRect = new Rect(
            rect.x + labelWidth - indentLength,
            rect.y,
            floatFieldWidth + indentLength,
            rect.height);

        var maxFloatFieldRect = new Rect(
            rect.x + labelWidth + floatFieldWidth + sliderWidth - indentLength,
            rect.y,
            floatFieldWidth + indentLength,
            rect.height);

        // Draw the label
        EditorGUI.LabelField(labelRect, label.text);

        // Draw the slider
        EditorGUI.BeginChangeCheck();

        SerializedProperty minProperty = property.FindPropertyRelative(nameof(MinMax<float>.Min));
        SerializedProperty maxProperty = property.FindPropertyRelative(nameof(MinMax<float>.Max));

        Vector2 sliderValue = new(minProperty.floatValue, maxProperty.floatValue);
        EditorGUI.MinMaxSlider(sliderRect, ref sliderValue.x, ref sliderValue.y, minMaxSliderAttribute.MinValue, minMaxSliderAttribute.MaxValue);

        sliderValue.x = EditorGUI.FloatField(minFloatFieldRect, sliderValue.x);
        sliderValue.x = Mathf.Clamp(sliderValue.x, minMaxSliderAttribute.MinValue, Mathf.Min(minMaxSliderAttribute.MaxValue, sliderValue.y));

        sliderValue.y = EditorGUI.FloatField(maxFloatFieldRect, sliderValue.y);
        sliderValue.y = Mathf.Clamp(sliderValue.y, Mathf.Max(minMaxSliderAttribute.MinValue, sliderValue.x), minMaxSliderAttribute.MaxValue);

        if(EditorGUI.EndChangeCheck()) {
            minProperty.floatValue = sliderValue.x;
            maxProperty.floatValue = sliderValue.y;
        }

        EditorGUI.EndProperty();

        EditorGUI.EndProperty();
    }
    #endregion
}
