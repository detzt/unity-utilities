/*
MIT License

Copyright (c) 2017 Mathieu Le Ber

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(Map<,>), true)]
public class MapPropertyDrawer : PropertyDrawer {
    private const string KeysFieldName = "keys";
    private const string ValuesFieldName = "values";
    protected const float IndentWidth = 15f;
    private static readonly GUIContent IconPlus = IconContent("Toolbar Plus", "Add entry");
    private static readonly GUIContent IconMinus = IconContent("Toolbar Minus", "Remove entry");
    private static readonly GUIContent WarningIconConflict = IconContent("console.warnicon.sml", "Conflicting key, this entry will be lost");
    private static readonly GUIContent WarningIconOther = IconContent("console.infoicon.sml", "Conflicting key");
    private static readonly GUIContent WarningIconNull = IconContent("console.warnicon.sml", "Null key, this entry will be lost");
    private static readonly GUIStyle ButtonStyle = GUIStyle.none;

    private class ConflictState {
        public object ConflictKey = null;
        public object ConflictValue = null;
        public int ConflictIndex = -1;
        public int ConflictOtherIndex = -1;
        public bool ConflictKeyPropertyExpanded = false;
        public bool ConflictValuePropertyExpanded = false;
        public float ConflictLineHeight = 0f;
    }

    private struct PropertyIdentity {
        public Object Instance; // UnityEngine.Object
        public string PropertyPath;

        public PropertyIdentity(SerializedProperty property) {
            Instance = property.serializedObject.targetObject;
            PropertyPath = property.propertyPath;
        }
    }

    private static readonly Dictionary<PropertyIdentity, ConflictState> ConflictStateDict = new();

    private enum Action {
        None,
        Add,
        Remove
    }


    #region UI Toolkit implementation

    private Foldout foldout;

    public VisualElement CreatePropertyGUI_WIP(SerializedProperty property) {
        // Create property container element
        var container = new VisualElement();
        container.styleSheets.Add(Resources.Load<StyleSheet>("EditorStyles"));
        container.AddToClassList("map");

        var keyArrayProperty = property.FindPropertyRelative(KeysFieldName);
        var valueArrayProperty = property.FindPropertyRelative(ValuesFieldName);

        ConflictState conflictState = GetConflictState(property);

        if(conflictState.ConflictIndex != -1) {
            keyArrayProperty.InsertArrayElementAtIndex(conflictState.ConflictIndex);
            var keyProperty = keyArrayProperty.GetArrayElementAtIndex(conflictState.ConflictIndex);
            SetPropertyValue(keyProperty, conflictState.ConflictKey);
            keyProperty.isExpanded = conflictState.ConflictKeyPropertyExpanded;

            if(valueArrayProperty != null) {
                valueArrayProperty.InsertArrayElementAtIndex(conflictState.ConflictIndex);
                var valueProperty = valueArrayProperty.GetArrayElementAtIndex(conflictState.ConflictIndex);
                SetPropertyValue(valueProperty, conflictState.ConflictValue);
                valueProperty.isExpanded = conflictState.ConflictValuePropertyExpanded;
            }
        }

        // var list = new ListView {
        //     makeItem = () => {
        //         // Create property fields
        //         var keyField = new PropertyField(keyArrayProperty.GetArrayElementAtIndex(0), "");
        //         var valueField = new PropertyField(valueArrayProperty.GetArrayElementAtIndex(0), "");

        //         // Add fields to the container
        //         var innerContainer = new GenericField<System.Type>(keyField, valueField, setupCompositeInput: true);
        //         return innerContainer;
        //     },
        //     bindItem = (e, i) => {
        //         var innerContainer = (GenericField<System.Type>)e;
        //         var keyField = (PropertyField)innerContainer[2];
        //         var valueField = (PropertyField)innerContainer[1];
        //         keyField.Bind(keyArrayProperty.GetArrayElementAtIndex(i).serializedObject);
        //         valueField.Bind(valueArrayProperty.GetArrayElementAtIndex(i).serializedObject);
        //     },
        //     itemsSource = EnumerateEntries(keyArrayProperty, valueArrayProperty).ToList(),
        // };
        // container.Add(list);

        foldout = new Foldout {
            text = property.displayName
        };
        foldout.AddToClassList("map__foldout");
        container.Add(foldout);

        var addButton = new Button(() => {
            AddEntry(property, keyArrayProperty, valueArrayProperty, keyArrayProperty.arraySize);
            _ = property.serializedObject.ApplyModifiedProperties();
            foldout.Add(CreateEntryGUI(keyArrayProperty, valueArrayProperty, keyArrayProperty.arraySize - 1));
            foldout.MarkDirtyRepaint();
        }) {
            text = "+"
        };
        addButton.AddToClassList("map__add-button");
        foldout.RegisterCallback<GeometryChangedEvent>(evt => {
            if(addButton.parent != null) return; // only once
            var foldoutContainer = foldout.Q(null, Foldout.inputUssClassName); // (class = "unity-foldout__input")
            foldoutContainer?.Add(addButton);
        });

        FillFoldout(keyArrayProperty, valueArrayProperty);

        // // Create property fields
        // var propertyField = new PropertyField(property);
        // container.Add(propertyField);

        container.RegisterCallback<GeometryChangedEvent>(CompleteSetup);
        return container;
    }

    /// <summary>
    /// Repopulates the foldout with entries from the key and value arrays.
    /// </summary>
    private void FillFoldout(SerializedProperty keyArrayProperty, SerializedProperty valueArrayProperty) {
        foldout.Clear();
        for(int i = 0; i < keyArrayProperty.arraySize; i++) {
            foldout.Add(CreateEntryGUI(keyArrayProperty, valueArrayProperty, i));
        }
    }

    private VisualElement CreateEntryGUI(SerializedProperty keyArrayProperty, SerializedProperty valueArrayProperty, int index) {
        var keyProperty = keyArrayProperty.GetArrayElementAtIndex(index);
        var valueProperty = valueArrayProperty?.GetArrayElementAtIndex(index);

        // Create property fields
        // calling new PropertyField(keyProperty, "") does not work when adding to the foldout later
        var keyField = new PropertyField();
        keyField.BindProperty(keyProperty);
        keyField.label = "";
        var valueField = new PropertyField();
        valueField.BindProperty(valueProperty);
        valueField.label = "";
        keyField.AddToClassList("map__key");
        valueField.AddToClassList("map__value");

        // Add fields to the container
        var innerContainer = new GenericField<System.Type>(keyField, valueField, setupCompositeInput: true);

        // Remove button
        var removeButton = new Button(() => {
            var currentIndex = innerContainer.parent.IndexOf(innerContainer);
            DeleteArrayElementAtIndex(keyArrayProperty, currentIndex);
            if(valueArrayProperty != null)
                DeleteArrayElementAtIndex(valueArrayProperty, currentIndex);
            _ = keyArrayProperty.serializedObject.ApplyModifiedProperties();
            //innerContainer.RemoveFromHierarchy();
            FillFoldout(keyArrayProperty, valueArrayProperty);
        }) {
            text = "-"
        };
        removeButton.AddToClassList("map__remove-button");
        innerContainer.Add(removeButton);

        return innerContainer;
    }

    private void CompleteSetup(GeometryChangedEvent evt) {
        //Debug.Log("CompleteSetup");
        var container = (VisualElement)evt.target;
        container.MarkDirtyRepaint();
    }

    #endregion

    #region IMGUI implementation

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        label = EditorGUI.BeginProperty(position, label, property);

        Action buttonAction = Action.None;
        int buttonActionIndex = 0;

        var keyArrayProperty = property.FindPropertyRelative(KeysFieldName);
        var valueArrayProperty = property.FindPropertyRelative(ValuesFieldName);

        ConflictState conflictState = GetConflictState(property);

        if(conflictState.ConflictIndex != -1) {
            keyArrayProperty.InsertArrayElementAtIndex(conflictState.ConflictIndex);
            var keyProperty = keyArrayProperty.GetArrayElementAtIndex(conflictState.ConflictIndex);
            SetPropertyValue(keyProperty, conflictState.ConflictKey);
            keyProperty.isExpanded = conflictState.ConflictKeyPropertyExpanded;

            if(valueArrayProperty != null) {
                valueArrayProperty.InsertArrayElementAtIndex(conflictState.ConflictIndex);
                var valueProperty = valueArrayProperty.GetArrayElementAtIndex(conflictState.ConflictIndex);
                SetPropertyValue(valueProperty, conflictState.ConflictValue);
                valueProperty.isExpanded = conflictState.ConflictValuePropertyExpanded;
            }
        }

        var buttonWidth = ButtonStyle.CalcSize(IconPlus).x;

        var labelPosition = position;
        labelPosition.height = EditorGUIUtility.singleLineHeight;
        labelPosition.xMax -= ButtonStyle.CalcSize(IconPlus).x;

        _ = EditorGUI.PropertyField(labelPosition, property, label, false);
        // property.isExpanded = EditorGUI.Foldout(labelPosition, property.isExpanded, label);
        if(property.isExpanded) {
            var buttonPosition = position;
            buttonPosition.xMin = buttonPosition.xMax - buttonWidth;
            buttonPosition.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.BeginDisabledGroup(conflictState.ConflictIndex != -1);
            if(GUI.Button(buttonPosition, IconPlus, ButtonStyle)) {
                buttonAction = Action.Add;
                buttonActionIndex = keyArrayProperty.arraySize;
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.indentLevel++;
            var linePosition = position;
            linePosition.y += EditorGUIUtility.singleLineHeight;
            linePosition.xMax -= buttonWidth;

            foreach(var entry in EnumerateEntries(keyArrayProperty, valueArrayProperty)) {
                var keyProperty = entry.KeyProperty;
                var valueProperty = entry.ValueProperty;
                int i = entry.Index;

                float lineHeight = DrawKeyValueLine(keyProperty, valueProperty, linePosition);

                buttonPosition = linePosition;
                buttonPosition.x = linePosition.xMax;
                buttonPosition.height = EditorGUIUtility.singleLineHeight;
                if(GUI.Button(buttonPosition, IconMinus, ButtonStyle)) {
                    buttonAction = Action.Remove;
                    buttonActionIndex = i;
                }

                if(i == conflictState.ConflictIndex && conflictState.ConflictOtherIndex == -1) {
                    var iconPosition = linePosition;
                    iconPosition.size = ButtonStyle.CalcSize(WarningIconNull);
                    GUI.Label(iconPosition, WarningIconNull);
                } else if(i == conflictState.ConflictIndex) {
                    var iconPosition = linePosition;
                    iconPosition.size = ButtonStyle.CalcSize(WarningIconConflict);
                    GUI.Label(iconPosition, WarningIconConflict);
                } else if(i == conflictState.ConflictOtherIndex) {
                    var iconPosition = linePosition;
                    iconPosition.size = ButtonStyle.CalcSize(WarningIconOther);
                    GUI.Label(iconPosition, WarningIconOther);
                }


                linePosition.y += lineHeight;
            }

            EditorGUI.indentLevel--;
        } else if(keyArrayProperty.arraySize == 0) {
            var buttonPosition = position;
            buttonPosition.xMin = buttonPosition.xMax - buttonWidth;
            buttonPosition.height = EditorGUIUtility.singleLineHeight;
            if(GUI.Button(buttonPosition, IconPlus, ButtonStyle)) {
                buttonAction = Action.Add;
                buttonActionIndex = keyArrayProperty.arraySize;
            }
        }

        if(buttonAction == Action.Add) {
            AddEntry(property, keyArrayProperty, valueArrayProperty, buttonActionIndex);
        } else if(buttonAction == Action.Remove) {
            DeleteArrayElementAtIndex(keyArrayProperty, buttonActionIndex);
            if(valueArrayProperty != null)
                DeleteArrayElementAtIndex(valueArrayProperty, buttonActionIndex);
        }

        conflictState.ConflictKey = null;
        conflictState.ConflictValue = null;
        conflictState.ConflictIndex = -1;
        conflictState.ConflictOtherIndex = -1;
        conflictState.ConflictLineHeight = 0f;
        conflictState.ConflictKeyPropertyExpanded = false;
        conflictState.ConflictValuePropertyExpanded = false;

        foreach(var entry1 in EnumerateEntries(keyArrayProperty, valueArrayProperty)) {
            var keyProperty1 = entry1.KeyProperty;
            int i = entry1.Index;
            object keyProperty1Value = GetPropertyValue(keyProperty1);

            if(keyProperty1Value == null) {
                var valueProperty1 = entry1.ValueProperty;
                SaveProperty(keyProperty1, valueProperty1, i, -1, conflictState);
                DeleteArrayElementAtIndex(keyArrayProperty, i);
                if(valueArrayProperty != null)
                    DeleteArrayElementAtIndex(valueArrayProperty, i);

                break;
            }


            foreach(var entry2 in EnumerateEntries(keyArrayProperty, valueArrayProperty, i + 1)) {
                var keyProperty2 = entry2.KeyProperty;
                int j = entry2.Index;
                object keyProperty2Value = GetPropertyValue(keyProperty2);

                if(ComparePropertyValues(keyProperty1Value, keyProperty2Value)) {
                    var valueProperty2 = entry2.ValueProperty;
                    SaveProperty(keyProperty2, valueProperty2, j, i, conflictState);
                    DeleteArrayElementAtIndex(keyArrayProperty, j);
                    if(valueArrayProperty != null)
                        DeleteArrayElementAtIndex(valueArrayProperty, j);

                    goto breakLoops;
                }
            }
        }
    breakLoops:

        EditorGUI.EndProperty();
    }

    private static float DrawKeyValueLine(SerializedProperty keyProperty, SerializedProperty valueProperty, Rect linePosition) {
        float labelWidth = EditorGUIUtility.labelWidth;
        float labelWidthRelative = labelWidth / linePosition.width;

        float keyPropertyHeight = EditorGUI.GetPropertyHeight(keyProperty);
        var keyPosition = linePosition;
        keyPosition.height = keyPropertyHeight;
        keyPosition.width = labelWidth - IndentWidth;
        EditorGUIUtility.labelWidth = keyPosition.width * labelWidthRelative;
        _ = EditorGUI.PropertyField(keyPosition, keyProperty, GUIContent.none, true);

        float valuePropertyHeight = EditorGUI.GetPropertyHeight(valueProperty);
        var valuePosition = linePosition;
        valuePosition.height = valuePropertyHeight;
        valuePosition.xMin += labelWidth;
        EditorGUIUtility.labelWidth = valuePosition.width * labelWidthRelative;
        EditorGUI.indentLevel--;
        _ = EditorGUI.PropertyField(valuePosition, valueProperty, GUIContent.none, true);
        EditorGUI.indentLevel++;

        EditorGUIUtility.labelWidth = labelWidth;

        return Mathf.Max(keyPropertyHeight, valuePropertyHeight);
    }

    #endregion

    private static void AddEntry(SerializedProperty property, SerializedProperty keyArrayProperty, SerializedProperty valueArrayProperty, int index) {
        // add new entry
        keyArrayProperty.InsertArrayElementAtIndex(index);
        valueArrayProperty?.InsertArrayElementAtIndex(index);

        // auto increment key
        var newEntry = keyArrayProperty.GetArrayElementAtIndex(index);
        if(index > 0 && IsIntValue(newEntry.propertyType))
            newEntry.intValue++;

        // automatically expand upon adding first entry
        if(index == 0)
            property.isExpanded = true;
    }

    private static void SaveProperty(SerializedProperty keyProperty, SerializedProperty valueProperty, int index, int otherIndex, ConflictState conflictState) {
        conflictState.ConflictKey = GetPropertyValue(keyProperty);
        conflictState.ConflictValue = valueProperty != null ? GetPropertyValue(valueProperty) : null;
        float keyPropertyHeight = EditorGUI.GetPropertyHeight(keyProperty);
        float valuePropertyHeight = valueProperty != null ? EditorGUI.GetPropertyHeight(valueProperty) : 0f;
        float lineHeight = Mathf.Max(keyPropertyHeight, valuePropertyHeight);
        conflictState.ConflictLineHeight = lineHeight;
        conflictState.ConflictIndex = index;
        conflictState.ConflictOtherIndex = otherIndex;
        conflictState.ConflictKeyPropertyExpanded = keyProperty.isExpanded;
        conflictState.ConflictValuePropertyExpanded = valueProperty != null && valueProperty.isExpanded;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        float propertyHeight = EditorGUIUtility.singleLineHeight;

        if(property.isExpanded) {
            var keysProperty = property.FindPropertyRelative(KeysFieldName);
            var valuesProperty = property.FindPropertyRelative(ValuesFieldName);

            foreach(var entry in EnumerateEntries(keysProperty, valuesProperty)) {
                var keyProperty = entry.KeyProperty;
                var valueProperty = entry.ValueProperty;
                float keyPropertyHeight = EditorGUI.GetPropertyHeight(keyProperty);
                float valuePropertyHeight = valueProperty != null ? EditorGUI.GetPropertyHeight(valueProperty) : 0f;
                float lineHeight = Mathf.Max(keyPropertyHeight, valuePropertyHeight);
                propertyHeight += lineHeight;
            }

            ConflictState conflictState = GetConflictState(property);

            if(conflictState.ConflictIndex != -1) {
                propertyHeight += conflictState.ConflictLineHeight;
            }
        }

        return propertyHeight;
    }

    private static ConflictState GetConflictState(SerializedProperty property) {
        PropertyIdentity propId = new(property);
        if(!ConflictStateDict.TryGetValue(propId, out ConflictState conflictState)) {
            conflictState = new ConflictState();
            ConflictStateDict.Add(propId, conflictState);
        }
        return conflictState;
    }

    private static readonly Dictionary<SerializedPropertyType, PropertyInfo> SerializedPropertyValueAccessorsDict;

    static MapPropertyDrawer() {
        Dictionary<SerializedPropertyType, string> serializedPropertyValueAccessorsNameDict = new() {
            { SerializedPropertyType.Integer, "intValue" },
            { SerializedPropertyType.Boolean, "boolValue" },
            { SerializedPropertyType.Float, "floatValue" },
            { SerializedPropertyType.String, "stringValue" },
            { SerializedPropertyType.Color, "colorValue" },
            { SerializedPropertyType.ObjectReference, "objectReferenceValue" },
            { SerializedPropertyType.LayerMask, "intValue" },
            { SerializedPropertyType.Enum, "intValue" },
            { SerializedPropertyType.Vector2, "vector2Value" },
            { SerializedPropertyType.Vector3, "vector3Value" },
            { SerializedPropertyType.Vector4, "vector4Value" },
            { SerializedPropertyType.Rect, "rectValue" },
            { SerializedPropertyType.ArraySize, "intValue" },
            { SerializedPropertyType.Character, "intValue" },
            { SerializedPropertyType.AnimationCurve, "animationCurveValue" },
            { SerializedPropertyType.Bounds, "boundsValue" },
            { SerializedPropertyType.Quaternion, "quaternionValue" },
        };
        System.Type serializedPropertyType = typeof(SerializedProperty);

        SerializedPropertyValueAccessorsDict = new Dictionary<SerializedPropertyType, PropertyInfo>();
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;

        foreach(var kvp in serializedPropertyValueAccessorsNameDict) {
            PropertyInfo propertyInfo = serializedPropertyType.GetProperty(kvp.Value, flags);
            SerializedPropertyValueAccessorsDict.Add(kvp.Key, propertyInfo);
        }
    }

    private static bool IsIntValue(SerializedPropertyType type) => type switch {
        SerializedPropertyType.Enum => true,
        SerializedPropertyType.Integer => true,
        _ => false,
    };

    private static GUIContent IconContent(string name, string tooltip) {
        var builtinIcon = EditorGUIUtility.IconContent(name);
        return new GUIContent(builtinIcon.image, tooltip);
    }

    private static void DeleteArrayElementAtIndex(SerializedProperty arrayProperty, int index) {
        var property = arrayProperty.GetArrayElementAtIndex(index);
        // if(arrayProperty.arrayElementType.StartsWith("PPtr<$"))
        if(property.propertyType == SerializedPropertyType.ObjectReference) {
            property.objectReferenceValue = null;
        }

        arrayProperty.DeleteArrayElementAtIndex(index);
    }

    public static object GetPropertyValue(SerializedProperty p) {
        return SerializedPropertyValueAccessorsDict.TryGetValue(p.propertyType, out PropertyInfo propertyInfo) ?
            propertyInfo.GetValue(p, null) :
            p.isArray ? GetPropertyValueArray(p) : GetPropertyValueGeneric(p);
    }

    private static void SetPropertyValue(SerializedProperty p, object v) {
        if(SerializedPropertyValueAccessorsDict.TryGetValue(p.propertyType, out PropertyInfo propertyInfo)) {
            propertyInfo.SetValue(p, v, null);
        } else {
            if(p.isArray)
                SetPropertyValueArray(p, v);
            else
                SetPropertyValueGeneric(p, v);
        }
    }

    private static object GetPropertyValueArray(SerializedProperty property) {
        object[] array = new object[property.arraySize];
        for(int i = 0; i < property.arraySize; i++) {
            SerializedProperty item = property.GetArrayElementAtIndex(i);
            array[i] = GetPropertyValue(item);
        }
        return array;
    }

    private static object GetPropertyValueGeneric(SerializedProperty property) {
        Dictionary<string, object> dict = new();
        var iterator = property.Copy();
        if(iterator.Next(true)) {
            var end = property.GetEndProperty();
            do {
                string name = iterator.name;
                object value = GetPropertyValue(iterator);
                dict.Add(name, value);
            } while(iterator.Next(false) && iterator.propertyPath != end.propertyPath);
        }
        return dict;
    }

    private static void SetPropertyValueArray(SerializedProperty property, object v) {
        object[] array = (object[])v;
        property.arraySize = array.Length;
        for(int i = 0; i < property.arraySize; i++) {
            SerializedProperty item = property.GetArrayElementAtIndex(i);
            SetPropertyValue(item, array[i]);
        }
    }

    private static void SetPropertyValueGeneric(SerializedProperty property, object v) {
        var dict = (Dictionary<string, object>)v;
        var iterator = property.Copy();
        if(iterator.Next(true)) {
            var end = property.GetEndProperty();
            do {
                string name = iterator.name;
                SetPropertyValue(iterator, dict[name]);
            } while(iterator.Next(false) && iterator.propertyPath != end.propertyPath);
        }
    }

    private static bool ComparePropertyValues(object value1, object value2) {
        return value1 is Dictionary<string, object> dictionary1 && value2 is Dictionary<string, object> dictionary2 ?
            CompareDictionaries(dictionary1, dictionary2) :
            Equals(value1, value2);
    }

    private static bool CompareDictionaries(Dictionary<string, object> dict1, Dictionary<string, object> dict2) {
        if(dict1.Count != dict2.Count)
            return false;

        foreach(var kvp1 in dict1) {
            var key1 = kvp1.Key;
            object value1 = kvp1.Value;

            if(!dict2.TryGetValue(key1, out object value2))
                return false;

            if(!ComparePropertyValues(value1, value2))
                return false;
        }

        return true;
    }

    private struct EnumerationEntry {
        public SerializedProperty KeyProperty;
        public SerializedProperty ValueProperty;
        public int Index;

        public EnumerationEntry(SerializedProperty keyProperty, SerializedProperty valueProperty, int index) {
            KeyProperty = keyProperty;
            ValueProperty = valueProperty;
            Index = index;
        }
    }

    private static IEnumerable<EnumerationEntry> EnumerateEntries(SerializedProperty keyArrayProperty, SerializedProperty valueArrayProperty, int startIndex = 0) {
        if(keyArrayProperty.arraySize > startIndex) {
            int index = startIndex;
            var keyProperty = keyArrayProperty.GetArrayElementAtIndex(startIndex);
            var valueProperty = valueArrayProperty?.GetArrayElementAtIndex(startIndex);
            var endProperty = keyArrayProperty.GetEndProperty();

            do {
                yield return new EnumerationEntry(keyProperty, valueProperty, index);
                index++;
            } while(keyProperty.Next(false)
                && (valueProperty == null || valueProperty.Next(false))
                && !SerializedProperty.EqualContents(keyProperty, endProperty));
        }
    }
}

public class MapStoragePropertyDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        _ = property.Next(true);
        _ = EditorGUI.PropertyField(position, property, label, true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        _ = property.Next(true);
        return EditorGUI.GetPropertyHeight(property);
    }
}
