using UnityEngine;

public class PluggableObject : MonoBehaviour
{
    protected BatterySlot batterySlot;
    protected bool hasPowerSupply => batterySlot != null && batterySlot.IsChargedBatteryConnected;
    public void SetSlot(BatterySlot slot)
    {
        if (batterySlot != null)
        {
            batterySlot.OnPowerConnected -= Slot_OnPowerConnected;
            batterySlot.OnPowerDisconnected -= Slot_OnPowerDisconnected;
        }
        batterySlot = slot;

        slot.OnPowerConnected += Slot_OnPowerConnected;
        slot.OnPowerDisconnected += Slot_OnPowerDisconnected;
    }

    protected virtual void Slot_OnPowerDisconnected(object sender, System.EventArgs e)
    {
        Debug.Log("Disconnected");
    }

    protected virtual void Slot_OnPowerConnected(object sender, System.EventArgs e)
    {
        Debug.Log("Connected");
    }
}
