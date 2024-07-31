using Assets.Scripts.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Radar : PluggableObject
{
    [SerializeField] private float radarReloadingTime = 10;
    [SerializeField] private Camera radarCamera;
    [SerializeField] private Color pointColor = Color.red;
    [SerializeField] private LayerMask targetLayerMask;
    [SerializeField] private RadarPoint pointPrefab;
    [SerializeField] private Transform sweepTransform;
    [SerializeField] private float rotationSpeed = 90f;
    [SerializeField] private float radarDistance = 5;

    private List<Collider2D> collidersList = new();
    private float scanDurationTime;
    private float scanDurationLeft = 0;

    private float radarReloadingTimeLeft = 0;

    private void Awake()
    {
        scanDurationTime = 360 / rotationSpeed;
        StopScanning();
    }

    private void Update()
    {
        HandleScanning();
    }

    protected override void Slot_OnPowerDisconnected(object sender, EventArgs e)
    {
        base.Slot_OnPowerDisconnected(sender, e);
        StopScanning();
    }

    public bool TryStartScanning()
    {
        if (radarReloadingTimeLeft <= 0)
        {
            StartScanning();
            return true;
        }
        return false;
    }

    private void StartScanning()
    {
        radarReloadingTimeLeft = radarReloadingTime;
        scanDurationLeft = scanDurationTime;
        sweepTransform.gameObject.SetActive(true);
    }

    private void StopScanning()
    {
        scanDurationLeft = 0;
        sweepTransform.gameObject.SetActive(false);
    }

    private void HandleScanning()
    {
        bool scanningWasActive = scanDurationLeft > 0;
        scanDurationLeft = Math.Max(scanDurationLeft - Time.deltaTime, 0);
        radarReloadingTimeLeft = Math.Max(radarReloadingTimeLeft - Time.deltaTime, 0);
        if (!hasPowerSupply || scanDurationLeft <= 0)
        {
            if (scanningWasActive)
            {
                StopScanning();
            }
            return;
        }

        float previousRotation = (sweepTransform.eulerAngles.z % 360) - 180;
        sweepTransform.eulerAngles += new Vector3(0, 0, -rotationSpeed * Time.deltaTime);
        float currentRotation = (sweepTransform.eulerAngles.z % 360) - 180;
        RaycastHit2D[] raycastHits = Physics2D.RaycastAll(Submarine.Instance.transform.position, MathUtils.GetVectorFromAngle(sweepTransform.eulerAngles.z), radarDistance, targetLayerMask);

        if (previousRotation < 0 && currentRotation >= 0)
        {
            collidersList.Clear();
        }
        foreach (var hit in raycastHits)
        {
            if (hit.collider != null)
            {
                if (!collidersList.Contains(hit.collider))
                {
                    collidersList.Add(hit.collider);
                    RadarPoint point = Instantiate(pointPrefab, hit.transform.position, Quaternion.identity);
                    point.Init(pointColor, 3, radarCamera.orthographicSize);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (Submarine.Instance == null)
        {
            return;
        }
        Gizmos.color = Color.red;
        Gizmos.DrawLine(Submarine.Instance.transform.position, Submarine.Instance.transform.position + MathUtils.GetVectorFromAngle(sweepTransform.eulerAngles.z) * radarDistance);
    }
}
