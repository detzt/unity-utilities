using NaughtyAttributes.Editor;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(MinMaxSliderAttribute))]
public class MinMaxSliderDrawer : PropertyDrawerBase {
    protected override float GetPropertyHeight_Internal(SerializedProperty property, GUIContent label) {
        return GetPropertyHeight(property.FindPropertyRelative(nameof(MinMax<float>.Min)));
    }

    protected override void OnGUI_Internal(Rect rect, SerializedProperty property, GUIContent label) {
        _ = EditorGUI.BeginProperty(rect, label, property);

        var minMaxSliderAttribute = (MinMaxSliderAttribute)attribute;

        _ = EditorGUI.BeginProperty(rect, label, property);

        float indentLength = NaughtyEditorGUI.GetIndentLength(rect);
        float labelWidth = EditorGUIUtility.labelWidth + NaughtyEditorGUI.HorizontalSpacing;
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
}
