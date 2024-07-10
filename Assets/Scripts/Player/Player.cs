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

    public float XInput => playerMovement.XInput;
    public bool IsOnLadder => isClimbing;
    private bool isClimbing;

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
        }

        var newObject = collision.GetComponent<PickingObject>();
        if (newObject!=null && !objectsForPicking.Contains(newObject))
        {
            objectsForPicking.Add(newObject);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Ladder>() != null)
        {
            isClimbing = false;
        }

        var newObject = collision.GetComponent<PickingObject>();
        if (newObject!=null && objectsForPicking.Contains(newObject))
        {
            objectsForPicking.Remove(newObject);
        }
    }

    private void Update()
    {
        isInteraction = Input.GetKey(KeyCode.Space);
        HandlePicking();

    }

    private void HandlePicking()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (pickedObject != null && !isClimbing)
            {
                pickedObject.Drop();
                pickedObject = null;
            }
            else if (pickedObject == null && objectsForPicking.Any() && !isClimbing)
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
}
