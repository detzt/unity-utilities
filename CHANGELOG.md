# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

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

[unreleased]: https://github.com/detzt/unity-utilities/v1.1.1...HEAD
[1.0.1]: https://github.com/detzt/unity-utilities/v1.0.0...v1.0.1
[1.0.0]: https://github.com/detzt/unity-utilities/tag/v1.0.0
