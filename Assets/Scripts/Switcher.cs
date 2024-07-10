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
    [SerializeField] private float gearSupportDelay = 2f;
    [SerializeField] private float gearCheckingOffset = 0.001f;
    [SerializeField] private float gizmosRadius = 0.05f;

    private float localXStartPosition;
    private float minLocalXPosition;
    private float maxLocalXPosition;

    private Collider2D collider;
    private Rigidbody2D rigidbody;
    private RigidbodyConstraints2D defaultRigidbodyContraints;
    private float gearDelayLeft = 0;
    private float gearSupportDelayLeft = 0;
    private GearEnum? previousGearPosition;


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
        if (!isConnected)
        {
            BlockMovement();
        }
    }

    private void FixedUpdate()
    {
        LimitMovement();
        float xPosition = transform.localPosition.x;
        xPosition = Mathf.Clamp(xPosition, minLocalXPosition, maxLocalXPosition);
        switchValue = MathHelper.MapValue(xPosition, minLocalXPosition, maxLocalXPosition, -1, 1);

        previousGearPosition = GetCurrentGear();
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

    protected override void Slot_OnBatteryDisconnected(object sender, EventArgs e)
    {
        base.Slot_OnBatteryDisconnected(sender, e);
        BlockMovement();
    }
    protected override void Slot_OnBatteryConnected(object sender, EventArgs e)
    {
        base.Slot_OnBatteryConnected(sender, e);
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
            return GearEnum.Highest;
        }
        if (distanceToRight <= gearCheckingOffset)
        {
            return GearEnum.Lowest;
        }
        return null;
    }

    private void LimitMovement()
    {
        Vector2 localPosition = transform.localPosition;
        localPosition = HandleGearChanging(localPosition);

        if (gearDelayLeft > 0)
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

    private Vector2 HandleGearChanging(Vector2 localPosition)
    {
        gearDelayLeft = Mathf.Max(0, gearDelayLeft - Time.deltaTime);
        gearSupportDelayLeft = Mathf.Max(0, gearSupportDelayLeft - Time.deltaTime);

        GearEnum? currentGear = GetCurrentGear();
        if (rigidbody.velocity.x != 0 && currentGear != null && previousGearPosition != currentGear && gearSupportDelayLeft <= 0)
        {
            string gearText = "unknown gear";
            switch (currentGear.Value)
            {
                case GearEnum.Highest:
                    localPosition.x = minLocalXPosition;
                    gearText = "highest gear";
                    break;
                case GearEnum.Neutral:
                    localPosition.x = localXStartPosition;
                    gearText = "neutral gear";
                    break;
                case GearEnum.Lowest:
                    localPosition.x = maxLocalXPosition;
                    gearText = "lowest gear";
                    break;
                default:
                    break;
            }
            transform.localPosition = localPosition;
            BlockMovement();
            gearDelayLeft = gearDelay;
            gearSupportDelayLeft = gearSupportDelay;
            Debug.Log(gearText);
        }
        else if (gearDelayLeft <= 0 && isConnected)
        {
            UnblockMovement();
        }

        return localPosition;
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
