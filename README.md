# GetAttribute
**Unity attribute that fills your component references for you.**
Stay in the flow and keep coding rather than fiddling around with Inspector fields.

## Quick start guide
Simply type the attribute before public or serialized fields. _The field will light up **red** if it can't find a component to fill it!_

This works **only in-editor,** not for objects created at runtime. In other words, it replaces dragging and dropping references in the Inspector.

```c#
using BGS.Attributes;
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
    
    ...
}
```

Version 2.0
