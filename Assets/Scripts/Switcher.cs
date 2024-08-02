using Assets.Scripts.Helpers;
using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Switcher : PluggableObject
{
    public enum GearEnum
    {
        Highest,
        Neutral,
        Lowest
    }

    [SerializeField] private float xOffset = 1f;
    [SerializeField] private float gearDelay = 0.5f;
    [SerializeField] private float gearCheckingOffset = 0.01f;
    [SerializeField] private float gizmosRadius = 0.05f;
    [SerializeField] private float pushingVelocity = 0.1f;

    private float localXStartPosition;
    private float minLocalXPosition;
    private float maxLocalXPosition;

    private Collider2D collider;
    private Rigidbody2D rigidbody;
    private RigidbodyConstraints2D defaultRigidbodyContraints;
    private float gearDelayLeft = 0;
    private GearEnum? previousGearPosition;


    /// <summary>
    /// Returns value from -1 to 1 of current switching position
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
        if (!hasPowerSupply)
        {
            BlockMovement();
        }
    }

    private void Update()
    {
        gearDelayLeft = Mathf.Max(0, gearDelayLeft - Time.deltaTime);
    }

    private void FixedUpdate()
    {
        HandleMovement();
        CalculateSwitchValue();
        previousGearPosition = GetCurrentGear();
    }

    private void CalculateSwitchValue()
    {
        GearEnum? currentGear = GetCurrentGear();
        if (currentGear == null)
        {
            float xPosition = transform.localPosition.x;
            xPosition = Mathf.Clamp(xPosition, minLocalXPosition, maxLocalXPosition);
            switchValue = MathHelper.MapValue(xPosition, minLocalXPosition, maxLocalXPosition, -1, 1);
            return;
        }

        switch (currentGear)
        {
            case GearEnum.Highest:
                switchValue = 1;
                break;
            case GearEnum.Lowest:
                switchValue = -1;
                break;
            case GearEnum.Neutral:
                switchValue = 0;
                break;
            default:
                break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        var minLocalPosition = transform.localPosition;
        minLocalPosition.x -= xOffset;
        var maxLocalPosition = transform.localPosition;
        maxLocalPosition.x += xOffset;

        Gizmos.DrawWireSphere(transform.parent.TransformPoint(minLocalPosition), gizmosRadius);
        Gizmos.DrawWireSphere(transform.parent.TransformPoint(maxLocalPosition), gizmosRadius);
    }

    protected override void Slot_OnPowerDisconnected(object sender, EventArgs e)
    {
        base.Slot_OnPowerDisconnected(sender, e);
        BlockMovement();
    }
    protected override void Slot_OnPowerConnected(object sender, EventArgs e)
    {
        base.Slot_OnPowerConnected(sender, e);
        UnblockMovement();
    }

    private GearEnum? GetCurrentGear()
    {
        Vector2 localPosition = transform.localPosition;
        float distanceToCenter = Mathf.Abs(localPosition.x - localXStartPosition);
        float distanceToLeft = Mathf.Abs(localPosition.x - minLocalXPosition);
        float distanceToRight = Mathf.Abs(localPosition.x - maxLocalXPosition);
        if (distanceToCenter <= gearCheckingOffset)
        {
            return GearEnum.Neutral;
        }
        if (distanceToLeft <= gearCheckingOffset)
        {
            return GearEnum.Lowest;
        }
        if (distanceToRight <= gearCheckingOffset)
        {
            return GearEnum.Highest;
        }
        return null;
    }

    private void HandleMovement()
    {
        if (gearDelayLeft > 0)
        {
            return;
        }
        Vector2 newLocalPosition = transform.localPosition;
        float movingDirection = GetMovingDirection();
        newLocalPosition.x += movingDirection;
        newLocalPosition = Vector2.Lerp(transform.localPosition, newLocalPosition, Time.fixedDeltaTime * pushingVelocity);
        newLocalPosition.x = Mathf.Clamp(newLocalPosition.x, minLocalXPosition, maxLocalXPosition);
        transform.position = transform.parent.TransformPoint(newLocalPosition);
        HandleGearChanging();
    }

    private float GetMovingDirection()
    {
        if (!collider.isTrigger)
        {
            return Player.Instance.XInput;
        }
        return 0;
    }

    private void HandleGearChanging()
    {
        GearEnum? currentGear = GetCurrentGear();
        if (currentGear != null && previousGearPosition != currentGear)
        {
            string gearText = "unknown gear";
            switch (currentGear.Value)
            {
                case GearEnum.Highest:
                    gearText = "highest gear";
                    break;
                case GearEnum.Neutral:
                    gearText = "neutral gear";
                    break;
                case GearEnum.Lowest:
                    gearText = "lowest gear";
                    break;
                default:
                    break;
            }
            BlockMovement();
            gearDelayLeft = gearDelay;
            Debug.Log(gearText);
        }
        else if (gearDelayLeft <= 0 && hasPowerSupply)
        {
            UnblockMovement();
        }
    }

    private void UnblockMovement()
    {
        rigidbody.constraints = defaultRigidbodyContraints;
    }

    private void BlockMovement()
    {
        rigidbody.constraints = defaultRigidbodyContraints | RigidbodyConstraints2D.FreezePositionX;
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
