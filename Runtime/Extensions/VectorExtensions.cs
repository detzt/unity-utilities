using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// Functionality extensions on the Vector class
/// </summary>
public static class VectorExtensions {
    /// <summary>
    /// Returns a copy of this color with modified values for the given components
    /// </summary>
    /// <param name="original">this specifies the class to apply the extension method to</param>
    /// <param name="r">The new r value</param>
    /// <param name="g">The new g value</param>
    /// <param name="b">The new b value</param>
    /// <param name="a">The new a value</param>
    /// <returns>The modified color</returns>
    public static Color With(this Color original, float? r = null, float? g = null, float? b = null, float? a = null) =>
        new(r ?? original.r, g ?? original.g, b ?? original.b, a ?? original.a);

    /// <summary>
    /// Returns a copy of this vector with modified values for the given components
    /// </summary>
    /// <param name="original">this specifies the class to apply the extension method to</param>
    /// <param name="x">The new x value</param>
    /// <param name="y">The new y value</param>
    /// <param name="z">The new z value</param>
    /// <returns>The modified vector</returns>
    public static Vector3 With(this Vector3 original, float? x = null, float? y = null, float? z = null) => new(x ?? original.x, y ?? original.y, z ?? original.z);

    /// <summary>
    /// Adds the given components to this vector
    /// </summary>
    /// <param name="v">this specifies the class to apply the extension method to</param>
    /// <param name="x">The x value to add</param>
    /// <param name="y">The y value to add</param>
    /// <param name="z">The z value to add</param>
    /// <returns>Self for chaining</returns>
    public static Vector3 Add(this Vector3 v, float? x = null, float? y = null, float? z = null) {
        if(x.HasValue) v.x += x.Value;
        if(y.HasValue) v.y += y.Value;
        if(z.HasValue) v.z += z.Value;
        return v;
    }

    /// <summary>
    /// Returns the the given vector <paramref name="v"/> rotated by the given amount in <paramref name="degrees"/>
    /// </summary>
    /// <param name="v">The vector to rotate</param>
    /// <param name="degrees">The amount in degrees to rotate</param>
    public static Vector2 Rotate(this Vector2 v, float degrees) {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = cos * tx - sin * ty;
        v.y = sin * tx + cos * ty;
        return v;
    }

    /// <summary>
    /// Calculates the component-wise division of this vector by the given vector.<br/>
    /// If the division would result in NaN, the component is set to 0.<br/>
    /// Then returns a new vector without modifying the original.
    /// </summary>
    /// <param name="a">The nominator</param>
    /// <param name="b">The denominator</param>
    public static Vector3 Div(this Vector3 a, Vector3 b) {
        var res = new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
        if(float.IsNaN(res.x)) res.x = 0f;
        if(float.IsNaN(res.y)) res.y = 0f;
        if(float.IsNaN(res.z)) res.z = 0f;
        return res;
    }

    /// <summary>
    /// Calculates the component-wise multiplication of this vector by the given vector.<br/>
    /// Then returns a new vector without modifying the original.
    /// </summary>
    /// <param name="a">The first factor</param>
    /// <param name="b">The second factor</param>
    public static Vector3 Mul(this Vector3 a, Vector3 b) => new(a.x * b.x, a.y * b.y, a.z * b.z);

    /// <summary>
    /// Returns the squared distance between this and the other vector
    /// </summary>
    public static float SqrDist(this Vector3 a, Vector3 b) => (a - b).sqrMagnitude;

    /// <summary>
    /// Returns the x and y coordinates as <see cref="Vector2">
    /// </summary>
    public static Vector2 XY(this Vector3 v) => new(v.x, v.y);

    /// <summary>
    /// Returns the x and z coordinates as <see cref="Vector2">
    /// </summary>
    public static Vector2 XZ(this Vector3 v) => new(v.x, v.z);

    /// <summary>
    /// Converts this <see cref="Vector2"/> into a <see cref="Vector3"/> by inserting a 0 in the middle
    /// </summary>
    public static Vector3 X0Y(this Vector2 v) => new(v.x, 0f, v.y);
}

/// <summary>
/// Class containing the vector equivalent operations of <see cref="Mathf"/>
/// </summary>
public static class MathV {

    /* Operators that are applied to every component */

    /// <summary>For each component, takes the absolute value value and returns it as a new vector.</summary>
    public static Vector3 Abs(Vector3 v) => new(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));

    /// <summary>Rounds each component to the nearest integer and returns it as a new vector.</summary>
    public static Vector3 Round(Vector3 v) => new(Mathf.Round(v.x), Mathf.Round(v.y), Mathf.Round(v.z));

    /// <summary>Rounds each component to the given number of decimal places.</summary>
    /// <param name="v">The vector to round</param>
    /// <param name="decimals">The number of digits after the decimal point to keep.</param>
    public static Vector3 Round(Vector3 v, int decimals) {
        return new Vector3((float)decimal.Round((decimal)v.x, decimals), (float)decimal.Round((decimal)v.y, decimals), (float)decimal.Round((decimal)v.z, decimals));
    }

    /// <summary>Returns the component-wise minimum of the two vectors.</summary>
    public static Vector3 Min(Vector3 a, Vector3 b) => new(Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y), Mathf.Min(a.z, b.z));

    /// <summary>Returns the component-wise maximum of the two vectors.</summary>
    public static Vector3 Max(Vector3 a, Vector3 b) => new(Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y), Mathf.Max(a.z, b.z));

    /// <summary>Returns the component-wise maximum of the vector and the given float.</summary>
    public static Vector3 Max(Vector3 a, float b) => new(Mathf.Max(a.x, b), Mathf.Max(a.y, b), Mathf.Max(a.z, b));

    [PublicAPI]
    /// <summary>Returns the component-wise clamped value of <paramref name="v"/> between <paramref name="min"/> and <paramref name="max"/>.</summary>
    public static Vector3 Clamp(Vector3 v, Vector3 min, Vector3 max) => new(Mathf.Clamp(v.x, min.x, max.x), Mathf.Clamp(v.y, min.y, max.y), Mathf.Clamp(v.z, min.z, max.z));

    [PublicAPI]
    /// <summary>Returns a random vector with components between 0 and the given range (both inclusive).</summary>
    public static Vector3 Random(Vector3 range) => new(UnityEngine.Random.Range(0f, range.x), UnityEngine.Random.Range(0f, range.y), UnityEngine.Random.Range(0f, range.z));


    /// <summary> Returns the biggest component of the given vector</summary>
    public static float Max(Vector3 v) => Mathf.Max(v.x, Mathf.Max(v.y, v.z));

    /// <summary> Returns the sum of all components</summary>
    public static float Sum(Vector3 v) => v.x + v.y + v.z;

    /// <summary>Returns the given two floats in ascending order</summary>
    public static (float, float) MinMax(float a, float b) => a <= b ? (a, b) : (b, a);
}
