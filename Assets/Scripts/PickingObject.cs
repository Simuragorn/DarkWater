using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PickingObject : MonoBehaviour
{
    [SerializeField] private int pickingDisplayOrder = 60;
    protected int defaultDisplayOrder;
    protected float posYOffset;
    protected bool isPickedUp;
    protected Collider2D pickingCollider;
    protected SpriteRenderer spriteRenderer;
    protected virtual void Awake()
    {
        pickingCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultDisplayOrder = spriteRenderer.sortingOrder;
    }

    public bool CanBePickedUp => !isPickedUp;

    public virtual void PickUp(Transform newParent)
    {
        posYOffset = transform.position.y - newParent.transform.position.y;
        transform.position = newParent.transform.position;
        transform.parent = newParent;
        spriteRenderer.sortingOrder = pickingDisplayOrder;
        isPickedUp = true;
    }

    public virtual void Drop()
    {
        Vector2 newPosition = transform.position;
        newPosition.y = transform.position.y + posYOffset;
        transform.position = newPosition;
        transform.parent = Submarine.Instance.transform;
        spriteRenderer.sortingOrder = defaultDisplayOrder;
        isPickedUp = false;
    }
}
