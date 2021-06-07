# GetAttribute
**Unity attribute that fills your component references for you.**
Stay in the flow and keep coding rather than fiddling around with Inspector fields.

## Quick start guide
Simply type the attribute before public or serialized fields.
_The field will light up **red** if it can't find a component to fill it!_

```
using BGS.Attributes;
using UnityEngine;

public class MyBehaviour : MonoBehaviour
{
    [Get] public Rigidbody2D rb2d;
    [GetInChildren] public SpriteRenderer spriteRenderer;
    [GetInParent] public Collider2D collider;
    [GetOrAdd] public NetworkIdentity networkID;
    [Find] public GameManager gameManager;
    
    ...
}
```
