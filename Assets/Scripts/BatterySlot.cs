using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BatterySlot : MonoBehaviour
{
    [SerializeField] private bool isCharger;
    [SerializeField] private Transform batteryPosition;
    [SerializeField] private List<PluggableObject> pluggedObjects;
    private Battery currentBattery;

    public event EventHandler OnBatteryConnected;
    public event EventHandler OnBatteryDisconnected;

    public Transform BatteryPosition => batteryPosition;

    public bool IsBatteryConnected => currentBattery != null;

    public bool IsCharger => isCharger;

    private void Awake()
    {
        if (pluggedObjects == null)
        {
            pluggedObjects = new List<PluggableObject>();
        }
        foreach (var pluggedObject in pluggedObjects)
        {
            pluggedObject.SetSlot(this);
        }
    }

    public void ConnectBattery(Battery battery)
    {
        currentBattery = battery;
        OnBatteryConnected?.Invoke(this, EventArgs.Empty);
    }
    public void DisconnectBattery()
    {
        currentBattery = null;
        OnBatteryDisconnected?.Invoke(this, EventArgs.Empty);
    }
}
