using System;
using UnityEngine;

/// <summary>
/// A serializable wrapper that allows serialization of interfaces.<br/>
/// Only works when the interface is implemented by a UnityEngine.Object, but this restriction is not enforced.
/// </summary>
/// <example>
/// Usage: <code>[SerializeField] private InterfaceReference&lt;ISomeInterface&gt; someField;</code>
/// </example>
[Serializable]
public class InterfaceReference<T> : ISerializationCallbackReceiver {

    /// <summary>
    /// The serialized value contained in this wrapper.<br/>
    /// Stored as general UnityEngine.Object
    /// </summary>
    [SerializeField]
    private UnityEngine.Object objectValue;

    private T interfaceValue;

    /// <summary>
    /// The value contained in this serializable wrapper.<br/>
    /// Returned as the interface type.
    /// </summary>
    public T Value {
        get => interfaceValue;
        set {
            interfaceValue = value;
            objectValue = value as UnityEngine.Object;
        }
    }

    public void OnBeforeSerialize() {
        // If the reference is not of the correct type,
        if(objectValue is not null and not T) {
            // but it's a GameObject, try to find a matching component on it.
#pragma warning disable UNT0014 // T is an interface that is usually implemented by components, but that's not really enforceable.
            if(objectValue is GameObject go && go.TryGetComponent<T>(out var component)) {
#pragma warning restore UNT0014 // Invalid type for call to GetComponent
                objectValue = component as UnityEngine.Object;
            } else {
                // Otherwise nullify the reference to keep its value valid.
                objectValue = null;
            }
        }
    }

    public void OnAfterDeserialize() {
        interfaceValue = objectValue is T value ? value : default;
    }

    /// <summary>
    /// Implicitly converts to the wrapped type.<br/>
    /// If T is an interface, this will only work as an explicit conversion due to C# restrictions.
    /// </summary>
    public static implicit operator T(InterfaceReference<T> self) => self.interfaceValue;

#if UNITY_EDITOR
    /// <summary>Gives the custom property drawer access to the name to avoid string literals.</summary>
    public const string NameOfObjectValue = nameof(objectValue);
#endif
}
