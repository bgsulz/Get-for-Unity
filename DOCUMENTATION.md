# Attributes

The attribute classes are as follows:

| Class Name | Syntax | Functionality |
| - | - | - |
| `Get` | `[Get]` / `[Get(GetterSource.Object)]` | Like `GetComponent`, searches the same GameObject as this script. |
| `GetInChildren` | `[GetInChildren]` / `[Get(GetterSource.Children)]` | Like `GetComponentInChildren`, searches this GameObject and its children. |
| `GetInParent` | `[GetInParent]` / `[Get(GetterSource.Parent)]` | Like `GetComponentInParent`, searches this GameObject and its parent. |
| `Find` | `[Find]` / `[Get(GetterSource.Find)]` | Like `FindObjectOfType`, searches all loaded GameObjects. |
| `FindAssets` | `[FindAssets]` / `[Get(GetterSource.FindAssets)]` | Searches the asset database for ScriptableObjects. |

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
