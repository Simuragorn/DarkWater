using Assets.Scripts.Consts;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BatterySlot : MonoBehaviour
{
    [SerializeField] private GameObject chargingEffects;
    [SerializeField] private bool isCharger;
    [SerializeField] private float chargingTimeInSeconds = 5;
    [SerializeField] private float dischargingTimeInSeconds = 20;

    [SerializeField] private Transform batteryPosition;
    [SerializeField] private List<PluggableObject> pluggedObjects;
    private Battery currentBattery;

    public event EventHandler OnPowerConnected;
    public event EventHandler OnPowerDisconnected;

    public Transform BatteryPosition => batteryPosition;

    private float chargingSpeed;
    private float dischargingSpeed;
    public float ChargingSpeed => chargingSpeed;
    public float DischargingSpeed => dischargingSpeed;

    public bool IsBatteryConnected => currentBattery != null;
    public bool IsChargedBatteryConnected => currentBattery != null && currentBattery.IsCharged;

    public bool IsCharger => isCharger;

    private void Awake()
    {
        chargingSpeed = PowerConstants.MaxBatteryPowerLevel / chargingTimeInSeconds;
        dischargingSpeed = PowerConstants.MaxBatteryPowerLevel / dischargingTimeInSeconds;
        if (pluggedObjects == null)
        {
            pluggedObjects = new List<PluggableObject>();
        }
        foreach (var pluggedObject in pluggedObjects)
        {
            pluggedObject.SetSlot(this);
        }
        chargingEffects.SetActive(false);
    }

    private void Battery_OnDischarged(object sender, EventArgs e)
    {
        chargingEffects.SetActive(false);
        OnPowerDisconnected?.Invoke(this, EventArgs.Empty);
    }

    private void Battery_OnCharged(object sender, EventArgs e)
    {
        chargingEffects.SetActive(true);
        OnPowerConnected?.Invoke(this, EventArgs.Empty);
    }

    public void ConnectBattery(Battery battery)
    {
        currentBattery = battery;
        battery.OnCharged += Battery_OnCharged;
        battery.OnDischarged += Battery_OnDischarged;
        if (battery.IsCharged)
        {
            chargingEffects.SetActive(true);
            OnPowerConnected?.Invoke(this, EventArgs.Empty);
        }
    }

    public void DisconnectBattery()
    {
        currentBattery.OnCharged -= Battery_OnCharged;
        currentBattery.OnDischarged -= Battery_OnDischarged;
        currentBattery = null;

        chargingEffects.SetActive(false);
        OnPowerDisconnected?.Invoke(this, EventArgs.Empty);
    }
}
