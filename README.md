# Get

**Unity attribute that magically seeks out object references.**
Stay in the flow; stop fiddling around dragging and dropping into with Inspector fields.

| [Documentation](DOCS.md) |
| - |

---

Tag your serialized fields with a `[Get]` attribute...

```c#
[GetInChildren] public Collider myCollider;
```

...and select the reference you're looking for.

![GetAttribute Update Demo](https://user-images.githubusercontent.com/38191432/190724011-d235cb6f-7d09-4844-8fad-d18abd8b62d7.png)

## Quick start guide
Simply type the attribute before serialized fields.

This works **only in-editor,** not for objects created at runtime. In other words, it replaces, simplifies, and accelerates the process of selecting references in the Inspector.

```c#
using Extra.Attributes;
using UnityEngine;

public class MyBehaviour : MonoBehaviour
{
    // Gets the component from the same GameObject, like GetComponent.
    [Get] public Rigidbody2D rb2d;
    
    // Gets the component from the same GameObject or its children, like GetComponentInChildren.
    [GetInChildren] public SpriteRenderer spriteRenderer;
    
    // Gets the component from the same GameObject or its parent, like GetComponentInParent.
    [GetInParent] public Collider2D hitbox;
    
    // Get the component from the same GameObject, its parent, or its children.
    [GetInChildrenAndParent] public SortingGroup sortingGroup;
    
    // Gets the component from any GameObject, like FindObjectOfType.
    [Find] public GameManager gameManager;
    
    // Gets the object from anywhere in AssetDatabase; works with ScriptableObjects.
    [FindAssets] public CardData data;
    
    // Gets objects that implement this interface. 
    // This is a wrapper type that works with any Get attribute. You can implement your own wrapper types.
    [Get] public InterfaceReference<IInteractionHandler> interactable;
    
    ...
}
```

## How do I add this to Unity?
It's easy!

#### If you have Git...
1. Open the Unity Editor. On the top ribbon, click Window > Package Manager.
2. Click the + button in the upper left corner, and click "Add package from git url..."
3. Enter "https://github.com/bgsulz/Get-for-Unity.git"
4. Enjoy!

#### If you don't have Git (or want to modify the code)...
1. Click the big green "Code" button and click Download ZIP.
2. Extract the contents of the .zip into your Unity project's Assets folder.
3. Enjoy!
