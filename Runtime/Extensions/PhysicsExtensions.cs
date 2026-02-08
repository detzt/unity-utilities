// MIT License
//
// Copyright (c) 2017 Justin Larrabee <justonia@gmail.com>
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.Assertions;
using static Utils;

/// <summary>
/// Utility class for extensions of the physics system
/// </summary>
[SuppressMessage("ReSharper", "UnusedMember.Global")] // Unused methods are suppressed for this class
[SuppressMessage("ReSharper", "Unity.PreferNonAllocApi")] // Allocating methods are only passed through
public static class PhysicsExtensions {
    private static int[] masksByLayer; // Representation of the layer collision matrix in physics settings

    /// <summary>
    /// Initializes the LayerMask lookup table
    /// </summary>
    private static void Init() {
        masksByLayer = new int[32];
        for(int i = 0; i < 32; i++) {
            int mask = 0;
            for(int j = 0; j < 32; j++) {
                if(!Physics.GetIgnoreLayerCollision(i, j))
                    mask |= 1 << j;
            }
            masksByLayer[i] = mask;
        }
    }

    /// <summary>
    /// Returns a LayerMask for the given layer according to the layer collision matrix setting
    /// </summary>
    /// <param name="layer">The layer to get its mask for</param>
    /// <returns>The LayerMask for the given layer</returns>
    public static int MaskForLayer(int layer) {
        if(masksByLayer == null) Init();
        Assert.IsNotNull(masksByLayer);
        return masksByLayer[layer];
    }

    // General

    public static Vector3 GetWorldSpaceCenter(Collider coll) {
        return coll switch {
            BoxCollider box => box.transform.TransformPoint(box.center),
            SphereCollider sphere => sphere.transform.TransformPoint(sphere.center),
            CapsuleCollider capsule => capsule.transform.TransformPoint(capsule.center),
            _ => ReturnValueAndLogError(Vector3.zero, $"PhysicsExtensions.GetWorldSpaceCenter: Unsupported collider {coll}"),
        };
    }

    public static int OverlapColliderNonAlloc(Collider coll, Collider[] results, int layerMask = Physics.DefaultRaycastLayers,
        QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal) {
        return coll switch {
            BoxCollider boxColl => OverlapBoxNonAlloc(boxColl, results, layerMask, queryTriggerInteraction),
            CapsuleCollider capsuleColl => OverlapCapsuleNonAlloc(capsuleColl, results, layerMask, queryTriggerInteraction),
            SphereCollider sphereColl => OverlapSphereNonAlloc(sphereColl, results, layerMask, queryTriggerInteraction),
            _ => ReturnValueAndLogError(0, $"PhysicsExtensions.OverlapColliderNonAlloc: Unsupported collider {coll}"),
        };
    }

    public static bool Contains(Collider coll, Vector3 point) {
        return coll switch {
            BoxCollider boxColl => boxColl.Contains(point),
            CapsuleCollider capsuleColl => capsuleColl.Contains(point),
            SphereCollider sphereColl => sphereColl.Contains(point),
            _ => ReturnValueAndLogError(false, $"PhysicsExtensions.Contains: Unsupported collider {coll}"),
        };
    }

#pragma warning disable UNT0028 // warnings about using allocating methods are not relevant here because it is only an API wrapper. They are propagated using [Obsolete]

    // Box

    public static bool BoxCast(BoxCollider box, Vector3 direction, float maxDistance = Mathf.Infinity, int layerMask = Physics.DefaultRaycastLayers,
        QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal) {
        box.ToWorldSpaceBox(out Vector3 center, out Vector3 halfExtents, out Quaternion orientation);
        return Physics.BoxCast(center, halfExtents, direction, orientation, maxDistance, layerMask, queryTriggerInteraction);
    }

    public static bool BoxCast(BoxCollider box, Vector3 direction, out RaycastHit hitInfo, float maxDistance = Mathf.Infinity, int layerMask = Physics.DefaultRaycastLayers,
        QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal) {
        box.ToWorldSpaceBox(out Vector3 center, out Vector3 halfExtents, out Quaternion orientation);
        return Physics.BoxCast(center, halfExtents, direction, out hitInfo, orientation, maxDistance, layerMask, queryTriggerInteraction);
    }

    [System.Obsolete("This method allocates memory. Use BoxCastNonAlloc instead.")]
    public static RaycastHit[] BoxCastAll(BoxCollider box, Vector3 direction, float maxDistance = Mathf.Infinity, int layerMask = Physics.DefaultRaycastLayers,
        QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal) {
        box.ToWorldSpaceBox(out Vector3 center, out Vector3 halfExtents, out Quaternion orientation);
        return Physics.BoxCastAll(center, halfExtents, direction, orientation, maxDistance, layerMask, queryTriggerInteraction);
    }

