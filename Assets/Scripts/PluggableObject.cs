using UnityEngine;

public class PluggableObject : MonoBehaviour
{
    protected BatterySlot batterySlot;
    protected bool isConnected => batterySlot != null && batterySlot.IsBatteryConnected;
    public void SetSlot(BatterySlot slot)
    {
        if (batterySlot != null)
        {
            batterySlot.OnBatteryConnected -= Slot_OnBatteryConnected;
            batterySlot.OnBatteryDisconnected -= Slot_OnBatteryDisconnected;
        }
        batterySlot = slot;

        slot.OnBatteryConnected += Slot_OnBatteryConnected;
        slot.OnBatteryDisconnected += Slot_OnBatteryDisconnected;
    }

    protected virtual void Slot_OnBatteryDisconnected(object sender, System.EventArgs e)
    {
        Debug.Log("Disconnected");
    }

    protected virtual void Slot_OnBatteryConnected(object sender, System.EventArgs e)
    {
        Debug.Log("Connected");
    }
}
