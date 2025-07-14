using UnityEngine;

/// <summary>
/// A pose in world space, consisting of a position and a rotation.<br/>
/// Like a lightweight <see cref="Transform"/> without scaling, parent, children, component, etc.<br/>
/// Contains the same data as a <see cref="LocalPose"/>, but is semantically different.
/// </summary>
[System.Serializable]
public struct WorldPose : System.IEquatable<WorldPose> {

    #region Data

    /// <summary>
    /// The world space position of the pose.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Following Unity name")]
    public Vector3 position;

    /// <summary>
    /// The world space rotation of the pose.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Following Unity name")]
    public Quaternion rotation;


    /// <summary>
    /// Constructs a new pose from the given position and rotation.
    /// </summary>
    /// <param name="position">The world space position of the pose.</param>
    /// <param name="rotation">The world space rotation of the pose.</param>
    public WorldPose(Vector3 position, Quaternion rotation) {
        this.position = position;
        this.rotation = rotation;
    }

    #endregion
    #region Properties

    private static readonly WorldPose IdentityPose = new(Vector3.zero, Quaternion.identity);
    /// <summary>
    /// A pose with zero position, and an identity rotation.
    /// </summary>
    public static WorldPose Identity => IdentityPose;

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
        return obj is WorldPose pose && Equals(pose);
    }

    public readonly bool Equals(WorldPose other) {
        return position == other.position && rotation == other.rotation;
    }

    public override int GetHashCode() {
        return position.GetHashCode() ^ (rotation.GetHashCode() << 1);
    }

    public static bool operator ==(WorldPose a, WorldPose b) => a.Equals(b);

    public static bool operator !=(WorldPose a, WorldPose b) => !(a == b);

    #endregion
    #region Implicit conversions

    public static implicit operator WorldPose(Transform transform) => new(transform.position, transform.rotation);

    public static implicit operator WorldPose(Rigidbody rigidbody) => new(rigidbody.position, rigidbody.rotation);

    #endregion
    #region Transformations

    /// <summary>
    /// Transforms the given <paramref name="position"/> from local space to world space.<br/>
    /// Analogous to Transform.TransformPoint.
    /// </summary>
    /// <param name="position">The position that gets transformed</param>
    /// <returns>The transformed position</returns>
    public readonly Vector3 TransformPoint(Vector3 position) {
        return this.position + rotation * position;
    }

    /// <summary>
    /// Transforms the given <paramref name="position"/> from world space to local space.<br/>
    /// Analogous to Transform.InverseTransformPoint.
    /// </summary>
    /// <param name="position">The position that gets transformed</param>
    /// <returns>The inversely transformed position</returns>
    public readonly Vector3 InverseTransformPoint(Vector3 position) {
        return Quaternion.Inverse(rotation) * (position - this.position);
    }

    /// <summary>
    /// Transforms a <see cref="LocalPose"/> (from local to world space).<br/>
    /// Analogous to Transform.TransformPose.
    /// </summary>
    /// <param name="poseToTransform">The pose that gets transformed</param>
    /// <returns>A new <see cref="WorldPose"/> representing the transformed <paramref name="poseToTransform"/></returns>
    public readonly WorldPose TransformPose(LocalPose poseToTransform) {
        return new WorldPose {
            position = TransformPoint(poseToTransform.position),
            rotation = rotation * poseToTransform.rotation
        };
    }

    /// <summary>
    /// Inverse transforms a <see cref="WorldPose"/> (from world to local space).<br/>
    /// Analogous to Transform.InverseTransformPose.
    /// </summary>
    /// <param name="poseToTransform">The pose that gets transformed</param>
    /// <returns>A new <see cref="LocalPose"/> representing the inversely transformed <paramref name="poseToTransform"/></returns>
    public readonly LocalPose InverseTransformPose(WorldPose poseToTransform) {
        return new LocalPose {
            position = InverseTransformPoint(poseToTransform.position),
            rotation = Quaternion.Inverse(rotation) * poseToTransform.rotation
        };
    }

    /// <summary>
    /// Takes the world space offset of this pose to the <paramref name="from"/> transform
    /// and applies it in the coordinate system of the <paramref name="to"/> transform.<br/>
    /// Conceptually the same as pose.SetParent(from, worldPositionStays: true); pose.SetParent(to, worldPositionStays: false).
    /// </summary>
    /// <param name="from">The transform to interpret the pose relative to</param>
    /// <param name="to">The transform under which the relative pose gets applied</param>
    public readonly WorldPose TransformFromTo(Transform from, Transform to) {
        LocalPose offset = from.InverseTransformPose(this);
        return to.TransformPose(offset);
    }

    /// <summary>
    /// Takes the world space offset of this pose to the <paramref name="from"/> pose
    /// and applies it in the coordinate system of the <paramref name="to"/> pose.<br/>
    /// Conceptually the same as pose.SetParent(from, worldPositionStays: true); pose.SetParent(to, worldPositionStays: false).
    /// </summary>
    /// <param name="from">The reference to interpret the pose relative to</param>
    /// <param name="to">The space in which the relative pose gets applied</param>
    public readonly WorldPose TransformFromTo(WorldPose from, WorldPose to) {
        LocalPose offset = from.InverseTransformPose(this);
        return to.TransformPose(offset);
    }

    /// <summary>
    /// Reconstructs a world space <see cref="WorldPose"/> so that a child of it with the given local pose would have the given world pose.<br/>
    /// I.e. result.TransformPose(childLocalPose) == childWorldPose.
    /// </summary>
    /// <param name="childWorldPose">The world space pose of the child</param>
    /// <param name="childLocalPose">The local space pose of the child</param>
    /// <returns>The world space pose of the parent</returns>
    public readonly WorldPose ReconstructParentPoseFromChildPose(WorldPose childWorldPose, LocalPose childLocalPose) {
        var rotation = childWorldPose.rotation * Quaternion.Inverse(childLocalPose.rotation);
        var position = childWorldPose.position - rotation * childLocalPose.position;
        return new WorldPose(position, rotation);
    }

    #endregion
    #region Interpolation

    /// <summary>
    /// Interpolates between two <see cref="WorldPose"/>s.<br/>
    /// Using position.Lerp and rotation.Slerp.
    /// </summary>
    /// <param name="a">The start pose, returned when <paramref name="t"/> <= 0</param>
    /// <param name="b">The end pose, returned when <paramref name="t"/> >= 1</param>
    /// <param name="t">Value used to interpolate between <paramref name="a"/> and <paramref name="b"/></param>
    /// <returns>The interpolated pose</returns>
    public static WorldPose Lerp(WorldPose a, WorldPose b, float t) {
        return new WorldPose(Vector3.Lerp(a.position, b.position, t), Quaternion.Slerp(a.rotation, b.rotation, t));
    }

    #endregion
}
