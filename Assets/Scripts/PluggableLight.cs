using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class PluggableLight : PluggableObject
{
    [SerializeField] private float changingIntensitySpeed = 1;
    private Light2D lightSource;
    private float maxIntesity;
    private void Awake()
    {
        lightSource = GetComponent<Light2D>();
        maxIntesity = lightSource.intensity;
        if (!isConnected)
        {
            lightSource.intensity = 0;
        }
    }

    private void Update()
    {
        int sign = isConnected ? 1 : -1;
        float newIntensity = lightSource.intensity + sign * Time.deltaTime * changingIntensitySpeed;
        newIntensity = Math.Clamp(newIntensity, 0, maxIntesity);
        lightSource.intensity = newIntensity;
    }

    protected override void Slot_OnBatteryConnected(object sender, EventArgs e)
    {
        base.Slot_OnBatteryConnected(sender, e);
    }

    protected override void Slot_OnBatteryDisconnected(object sender, EventArgs e)
    {
        base.Slot_OnBatteryDisconnected(sender, e);
    }
}
