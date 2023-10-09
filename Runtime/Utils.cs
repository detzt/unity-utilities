using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Collection of utility functions
/// </summary>
public static class Utils {

    /// <summary>
    /// Returns an iterator with all values of the given enum, which can be used in a foreach loop.
    /// </summary>
    /// <typeparam name="TEnum">The enum, whose values to get</typeparam>
    public static IEnumerable<TEnum> EnumValues<TEnum>() where TEnum : System.Enum => (TEnum[])System.Enum.GetValues(typeof(TEnum));


    /// <summary>Logs a warning with the given message and returns the given value, useful in switch expressions</summary>
    public static T ReturnValueAndLogError<T>(T value, string message) {
        Debug.LogError(message);
        return value;
    }
}
