using UnityEngine;

namespace Assets.Scripts.Utils
{
    public static class MathUtils
    {
        public static Vector3 GetVectorFromAngle(float angle)
        {
            float angleRad = angle * (Mathf.PI / 180f);
            return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        }
    }
}
