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
}
