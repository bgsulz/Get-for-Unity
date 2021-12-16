# GetAttribute
**Unity attribute that fills your component references for you.**
Stay in the flow and keep coding rather than fiddling around with Inspector fields.

Simply tag your fields with the attribute...

```c#
[GetInChildren] public Collider myCollider;
```

...and select the reference you're looking for.

![GetAttribute Graphics](https://user-images.githubusercontent.com/38191432/127414944-9ce09f7d-3aa0-4d1f-adca-2ee5062d92ae.png)

## Quick start guide
Simply type the attribute before public or serialized fields.

This works **only in-editor,** not for objects created at runtime. In other words, it replaces dragging and dropping references in the Inspector.

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
    [GetInParent] public Collider2D collider;
    
    // Gets the component from any GameObject, like FindObjectOfType.
    [Find] public GameManager gameManager;
    
    // Gets the component from any GameObject or Prefab, like FindObjectOfTypeAll.
    [FindPrefab] public Poolable poolable;
    
    ...
}
```

Version 3.0
