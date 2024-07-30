using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class RadarPoint : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private float disappearTimer;
    private float disappearTimerMax;
    private Color color;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        disappearTimerMax = 1f;
        disappearTimer = 0f;
        color = new Color(1, 1, 1, 1f);
    }

    void Update()
    {
        disappearTimer += Time.deltaTime;
        color.a = Mathf.Lerp(1, 0f, disappearTimer / disappearTimerMax);
        spriteRenderer.color = color;

        if (disappearTimer >= disappearTimerMax)
        {
            Destroy(gameObject);
        }
    }

    public void Init(Color newColor, float newDisappearTimerMax, float radarCameraSize)
    {
        color = newColor;
        disappearTimerMax = newDisappearTimerMax;
        disappearTimer = 0;
        transform.localScale *= radarCameraSize / 2;
    }
}
