using UnityEngine;

public class SubmarineMotor : MonoBehaviour
{
    [SerializeField] private Switcher motorSwitcher;
    [SerializeField] private float motorSpeed = 2;

    private void FixedUpdate()
    {
        Vector2 targetPos = transform.position + transform.right * Mathf.Sign(motorSwitcher.SwitchValue);
        float maxDistance = Mathf.Abs(motorSwitcher.SwitchValue) * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, targetPos, maxDistance);
    }
}
