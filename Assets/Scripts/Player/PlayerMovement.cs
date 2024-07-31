using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 2;
    [SerializeField] private float climbingSpeed = 1;

    public Vector2 Velocity => rigidbody.velocity;
    public float XInput => xInput;

    private Rigidbody2D rigidbody;
    private float xInput;
    private float yInput;
    private float defaultGravity;
    private Submarine submarine;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        defaultGravity = rigidbody.gravityScale;
        submarine = Submarine.Instance;
    }

    private void Update()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate()
    {
        float xVelocity = xInput * movementSpeed;
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
        if (Player.Instance.IsOnTerminal)
        {
            xVelocity = 0;
        }

        rigidbody.velocity = new Vector2(xVelocity, yVelocity);
    }
}
