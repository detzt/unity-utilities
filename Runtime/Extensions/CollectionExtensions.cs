﻿using System.Collections;

/// <summary>
/// Functionality extensions on the Collection classes
/// </summary>
public static class CollectionExtensions {

    /// <summary>Returns whether the given <paramref name="index"/> is within the bounds of this collection</summary>
    public static bool IsValidIndex(this ICollection collection, int index) {
        return 0 <= index && index < collection.Count;
    }
}
