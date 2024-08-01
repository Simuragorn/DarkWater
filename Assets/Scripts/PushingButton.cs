using Assets.Scripts.Helpers;
using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PushingButton : PluggableObject
{
    public enum ButtonPositionEnum
    {
        End,
        Start
    }

    public enum DirectionEnum
    {
        FromLeftToRight,
        FromRightToLeft,
    }

    [SerializeField] private float xMoveDistance = 1f;
    [SerializeField] private float positionDelay = 0.5f;
    [SerializeField] private float positionCheckingOffset = 0.001f;
    [SerializeField] private float gizmosRadius = 0.05f;
    [SerializeField] private float pushingVelocity = 1f;
    [SerializeField] private DirectionEnum pushingDirection;

    private float startLocalXPosition;
    private float endLocalXPosition;

    private Collider2D collider;
    private Rigidbody2D rigidbody;
    private float positionDelayLeft = 0;
    private ButtonPositionEnum? previousPosition;

    private void Awake()
    {
        collider = GetComponent<Collider2D>();
        rigidbody = GetComponent<Rigidbody2D>();
        startLocalXPosition = transform.localPosition.x;
        float xDirectionSign = pushingDirection == DirectionEnum.FromLeftToRight ? 1 : -1;
        endLocalXPosition = startLocalXPosition + xDirectionSign * xMoveDistance;
    }

    private void Update()
    {
        positionDelayLeft = Mathf.Max(0, positionDelayLeft - Time.deltaTime);
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        var minLocalPosition = transform.localPosition;
        var maxLocalPosition = transform.localPosition;
        float xDirectionSign = pushingDirection == DirectionEnum.FromLeftToRight ? 1 : -1;
        maxLocalPosition.x += xMoveDistance * xDirectionSign;

        Gizmos.DrawWireSphere(transform.parent.TransformPoint(minLocalPosition), gizmosRadius);
        Gizmos.DrawWireSphere(transform.parent.TransformPoint(maxLocalPosition), gizmosRadius);
    }

    private ButtonPositionEnum? GetCurrentPosition()
    {
        Vector2 localPosition = transform.localPosition;
        float distanceToStart = Mathf.Abs(localPosition.x - startLocalXPosition);
        float distanceToEnd = Mathf.Abs(localPosition.x - endLocalXPosition);
        if (distanceToStart <= positionCheckingOffset)
        {
            return ButtonPositionEnum.Start;
        }
        if (distanceToEnd <= positionCheckingOffset)
        {
            return ButtonPositionEnum.End;
        }
        return null;
    }

    private void HandleMovement()
    {
        Vector2 newLocalPosition = transform.localPosition;

        if (positionDelayLeft > 0)
        {
            return;
        }
        float movingDirection = GetMovingDirection();
        newLocalPosition.x += movingDirection;
        newLocalPosition = Vector2.Lerp(transform.localPosition, newLocalPosition, Time.fixedDeltaTime * pushingVelocity);
        float min = startLocalXPosition;
        float max = endLocalXPosition;
        if (pushingDirection == DirectionEnum.FromRightToLeft)
        {
            min = endLocalXPosition;
            max = startLocalXPosition;
        }
        newLocalPosition.x = Mathf.Clamp(newLocalPosition.x, min, max);

        rigidbody.MovePosition(transform.parent.TransformPoint(newLocalPosition));
        HandlePositionChanging(newLocalPosition);
    }

    private float GetMovingDirection()
    {
        if (!collider.isTrigger)
        {
            if ((Player.Instance.XInput >= 0 && pushingDirection == DirectionEnum.FromLeftToRight) ||
                (Player.Instance.XInput <= 0 && pushingDirection == DirectionEnum.FromRightToLeft))
            {
                return Player.Instance.XInput;
            }
        }
        return pushingDirection == DirectionEnum.FromLeftToRight ? -1 : 1;
    }

    private Vector2 HandlePositionChanging(Vector2 localPosition)
    {
        ButtonPositionEnum? currentPosition = GetCurrentPosition();
        if (currentPosition != null && previousPosition != currentPosition)
        {
            string positionText = "unknown position";

            switch (currentPosition.Value)
            {
                case ButtonPositionEnum.End:
                    localPosition.x = endLocalXPosition;
                    positionText = "end position";
                    break;
                case ButtonPositionEnum.Start:
                    localPosition.x = startLocalXPosition;
                    positionText = "start position";
                    break;
                default:
                    break;
            }
            transform.localPosition = localPosition;
            positionDelayLeft = positionDelay;
            Debug.Log(positionText);
        }

        previousPosition = currentPosition;
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
