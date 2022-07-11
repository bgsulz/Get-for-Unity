# Enums

## GetterSource

`GetterSource` is a flags enum for describing where the attribute should search.

| Name | Value | Meaning |
| - | - | - |
| Object | 1 | Search the same GameObject as this script. |
| Children | 2 | Search this GameObject and its children. |
| Parent | 4 | Search this GameObject and its parent. |
| Find | 8 | Search all loaded GameObjects. |
| FindAssets | 16 | Search the asset database. |

# Attributes

The pre-defined attribute classes are as follows:

| Class Name | Syntax | Equivalent Constructor | Functionality |
| - | - | - | - |
| `Get` | `[Get]` | `[Get(GetterSource.Object)]` | Like `GetComponent`, searches the same GameObject as this script. |
| `GetInChildren` | `[GetInChildren]` | `[Get(GetterSource.Children)]` | Like `GetComponentInChildren`, searches this GameObject and its children. |
| `GetInParent` | `[GetInParent]` | `[Get(GetterSource.Parent)]` | Like `GetComponentInParent`, searches this GameObject and its parent. |
| `GetInChildrenAndParent` | `[GetInChildrenAndParent]` | `[Get(GetterSource.Children \| GetterSource.Parent)]` | Searches this GameObject, its children, and its parent. |
| `Find` | `[Find]` | `[Get(GetterSource.Find)]` | Like `FindObjectOfType`, searches all loaded GameObjects. |
| `FindAssets` | `[FindAssets]` | `[Get(GetterSource.FindAssets)]` | Searches the asset database for ScriptableObjects. |
| `FindAll` | `[FindAll]` | `[Get(GetterSource.Children \| GetterSource.Parent \| GetterSource.FindAssets)]` | Searches all loaded GameObjects and the asset database. |

You can define your own custom search criteria by passing a `GetterSource` into the attribute's constructor. For example:

```cs
[Get(GetterSource.Parent | GetterSource.FindAssets)]
```

This will search this GameObject, its parent, and the asset database.

Unless no valid Objects are found, the field refuses to be empty. This is by design. In the many cases that a Component reference only exists to refer to a specific single instance -- in other words, just to link multiple scripts together -- auto-filling is convenient.

# Wrapper Types

Get is compatible with types that do not inherit from `UnityEngine.Object` but instead "wrap" a contained property of type `UnityEngine.Object`. A single convenience wrapper type is included. You can implement your own.

## InterfaceReference\<T\>

`InterfaceReference<T>` is a serializable class for referencing an Object that implements a specific interface.

```cs
// This searches for a Component on this GameObject that implements IInteractable.
[Get] public InterfaceReference<IInteractable> interactable;
```

### Properties

```cs
T AsInterface
```
Returns the wrapped Object as the specified interface. The object's implicit conversion to type T references this property.

## Custom Wrapper Type

You can implement your own Object wrapper type. For full functionality with Get, the following conditions must be true:
- The type implements the `IPropertyWrapper` interface.
- The type serializes a field of type `UnityEngine.Object` called `reference`.
- The type's first generic type parameter is its "unwrapped value." _(May not be required -- to be checked soon.)_
