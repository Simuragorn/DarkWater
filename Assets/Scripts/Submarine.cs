using UnityEngine;

public class Submarine : MonoBehaviour
{
    [SerializeField] private SubmarineMotor submarineMotor;

    public static Submarine Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
}
