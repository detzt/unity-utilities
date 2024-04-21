# Unity Utilities
This is a collection of project agnostic utilities and extensions for Unity.

## Serializable Types with Custom PropertyDrawers
They are implemented using PropertyDrawers for these specific types without a general editor that would inject itself everywhere. This makes this compatible and does not interfere with other custom editors.
- `Couple`, `Triple`, `Map`, `MinMax`, and `OptionalValue`
- `MinMaxSlider` attribute

## Attributes
- `AutoSetup` attribute that tries to wire component references automatically

## Extensions
### Extensions to Unity Vector types:
- `.With`, `.Add`, and `.Rotate` modifications, e.g. `Vector3 v = transform.forward.With(y: 0f);`
- Component wise `Mul` and `Div`
- `SqrDist` semantic shorthand
- `Vector3.XY`, `Vector3.XZ`, and `Vector2.X0Y` swizzling

### `MathV` static class with component wise analogs of Mathf methods:
- `Abs`, `Round`, `Max`, `Clamp`, and `Random` that operate on a single vector
- `Min` and `Max` that operate on two vectors
- `Max` and `Sum` that iterate over the components of a single vector
- `MinMax` that returns an ordered pair

### Extensions to C# System types:
- `ICollection.IsValidIndex` as guard against ArrayIndexOutOfBounds exceptions
- `IDictionary.GetOrCreate`
- `KeyValuePair.Deconstruct`

### Extensions to Unity Physics:
- `Overlap*`, `Check*`, and `*Cast` methods that take `*Collider` as a parameter
- Layer to LayerMask conversion

### Extensions to Unity Objects:
- `HasComponent`
- `GetOrAddComponent`
- `TryGetComponentInParent`
- `TryGetComponentInChildren`
- `GetComponentsInOnlyChildren`
- `GetComponentsInOnlyCousins`
- `SetPositionAndRotation` that takes a `Transform` as parameter.

### Extensions to Prefabs:
- `RevertIdenticalTransformOverrides` reverts unused overrides to local position and rotation to reduce prefab and scene file size and noise.
- `HasIdenticalTransformOverrides` returns whether RevertIdenticalTransformOverrides has something to revert.

### Extensions to UI Toolkit:
- `SetActive` for `VisualElement` that mimics `GameObject.SetActive`.

## UI Toolkit Custom Controls
- `AspectRatioContainer` that maintains a configured aspect ratio.

## WoldPose and LocalPose
- Each is a lightweight struct that contains a `position` and a `rotation` field.
  Similar to Unity's `Transform` but without scale, parent, children, component, and other overhead.
- They contain the same data, but are semantically different, as they encode the space in which they are to be interpreted.
- Various `TransformXXX` and `InverseTransformXXX` methods are provided to convert objects between spaces.
- `WorldPose` and `LocalPose` can be implicitly created from `Transform`.
  When implicit conversions from other custom types are added, `WorldPose` and `LocalPose` can be used as general purpose parameter that reduce the amount of overloads needed.

## Static `Utils` class with the remaining utilities:
- `EnumValues` iterator, e.g. `foreach(var value in Utils.EnumValues<SomeEnum>())`
- `ReturnValueAndLogError` utility for lean switch expressions

## Changes to Editor Styling:
- `Vector4` field in one line (like `Vector3` and `Quaternion` fields).
- Prevent clipping of MinMaxSlider Handles at edges by added padding.
