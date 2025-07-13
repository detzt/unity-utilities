using System;

/// <summary>
/// Caches a value and recomputes it when the validation function returns false.
/// <example><code>
/// CachedValue&lt;int&gt; cachedInt = new CachedValue&lt;int&gt;(() =&gt; {
///     int value = ComputeExpensiveValue();
///     var frameCount = Time.frameCount;
///     return (value, () =&gt; Time.frameCount == frameCount);
/// });
/// </code></example>
/// </summary>
/// <typeparam name="T">The type of the value to cache.</typeparam>
public class CachedValue<T> {
    /// <summary>A factory function that returns a tuple containing the current value and a validation function to check if that value is still valid.</summary>
    private readonly Func<(T Value, Func<bool> IsStillValid)> valueFactory;

    /// <summary>The cached value.</summary>
    private T cachedValue = default;
    /// <summary>A function that checks if the cached value is still valid.</summary>
    private Func<bool> isStillValid = () => false;

    public CachedValue(Func<(T, Func<bool>)> valueFactory) {
        this.valueFactory = valueFactory;
    }

    /// <summary>
    /// Returns the cached value if it is still valid, otherwise refreshes the value and returns the new cached value.
    /// </summary>
    public T Value {
        get {
            if(!isStillValid()) {
                Refresh();
            }
            return cachedValue;
        }
    }

    /// <summary>
    /// Forces a refresh of the cached value, regardless of its validity.
    /// </summary>
    public void Refresh() {
        (cachedValue, isStillValid) = valueFactory();
    }

    /// <summary>
    /// Implicitly converts the CachedValue to its underlying type.
    /// </summary>
    public static explicit operator T(CachedValue<T> cachedValue) => cachedValue.Value;
}
