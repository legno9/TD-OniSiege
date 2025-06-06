using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(EnemyAnimator))]
[RequireComponent(typeof(EnemyMovement))]
[RequireComponent(typeof(HealthSystem))]
[RequireComponent(typeof(EnemyStatusEffect))]
public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyConfig config;
    [SerializeField] private EnemyAnimator animator;
    [SerializeField] private EnemyMovement movement;
    [SerializeField] private HealthSystem healthSystem;
    [SerializeField] private EnemyStatusEffect statusEffect;
    
    private float _goldValue;
    private int _playerDamage;
    
    private void Awake()
    {
        if (!animator)
        {
            animator = GetComponentInChildren<EnemyAnimator>();
        }
        if (!movement)
        {
            movement = GetComponent<EnemyPathMovement>();
        }
        
        if (!healthSystem)
        {
            healthSystem = GetComponent<HealthSystem>();
        }
        if (!statusEffect)
        {
            statusEffect = GetComponent<EnemyStatusEffect>();
        }
        ApplyConfig();
    }
    
    private void ApplyConfig()
    {
        if (!config)
        {
            Debug.LogError("EnemyConfig is null for EnemyBase.", this);
            return;
        }

        animator.SetLibrary(config.SpriteLibraryAsset);
        movement.OnDirectionChanged+= animator.SetDirection;
        
        healthSystem.Initialize(config.MaxHealth, config.CanBeHealed);
        statusEffect.Initialize(config.MaxSpeed);
        
        _goldValue = config.GoldValue;
        _playerDamage = config.PlayerDamage;
    }
    
    private void Update()
    {
        if (healthSystem.HealthDepleted) return;
        movement.Move(statusEffect.GetEffectiveSpeed());
    }
    public void Die()
    {
        if (animator)
        {
            // _animator.SetTrigger("Die");
        }
        //Colliders
        Debug.Log($"Enemy died! Player would gain {_goldValue} gold.", this);
        Destroy(gameObject);
    }

    private void OnDisable()
    {
        if (movement && animator)
        {
            movement.OnDirectionChanged -= animator.SetDirection;
        }
    }
}
