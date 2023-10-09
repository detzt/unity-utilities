using UnityEngine;

/// <summary>
/// Generic Tuple to store two values
/// </summary>
[System.Serializable]
public struct Couple<T1, T2> {
    [SerializeField] public T1 Item1;
    [SerializeField] public T2 Item2;

    public Couple(T1 item1 = default, T2 item2 = default) { Item1 = item1; Item2 = item2; }
}

/// <summary>
/// Factory to create Couples with type inference
/// </summary>
public static class Couple {
    public static Couple<T1, T2> Create<T1, T2>(T1 item1, T2 item2) => new(item1, item2);
}
