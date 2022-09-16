# Enums

## GetterSource

`GetterSource` is a flags enum for describing where **Get** should search.

| Name | Value | Meaning |
| - | - | - |
| Object | 1 | Search the same GameObject as this script. |
| Children | 2 | Search this GameObject and its children. |
| Parent | 4 | Search this GameObject and its parent. |
| Find | 8 | Search all loaded GameObjects. |
| FindAssets | 16 | Search the asset database. |

# Attribute Constructors

### Base Constructor
```cs
GetFrom(GetterSource getterSource, bool autoFill = true)
```

### Inheriting Constructors
```cs
Get(bool autoFill = true) : base(GetterSource.Object)
GetInChildren(bool autoFill = true) : base(GetterSource.Children)
GetInParent(bool autoFill = true) : base(GetterSource.Parent)
GetInChildrenAndParent(bool autoFill = true) : base(GetterSource.Children | GetterSource.Parent)
Find(bool autoFill = true) : base(GetterSource.Find)
FindAssets(bool autoFill = true) : base(GetterSource.FindAssets)
FindAll(bool autoFill = true) : base(GetterSource.Children | GetterSource.Parent | GetterSource.FindAssets)
```

#### Parameters

---

```cs
GetterSource getterSource

```
Defines where **Get** should search.

> You can define a custom search query by passing a `GetterSource` into the base constructor. For example:

```cs
[GetFrom(GetterSource.Parent | GetterSource.FindAssets)]
```

This will search this GameObject, its parent, and the asset database. (You likely won't need this.)

---

```cs
bool autoFill
```

Determines whether the field should auto-fill or not.

> If the field is set to auto-fill, you can't drag and drop whatever object you want into it -- you have to use the dropdown.

> Usually, this is faster and more convenient. **Get** can relieve you of a lot of fiddly dragging and dropping.

---
---
# Wrapper Types

Get is compatible with types that do not inherit from `UnityEngine.Object` but instead "wrap" a contained property of type `UnityEngine.Object`.

## InterfaceReference\<T\>

`InterfaceReference<T>` is a serializable class for referencing a `UnityEngine.Object` that implements a specific interface.

```cs
[Get] public InterfaceReference<IInteractable> interactable;
```

This field searches for a Component on this GameObject that implements `IInteractable`.

### Properties
---
```cs
T AsInterface
```
Returns the wrapped Object as the specified interface.

`InterfaceReference<T>` is implicitly convertible to `T`. This property is useful when the implicit conversion is not enough.

---
---
## Custom Wrapper Type

You can implement your own wrapper type. For full functionality with **Get**:

- The type must implement the `IPropertyWrapper` interface.
- The type must have a serialized field of type `UnityEngine.Object` called `reference`.
	- If you need it, this field name ("reference") is defined as a static property: `IPropertyWrapper.PropertyName`.