#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;

/// <summary>
/// Functionality extensions to the new Input System
/// </summary>
public static class InputSystemExtensions {

    /// <summary>
    /// Enables or disables the input action map.<br/>
    /// Call <c>.Get().SetEnabled(value)</c> on your custom action map.
    /// </summary>
    /// <remarks>
    /// Note: Extending the custom action map directly is not possible because it is a generated struct.
    /// </remarks>
    /// <param name="enabled">Whether to enable or disable the input action map</param>
    public static void SetEnabled(this InputActionMap self, bool enabled) {
        if(enabled) self.Enable();
        else self.Disable();
    }
}
#endif