    public static int BoxCastNonAlloc(BoxCollider box, Vector3 direction, RaycastHit[] results, float maxDistance = Mathf.Infinity, int layerMask = Physics.DefaultRaycastLayers,
        QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal) {
        box.ToWorldSpaceBox(out Vector3 center, out Vector3 halfExtents, out Quaternion orientation);
        return Physics.BoxCastNonAlloc(center, halfExtents, direction, results, orientation, maxDistance, layerMask, queryTriggerInteraction);
    }

    public static bool CheckBox(BoxCollider box, int layerMask = Physics.DefaultRaycastLayers,
        QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal) {
        box.ToWorldSpaceBox(out Vector3 center, out Vector3 halfExtents, out Quaternion orientation);
        return Physics.CheckBox(center, halfExtents, orientation, layerMask, queryTriggerInteraction);
    }

    public static Collider[] OverlapBox(BoxCollider box, int layerMask = Physics.DefaultRaycastLayers,
        QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal) {
        box.ToWorldSpaceBox(out Vector3 center, out Vector3 halfExtents, out Quaternion orientation);
        return Physics.OverlapBox(center, halfExtents, orientation, layerMask, queryTriggerInteraction);
    }

    public static int OverlapBoxNonAlloc(BoxCollider box, Collider[] results, int layerMask = Physics.DefaultRaycastLayers,
        QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal) {
        box.ToWorldSpaceBox(out Vector3 center, out Vector3 halfExtents, out Quaternion orientation);
        return Physics.OverlapBoxNonAlloc(center, halfExtents, results, orientation, layerMask, queryTriggerInteraction);
    }

    public static void ToWorldSpaceBox(this BoxCollider box, out Vector3 center, out Vector3 halfExtents, out Quaternion orientation) {
        Transform t;
        orientation = (t = box.transform).rotation;
        center = t.TransformPoint(box.center);
        Vector3 lossyScale = t.lossyScale;
        Vector3 scale = MathV.Abs(lossyScale);
        halfExtents = Vector3.Scale(scale, box.size) * 0.5f;
    }

    public static bool Contains(this BoxCollider box, Vector3 point) {
        box.ToWorldSpaceBox(out Vector3 center, out Vector3 halfExtents, out Quaternion orientation);
        Vector3 localPoint = Quaternion.Inverse(orientation) * (point - center);
        return Mathf.Abs(localPoint.x) <= halfExtents.x &&
               Mathf.Abs(localPoint.y) <= halfExtents.y &&
               Mathf.Abs(localPoint.z) <= halfExtents.z;
    }

    // Sphere

    public static bool SphereCast(SphereCollider sphere, Vector3 direction, out RaycastHit hitInfo, float maxDistance = Mathf.Infinity,
        int layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal) {
        sphere.ToWorldSpaceSphere(out Vector3 center, out float radius);
        return Physics.SphereCast(center, radius, direction, out hitInfo, maxDistance, layerMask, queryTriggerInteraction);
    }

    [System.Obsolete("This method allocates memory. Use SphereCastNonAlloc instead.")]
    public static RaycastHit[] SphereCastAll(SphereCollider sphere, Vector3 direction, float maxDistance = Mathf.Infinity, int layerMask = Physics.DefaultRaycastLayers,
        QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal) {
        sphere.ToWorldSpaceSphere(out Vector3 center, out float radius);
        return Physics.SphereCastAll(center, radius, direction, maxDistance, layerMask, queryTriggerInteraction);
    }

    public static int SphereCastNonAlloc(SphereCollider sphere, Vector3 direction, RaycastHit[] results, float maxDistance = Mathf.Infinity,
        int layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal) {
        sphere.ToWorldSpaceSphere(out Vector3 center, out float radius);
        return Physics.SphereCastNonAlloc(center, radius, direction, results, maxDistance, layerMask, queryTriggerInteraction);
    }

    public static bool CheckSphere(SphereCollider sphere, int layerMask = Physics.DefaultRaycastLayers,
        QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal) {
        sphere.ToWorldSpaceSphere(out Vector3 center, out float radius);
        return Physics.CheckSphere(center, radius, layerMask, queryTriggerInteraction);
    }

    [System.Obsolete("This method allocates memory. Use OverlapSphereNonAlloc instead.")]
    public static Collider[] OverlapSphere(SphereCollider sphere, int layerMask = Physics.DefaultRaycastLayers,
        QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal) {
        sphere.ToWorldSpaceSphere(out Vector3 center, out float radius);
        return Physics.OverlapSphere(center, radius, layerMask, queryTriggerInteraction);
    }

    public static int OverlapSphereNonAlloc(SphereCollider sphere, Collider[] results, int layerMask = Physics.DefaultRaycastLayers,
        QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal) {
        sphere.ToWorldSpaceSphere(out Vector3 center, out float radius);
        return Physics.OverlapSphereNonAlloc(center, radius, results, layerMask, queryTriggerInteraction);
    }

    public static void ToWorldSpaceSphere(this SphereCollider sphere, out Vector3 center, out float radius) {
        Transform t;
        center = (t = sphere.transform).TransformPoint(sphere.center);
        radius = sphere.radius * MathV.Max(MathV.Abs(t.lossyScale));
    }

