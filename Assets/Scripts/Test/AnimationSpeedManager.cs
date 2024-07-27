using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationSpeedManager : MonoBehaviour
{
    [Range(-1, 1)]
    [SerializeField] private float animationSpeed = 1;
    private Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        animator.SetFloat("mySpeed", animationSpeed);
    }
}
