using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Serializable Dictionary
/// </summary>
[System.Serializable]
public class OldMap<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver {
    [SerializeField] protected List<TKey> keys = new();
    [SerializeField] protected List<TValue> values = new();

    // save the dictionary to lists
    public void OnBeforeSerialize() {
        keys.Clear();
        values.Clear();
        foreach(KeyValuePair<TKey, TValue> pair in this) {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    // load dictionary from lists
    public void OnAfterDeserialize() {
        Clear();

        if(keys.Count != values.Count)
            throw new System.Exception($"there are {keys.Count} keys and {values.Count} values after deserialization. Make sure that both key and value types are serializable.");

        for(int i = 0; i < keys.Count; i++)
            Add(keys[i], values[i]);
    }
}
