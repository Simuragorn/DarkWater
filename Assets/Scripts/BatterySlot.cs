using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BatterySlot : MonoBehaviour
{
    [SerializeField] private bool isCharger;
    [SerializeField] private Transform batteryPosition;
    private Battery currentBattery;

    public Transform BatteryPosition => batteryPosition;

    public bool CanBeUsedForBattery => currentBattery == null;

    public bool IsCharger => isCharger;

    public void ConnectBattery(Battery battery)
    {
        currentBattery = battery;
    }
    public void DisconnectBattery()
    {
        currentBattery = null;
    }
}
