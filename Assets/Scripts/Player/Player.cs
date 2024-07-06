using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    public static Player Instance;

    public float XInput => playerMovement.XInput;
    public bool IsOnLadder => isOnLadder;
    private bool isOnLadder;

    public bool IsInteraction => isInteraction;
    private bool isInteraction;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Ladder>() != null)
        {
            isOnLadder = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Ladder>() != null)
        {
            isOnLadder = false;
        }
    }

    private void Update()
    {
        isInteraction = Input.GetKey(KeyCode.E);
    }
}