    public static bool Contains(this SphereCollider sphere, Vector3 point) {
        sphere.ToWorldSpaceSphere(out Vector3 center, out float radius);
        return (point - center).sqrMagnitude <= radius * radius;
    }

    // Capsule

    public static bool CapsuleCast(CapsuleCollider capsule, Vector3 direction, out RaycastHit hitInfo, float maxDistance = Mathf.Infinity,
        int layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal) {
        capsule.ToWorldSpaceCapsule(out Vector3 point0, out Vector3 point1, out float radius);
        return Physics.CapsuleCast(point0, point1, radius, direction, out hitInfo, maxDistance, layerMask, queryTriggerInteraction);
    }

    [System.Obsolete("This method allocates memory. Use CapsuleCastNonAlloc instead.")]
    public static RaycastHit[] CapsuleCastAll(CapsuleCollider capsule, Vector3 direction, float maxDistance = Mathf.Infinity, int layerMask = Physics.DefaultRaycastLayers,
        QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal) {
        capsule.ToWorldSpaceCapsule(out Vector3 point0, out Vector3 point1, out float radius);
        return Physics.CapsuleCastAll(point0, point1, radius, direction, maxDistance, layerMask, queryTriggerInteraction);
    }

    public static int CapsuleCastNonAlloc(CapsuleCollider capsule, Vector3 direction, RaycastHit[] results, float maxDistance = Mathf.Infinity,
        int layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal) {
        capsule.ToWorldSpaceCapsule(out Vector3 point0, out Vector3 point1, out float radius);
        return Physics.CapsuleCastNonAlloc(point0, point1, radius, direction, results, maxDistance, layerMask, queryTriggerInteraction);
    }

    public static bool CheckCapsule(CapsuleCollider capsule, int layerMask = Physics.DefaultRaycastLayers,
        QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal) {
        capsule.ToWorldSpaceCapsule(out Vector3 point0, out Vector3 point1, out float radius);
        return Physics.CheckCapsule(point0, point1, radius, layerMask, queryTriggerInteraction);
    }

    [System.Obsolete("This method allocates memory. Use OverlapCapsuleNonAlloc instead.")]
    public static Collider[] OverlapCapsule(CapsuleCollider capsule, int layerMask = Physics.DefaultRaycastLayers,
        QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal) {
        capsule.ToWorldSpaceCapsule(out Vector3 point0, out Vector3 point1, out float radius);
        return Physics.OverlapCapsule(point0, point1, radius, layerMask, queryTriggerInteraction);
    }

    public static int OverlapCapsuleNonAlloc(CapsuleCollider capsule, Collider[] results, int layerMask = Physics.DefaultRaycastLayers,
        QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal) {
        capsule.ToWorldSpaceCapsule(out Vector3 point0, out Vector3 point1, out float radius);
        return Physics.OverlapCapsuleNonAlloc(point0, point1, radius, results, layerMask, queryTriggerInteraction);
    }

#pragma warning restore UNT0028

    public static void ToWorldSpaceCapsule(this CapsuleCollider capsule, out Vector3 point0, out Vector3 point1, out float radius) {
        Transform t;
        Vector3 center = (t = capsule.transform).TransformPoint(capsule.center);
        radius = 0f;
        float height = 0f;
        Vector3 lossyScale = MathV.Abs(t.lossyScale);
        Vector3 dir = Vector3.zero;

        switch(capsule.direction) {
            case 0: // x
                radius = Mathf.Max(lossyScale.y, lossyScale.z) * capsule.radius;
                height = lossyScale.x * capsule.height;
                dir = t.TransformDirection(Vector3.right);
                break;
            case 1: // y
                radius = Mathf.Max(lossyScale.x, lossyScale.z) * capsule.radius;
                height = lossyScale.y * capsule.height;
                dir = t.TransformDirection(Vector3.up);
                break;
            case 2: // z
                radius = Mathf.Max(lossyScale.x, lossyScale.y) * capsule.radius;
                height = lossyScale.z * capsule.height;
                dir = t.TransformDirection(Vector3.forward);
                break;
            default:
                Debug.LogWarning($"PhysicsExtensions.ToWorldSpaceCapsule: unknown direction {capsule.direction}");
                break;
        }

        if(height < radius * 2f) dir = Vector3.zero;

        point0 = center + dir * (height * 0.5f - radius);
        point1 = center - dir * (height * 0.5f - radius);
    }

    public static bool Contains(this CapsuleCollider capsule, Vector3 point) {
        capsule.ToWorldSpaceCapsule(out Vector3 point0, out Vector3 point1, out float radius);
        Vector3 axis = point1 - point0;
        float lengthSqr = axis.sqrMagnitude;
        if(lengthSqr == 0f) {
            // Degenerate to sphere
            return (point - point0).sqrMagnitude <= radius * radius;
        }
        float t = Vector3.Dot(point - point0, axis) / lengthSqr;
        Vector3 projection = point0 + Mathf.Clamp01(t) * axis;
        return (point - projection).sqrMagnitude <= radius * radius;
    }
}
