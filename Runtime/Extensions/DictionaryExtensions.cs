using System.Collections.Generic;

/// <summary>
/// Functionality extensions on the Dictionary class(es)
/// </summary>
public static class DictionaryExtensions {

    /// <summary>Returns the value for the given <paramref name="key"/>. If none is stored, a new entry is created first.</summary>
    public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key) where TValue : new() {
        if(dict.TryGetValue(key, out TValue val)) return val;

        val = new TValue();
        dict.Add(key, val);
        return val;
    }

    /// <summary>Allows deconstruction of a key value pair into two variables</summary>
    public static void Deconstruct<TK, TV>(this KeyValuePair<TK, TV> kvp, out TK key, out TV value) {
        key = kvp.Key;
        value = kvp.Value;
    }
}
