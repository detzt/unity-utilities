using UnityEditor;
using UnityEngine;

/// <summary>
/// Collection of utility functions for working with prefabs
/// </summary>
public static class PrefabUtils {

    /// <summary>
    /// Reverts the local rotation and position if they are identical to its source prefab
    /// </summary>
    /// <param name="transform">The Transform to change</param>
    public static void RevertIdenticalTransformOverrides(Transform transform) {
        var reference = PrefabUtility.GetCorrespondingObjectFromSource(transform);
        if(reference != null) RevertIdenticalTransformOverrides(transform, reference.transform);
    }

    /// <summary>
    /// Reverts the local rotation and position if they are identical to the given <paramref name="reference"/> Transform
    /// </summary>
    /// <param name="transform">The Transform to change</param>
    /// <param name="reference">Reference Transform</param>
    public static void RevertIdenticalTransformOverrides(Transform transform, Transform reference) {
        using var so = new SerializedObject(transform);
        Undo.RecordObject(transform, "Revert Identity Transforms");
        PrefabUtility.RecordPrefabInstancePropertyModifications(transform);

        var posDiff = transform.position - reference.position;
        if(transform.rotation == reference.rotation)
            PrefabUtility.RevertPropertyOverride(so.FindProperty("m_LocalRotation"), InteractionMode.AutomatedAction);
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
        var posDiff = transform.position - reference.position;
        return transform.rotation == reference.rotation && so.FindProperty("m_LocalRotation").prefabOverride ||
               Mathf.Abs(posDiff.x) < float.Epsilon && so.FindProperty("m_LocalPosition.x").prefabOverride ||
               Mathf.Abs(posDiff.y) < float.Epsilon && so.FindProperty("m_LocalPosition.y").prefabOverride ||
               Mathf.Abs(posDiff.z) < float.Epsilon && so.FindProperty("m_LocalPosition.z").prefabOverride;
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
