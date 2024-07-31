using Assets.Scripts.Consts;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Battery : PickingObject
{
    [SerializeField] private float initialPowerLevel = 100;
    [SerializeField] private TextMeshProUGUI powerText;
    public bool IsCharged => currentPowerLevel > 0;
    public event EventHandler OnCharged;
    public event EventHandler OnDischarged;

    private List<BatterySlot> batterySlots = new List<BatterySlot>();
    BatterySlot currentSlot;
    private float currentPowerLevel;

    protected override void Awake()
    {
        base.Awake();
        currentPowerLevel = initialPowerLevel;
    }

    private void Update()
    {
        HandlePowerLevel();
    }

    private void HandlePowerLevel()
    {
        powerText.text = $"Power: {Math.Round(currentPowerLevel, 2)}";

        if (currentSlot == null)
        {
            return;
        }
        float oldPowerLevel = currentPowerLevel;
        float newPowerLevel = currentPowerLevel;
        if (currentSlot.IsCharger)
        {
            newPowerLevel += currentSlot.ChargingSpeed * Time.deltaTime;
        }
        else
        {
            newPowerLevel -= currentSlot.DischargingSpeed * Time.deltaTime;
        }
        currentPowerLevel = Mathf.Clamp(newPowerLevel, 0, PowerConstants.MaxBatteryPowerLevel);

        if (oldPowerLevel != currentPowerLevel)
        {
            if (oldPowerLevel == 0)
            {
                OnCharged?.Invoke(this, EventArgs.Empty);
            }
            else if (currentPowerLevel == 0)
            {
                OnDischarged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public override void PickUp(Transform newParent)
    {
        float oldPosYOffset = posYOffset;
        base.PickUp(newParent);
        if (currentSlot != null)
        {
            currentSlot.DisconnectBattery();
            posYOffset = oldPosYOffset;
            currentSlot = null;
        }
    }

    public override void Drop()
    {
        var suitableSlots = batterySlots.Where(bs => !bs.IsBatteryConnected).ToList();
        var newSlot = suitableSlots.OrderBy(o => Vector2.Distance(o.transform.position, transform.position)).FirstOrDefault();
        if (newSlot != null)
        {
            SetToSlot(newSlot);
        }
        else
        {
            base.Drop();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var newSlot = collision.GetComponent<BatterySlot>();
        if (newSlot != null && !batterySlots.Contains(newSlot))
        {
            batterySlots.Add(newSlot);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        var newSlot = collision.GetComponent<BatterySlot>();
        if (newSlot != null && batterySlots.Contains(newSlot))
        {
            batterySlots.Remove(newSlot);
        }
    }
    public virtual void SetToSlot(BatterySlot newSlot)
    {
        newSlot.ConnectBattery(this);
        isPickedUp = false;
        Transform newParent = newSlot.BatteryPosition.transform;
        posYOffset = transform.position.y - newParent.position.y;
        transform.position = newParent.position;
        transform.parent = newParent;
        spriteRenderer.sortingOrder = defaultDisplayOrder;
        currentSlot = newSlot;
    }
}
