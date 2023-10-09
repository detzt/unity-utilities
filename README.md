# Unity Utilities
This is a collection of project agnostic utilities and extensions for Unity.

## Serializable Types with Custom Editors
- `Couple`, `Triple`, `Map`, `MinMax`, and `OptionalValue`
- `MinMaxSlider` attribute (uses NaughtyAttributes)

## Attributes
- `AutoSetup` attribute that tries to wire component references automatically

## Extensions
### Extensions to Unity Vector types:
- `.With`, `.Add`, and `.Rotate` modifications, e.g. `Vector3 v = Vector3.up.With(y: 25f);`
- Component wise `Mul` and `Div`
- `SqrDist` semantic shorthand
- `XY` swizzling

### `MathV` static class with component wise analogs of Mathf methods:
- `Abs`, `Round`, `Max`, `Clamp`, and `Random` that operate on a single vector
- `Min` and `Max` that operate on two vectors
- `Max` and `Sum` that iterate over the components of a single vector
- `MinMax` that returns an ordered pair

### Extensions to C# System types:
- `Array.IsValidIndex` as guard against ArrayIndexOutOfBounds exceptions
- `IDictionary.GetOrCreate`
- `KeyValuePair.Deconstruct`

### Extensions to Unity Physics:
- `Overlap*`, `Check*`, and `*Cast` methods that take `*Collider` as a parameter
- Layer to LayerMask conversion

### Extensions to Unity Objects:
- `HasComponent`
- `GetOrAddComponent`
- `TryGetComponentInParent`
- `GetComponentsInOnlyChildren`
- `GetComponentsInOnlyCousins`

## Static `Utils` class with the remaining utilities:
- `EnumValues` iterator, e.g. `foreach(var value in Utils.EnumValues<SomeEnum>())`
- `ReturnValueAndLogError` utility for lean switch expressions
