using UnityEngine;

/// <summary>
/// A pose in local space, consisting of a position and a rotation.<br/>
/// Like a lightweight <see cref="Transform"/> without scaling, parent, children, component, etc.<br/>
/// Contains the same data as a <see cref="WorldPose"/>, but is semantically different.
/// </summary>
[System.Serializable]
public struct LocalPose : System.IEquatable<LocalPose> {

    #region Data

    /// <summary>
    /// The local position of the pose.
    /// </summary>
    public Vector3 position;

    /// <summary>
    /// The local rotation of the pose.
    /// </summary>
    public Quaternion rotation;


    /// <summary>
    /// Constructs a new pose from the given position and rotation.
    /// </summary>
    /// <param name="position">The local position of the pose.</param>
    /// <param name="rotation">The local rotation of the pose.</param>
    public LocalPose(Vector3 position, Quaternion rotation) {
        this.position = position;
        this.rotation = rotation;
    }

    #endregion
    #region Properties

    private static readonly LocalPose identityPose = new(Vector3.zero, Quaternion.identity);
    /// <summary>
    /// A pose with zero position, and an identity rotation.
    /// </summary>
    public static LocalPose Identity => identityPose;

    /// <summary>
    /// The forward vector of the pose.
    /// </summary>
    public readonly Vector3 Forward => rotation * Vector3.forward;

    /// <summary>
    /// The right vector of the pose.
    /// </summary>
    public readonly Vector3 Right => rotation * Vector3.right;

    /// <summary>
    /// The up vector of the pose.
    /// </summary>
    public readonly Vector3 Up => rotation * Vector3.up;

    #endregion
    #region Struct essentials

    public override readonly string ToString() {
        return $"({position}, {rotation})";
    }

    public override readonly bool Equals(object obj) {
        return obj is LocalPose pose && Equals(pose);
    }

    public readonly bool Equals(LocalPose other) {
        return position == other.position && rotation == other.rotation;
    }

    public override int GetHashCode() {
        return position.GetHashCode() ^ (rotation.GetHashCode() << 1);
    }

    public static bool operator ==(LocalPose a, LocalPose b) => a.Equals(b);

    public static bool operator !=(LocalPose a, LocalPose b) => !(a == b);

    #endregion
    #region Implicit conversions

    public static implicit operator LocalPose(Transform transform) => new(transform.localPosition, transform.localRotation);

    #endregion
    #region Interpolation

    /// <summary>
    /// Interpolates between two <see cref="LocalPose"/>s.<br/>
    /// Using position.Lerp and rotation.Slerp.
    /// </summary>
    /// <param name="a">The start pose, returned when <paramref name="t"/> <= 0</param>
    /// <param name="b">The end pose, returned when <paramref name="t"/> >= 1</param>
    /// <param name="t">Value used to interpolate between <paramref name="a"/> and <paramref name="b"/></param>
    /// <returns>The interpolated pose</returns>
    public static LocalPose Lerp(LocalPose a, LocalPose b, float t) {
        return new LocalPose(Vector3.Lerp(a.position, b.position, t), Quaternion.Slerp(a.rotation, b.rotation, t));
    }

    #endregion
}
