using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Functionality extensions to Unity class(es)
/// </summary>
public static class UnityExtensions {

    /// <summary>Returns whether a component of type <typeparamref name="T"/> is attached to the GameObject.</summary>
    public static bool HasComponent<T>(this GameObject self) where T : Component => self.TryGetComponent<T>(out _);

    /// <summary>Returns whether a component of type <typeparamref name="T"/> is attached to the GameObject.</summary>
    public static bool HasComponent<T>(this Component self) where T : Component => self.TryGetComponent<T>(out _);

    /// <summary>Adds a component of type <typeparamref name="T"/> if one doesn't already exist and returns it.</summary>
    public static T GetOrAddComponent<T>(this GameObject self) where T : Component {
        return self.TryGetComponent(out T component) ? component : self.AddComponent<T>();
    }

    /// <summary>Returns whether a component of type <typeparamref name="T"/> is attached to the GameObject or any of its parents.</summary>
    public static bool TryGetComponentInParent<T>(this GameObject self, out T component) where T : Component {
        component = self.GetComponentInParent<T>();
        return component != null;
    }

    /// <summary>Returns whether a component of type <typeparamref name="T"/> is attached to the GameObject or any of its parents.</summary>
    public static bool TryGetComponentInParent<T>(this Component self, out T component) where T : Component {
        component = self.GetComponentInParent<T>();
        return component != null;
    }

    /// <summary>Returns whether a component of type <typeparamref name="T"/> is attached to the GameObject or any of its children.</summary>
    public static bool TryGetComponentInChildren<T>(this GameObject self, out T component) where T : Component {
        component = self.GetComponentInChildren<T>();
        return component != null;
    }

    /// <summary>Returns whether a component of type <typeparamref name="T"/> is attached to the GameObject or any of its children.</summary>
    public static bool TryGetComponentInChildren<T>(this Component self, out T component) where T : Component {
        component = self.GetComponentInChildren<T>();
        return component != null;
    }

    /// <summary>Gets references to all components of type <typeparamref name="T"/> on any direct children, without itself.</summary>
    public static List<T> GetComponentsInOnlyChildren<T>(this Component self, bool includeInactive = false) where T : Component {
        var res = new List<T>();
        foreach(Transform child in self.transform) {
            if(includeInactive || child.gameObject.activeInHierarchy)
                res.AddRange(child.GetComponents<T>());
        }
        return res;
    }

    /// <summary>Gets references to all components of type <typeparamref name="T"/> on any cousin, sibling, and self.</summary>
    public static List<T> GetComponentsInOnlyCousins<T>(this Component self, bool includeInactive = false) where T : Component {
        var parent = self.transform.parent;
        if(parent == null) return null;
        var grandparent = parent.parent;
        if(grandparent == null) return null;
        var res = new List<T>();

        foreach(Transform uncle in grandparent) {
            res.AddRange(uncle.GetComponentsInOnlyChildren<T>(includeInactive));
        }
        return res;
    }

    /// <summary>Sets the world space position and rotation of the transform to the given <paramref name="reference"/>.</summary>
    public static void SetPositionAndRotation(this Transform self, Transform reference) {
        self.SetPositionAndRotation(reference.position, reference.rotation);
    }


    /// <summary>Sets the world space position and rotation of the transform to the given <paramref name="pose"/>.</summary>
    /// <param name="self">The transform to set the position and rotation of</param>
    /// <param name="pose">The world space position and rotation to set</param>
    public static void SetPositionAndRotation(this Transform self, WorldPose pose) {
        self.SetPositionAndRotation(pose.position, pose.rotation);
    }

    /// <summary>Sets the local position and local rotation of the transform to the given <paramref name="pose"/>.</summary>
    /// <param name="self">The transform to set the local position and local rotation of</param>
    /// <param name="pose">The local position and local rotation to set</param>
    public static void SetLocalPositionAndRotation(this Transform self, LocalPose pose) {
        self.SetLocalPositionAndRotation(pose.position, pose.rotation);
    }


    /// <summary>
    /// Transforms a <see cref="LocalPose"/> (from local to world space).<br/>
    /// Analogous to Transform.TransformPoint.
    /// </summary>
    /// <param name="self">The transform that is used to transform the pose</param>
    /// <param name="poseToTransform">The pose that gets transformed</param>
    /// <returns>A new <see cref="WorldPose"/> representing the transformed <paramref name="poseToTransform"/></returns>
    public static WorldPose TransformPose(this Transform self, LocalPose poseToTransform) {
        return new WorldPose {
            position = self.TransformPoint(poseToTransform.position),
            rotation = self.rotation * poseToTransform.rotation
        };
    }

    /// <summary>
    /// Inverse transforms a <see cref="WorldPose"/> (from world to local space).<br/>
    /// Analogous to Transform.InverseTransformPoint.
    /// </summary>
    /// <param name="self">The transform that is used to transform the pose</param>
    /// <param name="poseToTransform">The pose that gets transformed</param>
    /// <returns>A new <see cref="LocalPose"/> representing the inversely transformed <paramref name="poseToTransform"/></returns>
    public static LocalPose InverseTransformPose(this Transform self, WorldPose poseToTransform) {
        return new LocalPose {
            position = self.InverseTransformPoint(poseToTransform.position),
            rotation = Quaternion.Inverse(self.rotation) * poseToTransform.rotation
        };
    }
}
