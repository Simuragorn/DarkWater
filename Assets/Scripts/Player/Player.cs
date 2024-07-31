using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [SerializeField] private Transform pickingContainer;
    [SerializeField] private PlayerMovement playerMovement;
    public static Player Instance;

    [SerializeField]
    private Terminal availableTerminal;

    public float XInput => playerMovement.XInput;
    public bool IsOnLadder => isClimbing;
    private bool isClimbing;

    [SerializeField]
    public bool IsOnTerminal => isOnTerminal;
    private bool isOnTerminal;

    public bool IsInteraction => isInteraction;
    private bool isInteraction;
    private PickingObject pickedObject;
    private List<PickingObject> objectsForPicking = new List<PickingObject>();

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
            return;
        }

        var newObject = collision.GetComponent<PickingObject>();
        if (newObject != null && !objectsForPicking.Contains(newObject))
        {
            objectsForPicking.Add(newObject);
            return;
        }

        Terminal closestTerminal = collision.GetComponent<Terminal>();
        if (closestTerminal != null)
        {
            availableTerminal = closestTerminal;
            return;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Ladder>() != null)
        {
            isClimbing = false;
            return;
        }

        var newObject = collision.GetComponent<PickingObject>();
        if (newObject != null && objectsForPicking.Contains(newObject))
        {
            objectsForPicking.Remove(newObject);
            return;
        }

        Terminal closestTerminal = collision.GetComponent<Terminal>();
        if (closestTerminal != null && availableTerminal == closestTerminal)
        {
            availableTerminal = null;
            isOnTerminal = false;
            return;
        }
    }

    private void Update()
    {
        isInteraction = Input.GetKey(KeyCode.Space);
        HandleTerminalInteraction();
        HandlePicking();
    }

    private void HandleTerminalInteraction()
    {
        if (isClimbing || pickedObject != null)
        {
            return;
        }
        if (!isOnTerminal && availableTerminal != null && Input.GetKeyDown(KeyCode.E))
        {
            isOnTerminal = availableTerminal.TryOpenTerminal();
        }
        else if (isOnTerminal && Input.GetKeyDown(KeyCode.Escape))
        {
            isOnTerminal = false;
            availableTerminal.CloseTerminal();
        }
    }

    private void HandlePicking()
    {
        if (isClimbing || isOnTerminal || !Input.GetKeyDown(KeyCode.E))
        {
            return;
        }
        if (pickedObject != null)
        {
            pickedObject.Drop();
            pickedObject = null;
        }
        else if (pickedObject == null && objectsForPicking.Any())
        {
            var suitableObjects = objectsForPicking.Where(o => o.CanBePickedUp).ToList();
            var newObject = suitableObjects.OrderBy(o => Vector2.Distance(o.transform.position, transform.position)).FirstOrDefault();
            if (newObject != null)
            {
                newObject.PickUp(pickingContainer);
                pickedObject = newObject;
            }
        }
    }
}
