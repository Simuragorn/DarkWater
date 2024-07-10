using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Battery : PickingObject
{
    private List<BatterySlot> batterySlots = new List<BatterySlot>();    
    BatterySlot currentSlot;

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
        var suitableSlots = batterySlots.Where(bs => bs.CanBeUsedForBattery).ToList();
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
