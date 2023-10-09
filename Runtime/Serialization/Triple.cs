using UnityEngine;

/// <summary>
/// Generic Tuple to store three values
/// </summary>
[System.Serializable]
public struct Triple<T1, T2, T3> {
    [SerializeField] public T1 Item1;
    [SerializeField] public T2 Item2;
    [SerializeField] public T3 Item3;

    public Triple(T1 item1 = default, T2 item2 = default, T3 item3 = default) {
        Item1 = item1;
        Item2 = item2;
        Item3 = item3;
    }
}

/// <summary>
/// Factory to create Triples with type inference
/// </summary>
public static class Triple {
    public static Triple<T1, T2, T3> Create<T1, T2, T3>(T1 item1, T2 item2, T3 item3) => new(item1, item2, item3);
}
