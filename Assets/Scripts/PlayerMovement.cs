using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5;

    private Rigidbody2D rigidbody;
    private float xInput;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        xInput = Input.GetAxisRaw("Horizontal");
    }

    private void FixedUpdate()
    {
        rigidbody.velocity = new Vector2(xInput * movementSpeed, rigidbody.velocity.y);
    }
}
