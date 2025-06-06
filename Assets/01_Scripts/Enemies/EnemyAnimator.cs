using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.U2D.Animation;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(SpriteLibrary))]
[RequireComponent(typeof(SpriteResolver))]
public class EnemyAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteLibrary library;

    private static readonly int XDirection = Animator.StringToHash("XDirection");
    private static readonly int YDirection = Animator.StringToHash("YDirection");
    
    private void Awake()
    {
        if (!animator)
        {
            animator = GetComponent<Animator>();
        }
        if (!library)
        {
            library = GetComponent<SpriteLibrary>();
        }
    }

    public void SetLibrary(SpriteLibraryAsset configLibrary)
    {
        if (!library) return;
        library.spriteLibraryAsset = configLibrary;
    }
    
    public void SetDirection(Vector2 direction)
    {
        if (!animator) return;

        direction.Normalize();

        animator.SetFloat(XDirection, direction.x);
        animator.SetFloat(YDirection, direction.y);
    }

    // public void TriggerHit()
    // {
    //     if (animator != null)
    //     {
    //         animator.SetTrigger(HitTriggerHash);
    //     }
    // }
    //
    // public void TriggerDeath()
    // {
    //     if (animator != null)
    //     {
    //         animator.SetTrigger(HasDiedHash);
    //     }
    // }
    
}
