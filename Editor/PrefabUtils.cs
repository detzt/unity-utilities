using UnityEditor;
using UnityEngine;

/// <summary>
/// Collection of utility functions for working with prefabs
/// </summary>
public static class PrefabUtils {

    /// <summary>
    /// Reverts the local rotation and local position if they are identical to its source prefab
    /// </summary>
    /// <param name="transform">The Transform to change</param>
    public static void RevertIdenticalTransformOverrides(Transform transform) {
        var reference = PrefabUtility.GetCorrespondingObjectFromSource(transform);
        if(reference != null) RevertIdenticalTransformOverrides(transform, reference.transform);
    }

    /// <summary>
    /// Reverts the local rotation and local position if they are identical to the given <paramref name="reference"/> Transform
    /// </summary>
    /// <param name="transform">The Transform to change</param>
    /// <param name="reference">Reference Transform</param>
    public static void RevertIdenticalTransformOverrides(Transform transform, Transform reference) {
        using var so = new SerializedObject(transform);
        Undo.RecordObject(transform, "Revert Identity Transforms");
        PrefabUtility.RecordPrefabInstancePropertyModifications(transform);

        var posDiff = transform.localPosition - reference.localPosition;
        if(transform.localRotation == reference.localRotation) {
            PrefabUtility.RevertPropertyOverride(so.FindProperty("m_LocalRotation"), InteractionMode.AutomatedAction);
            PrefabUtility.RevertPropertyOverride(so.FindProperty("m_LocalEulerAnglesHint"), InteractionMode.AutomatedAction);
        } else {
            if(Mathf.Abs(so.FindProperty("m_LocalRotation.x").floatValue - reference.localEulerAngles.x) < float.Epsilon)
                PrefabUtility.RevertPropertyOverride(so.FindProperty("m_LocalRotation.x"), InteractionMode.AutomatedAction);
            if(Mathf.Abs(so.FindProperty("m_LocalRotation.y").floatValue - reference.localEulerAngles.y) < float.Epsilon)
                PrefabUtility.RevertPropertyOverride(so.FindProperty("m_LocalRotation.y"), InteractionMode.AutomatedAction);
            if(Mathf.Abs(so.FindProperty("m_LocalRotation.z").floatValue - reference.localEulerAngles.z) < float.Epsilon)
                PrefabUtility.RevertPropertyOverride(so.FindProperty("m_LocalRotation.z"), InteractionMode.AutomatedAction);
        }
        if(Mathf.Abs(posDiff.x) < float.Epsilon)
            PrefabUtility.RevertPropertyOverride(so.FindProperty("m_LocalPosition.x"), InteractionMode.AutomatedAction);
        if(Mathf.Abs(posDiff.y) < float.Epsilon)
            PrefabUtility.RevertPropertyOverride(so.FindProperty("m_LocalPosition.y"), InteractionMode.AutomatedAction);
        if(Mathf.Abs(posDiff.z) < float.Epsilon)
            PrefabUtility.RevertPropertyOverride(so.FindProperty("m_LocalPosition.z"), InteractionMode.AutomatedAction);

        _ = so.ApplyModifiedProperties();
    }

    /// <summary>
    /// Returns whether the given <paramref name="transform"/> has default overrides that are identical to its source prefab
    /// </summary>
    /// <param name="transform">The Transform to check</param>
    public static bool HasIdenticalTransformOverrides(Transform transform) {
        var reference = PrefabUtility.GetCorrespondingObjectFromSource(transform);
        return reference != null && HasIdenticalTransformOverrides(transform, reference);
    }

    /// <summary>
    /// Returns whether the given <paramref name="transform"/> has default overrides that are identical to the given <paramref name="reference"/>
    /// </summary>
    /// <param name="transform">The Transform to check</param>
    /// <param name="reference">The reference Transform to compare to</param>
    public static bool HasIdenticalTransformOverrides(Transform transform, Transform reference) {
        using var so = new SerializedObject(transform);

        // Check position
        var posDiff = transform.localPosition - reference.localPosition;
        if(Mathf.Abs(posDiff.x) < float.Epsilon && so.FindProperty("m_LocalPosition.x").prefabOverride ||
           Mathf.Abs(posDiff.y) < float.Epsilon && so.FindProperty("m_LocalPosition.y").prefabOverride ||
           Mathf.Abs(posDiff.z) < float.Epsilon && so.FindProperty("m_LocalPosition.z").prefabOverride) {
            return true;
        }

        // Check rotation
        if(so.FindProperty("m_LocalRotation").prefabOverride || so.FindProperty("m_LocalEulerAnglesHint").prefabOverride) {
            // There are some overrides, check if something can be reverted

            // Check if the rotation is the same
            if(transform.localRotation == reference.localRotation) return true;

            // Check cases like one of the euler angles is -0
            if(Mathf.Abs(so.FindProperty("m_LocalRotation.x").floatValue - reference.localEulerAngles.x) < float.Epsilon && so.FindProperty("m_LocalRotation.x").prefabOverride ||
               Mathf.Abs(so.FindProperty("m_LocalRotation.y").floatValue - reference.localEulerAngles.y) < float.Epsilon && so.FindProperty("m_LocalRotation.y").prefabOverride ||
               Mathf.Abs(so.FindProperty("m_LocalRotation.z").floatValue - reference.localEulerAngles.z) < float.Epsilon && so.FindProperty("m_LocalRotation.z").prefabOverride) {
                return true;
            }
        }
        return false;
    }

    [MenuItem("CONTEXT/Transform/RevertIdenticalTransformOverrides", false)]
    private static void RevertIdenticalTransformOverrides(MenuCommand command) {
        var transform = (Transform)command.context;
        RevertIdenticalTransformOverrides(transform);
    }

    [MenuItem("CONTEXT/Transform/RevertIdenticalTransformOverrides", true)]
    private static bool RevertIdenticalTransformOverridesValidation(MenuCommand command) {
        var transform = (Transform)command.context;
        return HasIdenticalTransformOverrides(transform);
    }
}
