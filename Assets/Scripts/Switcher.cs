using Assets.Scripts.Consts;
using Assets.Scripts.Helpers;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Switcher : MonoBehaviour
{
    [SerializeField] private float xOffset = 1f;
    [SerializeField] private float neutralGearDelay = 0.5f;
    [SerializeField] private float neutralGearSupportDelay = 2f;
    [SerializeField] private float gearCheckingOffset = 0.001f;

    private float localXStartPosition;
    private float minLocalXPosition;
    private float maxLocalXPosition;

    private Collider2D collider;
    private Rigidbody2D rigidbody;
    private RigidbodyConstraints2D defaultRigidbodyContraints;
    private float neutralGearDelayLeft = 0;
    private float neutralGearSupportDelayLeft = 0;


    /// <summary>
    /// Returns value from 0 to 1 of current switching position
    /// </summary>
    public float SwitchValue => switchValue;
    [SerializeField] private float switchValue;
    private void Awake()
    {
        collider = GetComponent<Collider2D>();
        rigidbody = GetComponent<Rigidbody2D>();
        localXStartPosition = transform.localPosition.x;

        minLocalXPosition = localXStartPosition - xOffset;
        maxLocalXPosition = localXStartPosition + xOffset;
        defaultRigidbodyContraints = rigidbody.constraints;
    }

    private void FixedUpdate()
    {
        LimitMovement();
        float xPosition = transform.localPosition.x;
        xPosition = Mathf.Clamp(xPosition, minLocalXPosition, maxLocalXPosition);
        switchValue = MathHelper.MapValue(xPosition, minLocalXPosition, maxLocalXPosition, -1, 1);
    }

    private void LimitMovement()
    {
        Vector2 localPosition = transform.localPosition;
        localPosition = HandleNeutralGear(localPosition);

        if (neutralGearDelayLeft > 0)
        {
            return;
        }

        localPosition.x = Mathf.Clamp(localPosition.x, minLocalXPosition, maxLocalXPosition);
        transform.localPosition = Vector2.Lerp(transform.localPosition, localPosition, Time.fixedDeltaTime * 10);

        if ((rigidbody.velocity.x < 0 && transform.localPosition.x <= minLocalXPosition) ||
            (rigidbody.velocity.x > 0 && transform.localPosition.x >= maxLocalXPosition))
        {
            rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
        }
    }

    private Vector2 HandleNeutralGear(Vector2 localPosition)
    {
        neutralGearDelayLeft = Mathf.Max(0, neutralGearDelayLeft - Time.deltaTime);
        neutralGearSupportDelayLeft = Mathf.Max(0, neutralGearSupportDelayLeft - Time.deltaTime);

        float distanceToCenter = Mathf.Abs(localPosition.x - localXStartPosition);
        if (rigidbody.velocity.x != 0 && distanceToCenter <= gearCheckingOffset && neutralGearSupportDelayLeft <= 0)
        {
            localPosition.x = localXStartPosition;
            transform.localPosition = localPosition;

            rigidbody.constraints = defaultRigidbodyContraints | RigidbodyConstraints2D.FreezePositionX;
            neutralGearDelayLeft = neutralGearDelay;
            neutralGearSupportDelayLeft = neutralGearSupportDelay;
        }
        else if (neutralGearDelayLeft <= 0)
        {
            rigidbody.constraints = defaultRigidbodyContraints;
        }

        return localPosition;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        var player = collision.gameObject.GetComponent<Player>();
        if (player == null)
        {
            return;
        }
        collider.isTrigger = !player.IsInteraction;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.gameObject.GetComponent<Player>();
        if (player == null)
        {
            return;
        }
        collider.isTrigger = !player.IsInteraction;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        var player = collision.gameObject.GetComponent<Player>();
        if (player == null)
        {
            return;
        }
        collider.isTrigger = true;
    }
}
