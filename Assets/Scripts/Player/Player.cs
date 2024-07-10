using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    private enum PickingActionEnum
    {
        None,
        Pick,
        Drop,
    }

    [SerializeField] private Transform pickingContainer;
    [SerializeField] private PlayerMovement playerMovement;
    public static Player Instance;

    public float XInput => playerMovement.XInput;
    public bool IsOnLadder => isClimbing;
    private bool isClimbing;

    public bool IsInteraction => isInteraction;
    private bool isInteraction;
    private PickingActionEnum pickingAction = PickingActionEnum.None;
    private PickingObject pickedObject;

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
            isClimbing = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Ladder>() != null)
        {
            isClimbing = false;
        }
    }

    private void Update()
    {
        isInteraction = Input.GetKey(KeyCode.Space);
        DefinePickingAction();

    }

    private void DefinePickingAction()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (pickingAction == PickingActionEnum.None && pickedObject != null && !isClimbing)
            {
                pickingAction = PickingActionEnum.Drop;
            }
            else if (pickedObject == null && !isClimbing)
            {
                pickingAction = PickingActionEnum.Pick;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        switch (pickingAction)
        {
            case PickingActionEnum.None:
                break;
            case PickingActionEnum.Pick:
                var battery = collision.GetComponent<PickingObject>();
                if (battery != null && battery.CanBePickedUp)
                {
                    battery.PickUp(pickingContainer);
                    pickedObject = battery;
                }
                break;
            case PickingActionEnum.Drop:
                pickedObject.Drop();
                pickedObject = null;
                break;
            default:
                break;
        }
        pickingAction = PickingActionEnum.None;

    }
}
