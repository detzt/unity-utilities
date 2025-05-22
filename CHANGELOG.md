# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.6.1] - 2025-04-13

### Added
- `MathV.Round` overload with a configurable number of decimal digits to keep.

## [1.6.0] - 2024-08-15

### Added
- `InterfaceReference<T>` allows serializing objects that implement an interface.

## [1.5.1] - 2024-08-14

### Fixed
- Removed `AspectRatioContainer` for Unity versions below 2023.2, because the current implementation uses features that are not available in older versions.

## [1.5.0] - 2024-07-15

### Added
- `InputActionMap.SetEnabled` that calls `Enable` or `Disable` depending on the parameter.

## [1.4.0] - 2024-04-21

### Added
- `TryGetComponentInChildren`

## [1.3.3] - 2024-04-21

- Extended `Array.IsValidIndex` to be usable on all `ICollection` types.

## [1.3.2] - 2024-04-08

### Fixed
- `IdenticalTransformOverrides` now also checks for individual euler angles. (-0, 90, -0) == (0, 90, 0)

## [1.3.1] - 2024-03-25

### Fixed
- Custom PropertyDrawer for `Map` is now also applied to subclasses of `Map`.
- Proper `ToString` for `Couple`, `Triple`, `MinMax`, and `OptionalValue` types.

## [1.3.0] - 2024-03-15

### Added

- WoldPose and LocalPose
  - Each is a lightweight struct that contains a `position` and a `rotation` field.
    Similar to Unity's `Transform` but without scale, parent, children, component, and other overhead.
  - They contain the same data, but are semantically different, as they encode the space in which they are to be interpreted.
  - Various `TransformXXX` and `InverseTransformXXX` methods are provided to convert objects between spaces.
  - `WorldPose` and `LocalPose` can be implicitly created from `Transform`.
    When implicit conversions from other custom types are added, `WorldPose` and `LocalPose` can be used as general purpose parameter that reduce the amount of overloads needed.

## [1.2.0] - 2024-02-24

### Added
- UI Toolkit Custom Controls:
  - `AspectRatioContainer` that maintains a configured aspect ratio.

## [1.1.0] - 2024-02-21

### Added
- Extensions to Prefabs:
  - `RevertIdenticalTransformOverrides` reverts unused overrides to local position and rotation to reduce prefab and scene file size and noise.
  - `HasIdenticalTransformOverrides` returns whether RevertIdenticalTransformOverrides has something to revert.
- Extensions to UI Toolkit:
  - `SetActive` for `VisualElement` that mimics `GameObject.SetActive`.
- Changes to Editor Styling:
  - `Vector4` field in one line (like `Vector3` and `Quaternion` fields).
  - Prevent clipping of MinMaxSlider Handles at edges by added padding.
- Extensions to Unity Objects:
  - `SetPositionAndRotation` that takes a `Transform` as parameter.
  - `Vector3.XZ` and `Vector2.X0Y` swizzling.

## [1.0.0] - 2023-10-09

### Added

- Serializable `Couple`, `Triple`, `Map`, `MinMax`, and `OptionalValue` types with custom PropertyDrawers
- `MinMaxSlider` attribute
- `AutoSetup` attribute that tries to wire component references automatically
- Extensions to Unity Vector types:
  - `.With`, `.Add`, and `.Rotate` modifications, e.g. `Vector3 v = transform.forward.With(y: 0f);`
  - Component wise `Mul` and `Div`
  - `SqrDist` semantic shorthand
  - `Vector3.XY` swizzling
- `MathV` static class with component wise analogs of Mathf methods:
  - `Abs`, `Round`, `Max`, `Clamp`, and `Random` that operate on a single vector
  - `Min` and `Max` that operate on two vectors
  - `Max` and `Sum` that iterate over the components of a single vector
  - `MinMax` that returns an ordered pair
- Extensions to C# System types:
  - `Array.IsValidIndex` as guard against ArrayIndexOutOfBounds exceptions
  - `IDictionary.GetOrCreate`
  - `KeyValuePair.Deconstruct`
- Extensions to Unity Physics:
  - `Overlap*`, `Check*`, and `*Cast` methods that take `*Collider` as a parameter
  - Layer to LayerMask conversion
- Extensions to Unity Objects:
  - `HasComponent`
  - `GetOrAddComponent`
  - `TryGetComponentInParent`
  - `GetComponentsInOnlyChildren`
  - `GetComponentsInOnlyCousins`
- Static `Utils` class with the remaining utilities:
  - `EnumValues` iterator, e.g. `foreach(var value in Utils.EnumValues<SomeEnum>())`
  - `ReturnValueAndLogError` utility for lean switch expressions

[unreleased]: https://github.com/detzt/unity-utilities/v1.4.0...HEAD
[1.4.0]: https://github.com/detzt/unity-utilities/v1.3.0...v1.4.0
[1.3.0]: https://github.com/detzt/unity-utilities/v1.2.0...v1.3.0
[1.2.0]: https://github.com/detzt/unity-utilities/v1.1.0...v1.2.0
[1.1.0]: https://github.com/detzt/unity-utilities/v1.0.0...v1.1.0
[1.0.0]: https://github.com/detzt/unity-utilities/tag/v1.0.0
