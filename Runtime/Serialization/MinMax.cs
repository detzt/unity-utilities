using UnityEngine;

/// <summary>
/// Generic Template to store a minimal and maximal value of something
/// </summary>
[System.Serializable]
public struct MinMax<T> {
    public MinMax(T min = default, T max = default) { Min = min; Max = max; }
    [SerializeField] public T Min;
    [SerializeField] public T Max;
}

public static class MinMaxExtensions {
    public static float Clamp(this MinMax<float> self, float value) => Mathf.Clamp(value, self.Min, self.Max);
    public static Vector3 Lerp(this MinMax<Vector3> self, float alpha) => Vector3.Lerp(self.Min, self.Max, alpha);
    public static float Lerp(this MinMax<float> self, float alpha) => Mathf.Lerp(self.Min, self.Max, alpha);
    public static float InverseLerp(this MinMax<float> self, float value) => Mathf.InverseLerp(self.Min, self.Max, value);
    public static float Random(this MinMax<float> self) => UnityEngine.Random.Range(self.Min, self.Max);
    public static float MinSqr(this MinMax<float> self) => self.Min * self.Min;
    public static float MaxSqr(this MinMax<float> self) => self.Max * self.Max;
    /// <summary>Returns true iff the given value is at least as large as the minimum and at most as large as the maximum, inclusive.</summary>
    public static bool Contains(this MinMax<float> self, float value) => self.Min <= value && value <= self.Max;
    public static bool Contains(this MinMax<float> self, float value, float tolerance) => self.Min - tolerance <= value && value <= self.Max + tolerance;

    /// <summary>
    /// This lerps between two MinMax structs component wise.
    /// Use the extension method if you want to lerp within a single MinMax struct.
    /// </summary>
    public static MinMax<float> Lerp(MinMax<float> a, MinMax<float> b, float t) {
        return new MinMax<float>(
            Mathf.Lerp(a.Min, b.Min, t),
            Mathf.Lerp(a.Max, b.Max, t)
        );
    }
}
