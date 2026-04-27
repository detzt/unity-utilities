using UnityEngine;

/// <summary>
/// Functionality extensions to the Animator and Animation components.
/// </summary>
public static class AnimationExtensions {

    /// <summary>
    /// Returns whether the animator is in the given state.
    /// </summary>
    /// <param name="stateHash">The hashed name of the state to check.</param>
    /// <param name="layer">The layer index to check, defaults to the first layer.</param>
    /// <returns></returns>
    public static bool IsInState(this Animator self, int stateHash, int layer = 0) => self.GetCurrentAnimatorStateInfo(layer).shortNameHash == stateHash;

    /// <summary>
    /// Returns the value of the specified trigger parameter.<br/>
    /// Same as <see cref="Animator.GetBool(int)"/> but with a semantically correct name.
    /// </summary>
    /// <inheritdoc cref="Animator.GetBool(int)"/>
    public static bool GetTrigger(this Animator self, int id) => self.GetBool(id);
    /// <summary>
    /// Returns the value of the specified trigger parameter.<br/>
    /// Same as <see cref="Animator.GetBool(string)"/> but with a semantically correct name.
    /// </summary>
    /// <inheritdoc cref="Animator.GetBool(string)"/>
    public static bool GetTrigger(this Animator self, string name) => self.GetBool(name);

}
