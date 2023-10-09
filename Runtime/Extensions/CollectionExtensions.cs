/// <summary>
/// Functionality extensions on the Collection classes
/// </summary>
public static class CollectionExtensions {

    /// <summary>Returns whether the given <paramref name="index"/> is within the bounds of this array</summary>
    public static bool IsValidIndex(this System.Array a, int index) {
        return 0 <= index && index < a.Length;
    }
}
