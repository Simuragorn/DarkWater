using UnityEngine;

public class SubmarineMotor : MonoBehaviour
{
    [SerializeField] private Switcher motorSwitcher;
    [SerializeField] private Switcher buoyancySwitcher;
    [SerializeField] private float motorSpeed = 2;

    [SerializeField] private float smoothTime = 1f;

    private float currentMotorValue;
    private float currentBuoyancyValue;
    private float motorVelocity;
    private float buoyancyVelocity;

    private void FixedUpdate()
    {
        currentMotorValue = Mathf.SmoothDamp(currentMotorValue, motorSwitcher.SwitchValue, ref motorVelocity, smoothTime);
        currentBuoyancyValue = Mathf.SmoothDamp(currentBuoyancyValue, buoyancySwitcher.SwitchValue, ref buoyancyVelocity, smoothTime);

        Vector2 targetPos = transform.position;
        Vector2 horizontalMovement = transform.right * currentMotorValue;
        Vector2 verticalMovement = transform.up * currentBuoyancyValue;
        targetPos = targetPos + horizontalMovement + verticalMovement;
        float maxDistance = motorSpeed * Time.deltaTime * Mathf.Max(Mathf.Abs(currentMotorValue), Mathf.Abs(currentBuoyancyValue));

        //CONFLICTS WITH rigidbody.MovePosition
        //transform.position = Vector2.MoveTowards(transform.position, targetPos, maxDistance);
    }
}
