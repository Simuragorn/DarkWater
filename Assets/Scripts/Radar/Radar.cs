using Assets.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour
{
    [SerializeField] private Camera radarCamera;
    [SerializeField] private Color pointColor = Color.red;
    [SerializeField] private LayerMask targetLayerMask;
    [SerializeField] private RadarPoint pointPrefab;
    [SerializeField] private Transform sweepTransform;
    [SerializeField] private float rotationSpeed = 90f;
    [SerializeField] private float radarDistance = 5;

    private List<Collider2D> collidersList = new();

    private void Update()
    {
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
