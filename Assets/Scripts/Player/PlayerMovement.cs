using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 2;
    [SerializeField] private float climbingSpeed = 1;

    public Vector2 Velocity => rigidbody.velocity;

    private Rigidbody2D rigidbody;
    private float xInput;
    private float yInput;
    private float defaultGravity;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        defaultGravity = rigidbody.gravityScale;
    }

    private void Update()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate()
    {
        float yVelocity = rigidbody.velocity.y;
        if (Player.Instance.IsOnLadder)
        {
            yVelocity = yInput * climbingSpeed;
            rigidbody.gravityScale = 0;
        }
        else
        {
            rigidbody.gravityScale = defaultGravity;
        }
        rigidbody.velocity = new Vector2(xInput * movementSpeed, yVelocity);
    }
}
