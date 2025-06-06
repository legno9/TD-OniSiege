using UnityEngine;

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
    
    public static event System.Action<Enemy> OnEnemyDied;
    public static event System.Action<Enemy> OnEnemyReachedEnd;
    
    public float GoldValue { get; private set; }
    public int PlayerDamage { get; private set; }

    private bool _dead = false;
    
    
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
    }
    
    public void ApplyConfig(Vector2[] pathPoints)
    {
        if (!config)
        {
            Debug.LogError("EnemyConfig is null for EnemyBase.", this);
            return;
        }
        
        if (pathPoints is not { Length: > 1 })
        {
            Debug.LogError("Enemy path points are not set or too few! Cannot initialize movement.", this);
            return;
        }
        
        animator.SetLibrary(config.SpriteLibraryAsset);
        movement.OnDirectionChanged+= animator.SetDirection;
        movement.OnReachedEnd += ReachedEnd;
        movement.SetPath(pathPoints);
        
        healthSystem.Initialize(config.MaxHealth, config.CanBeHealed);
        healthSystem.OnHealthDepleted += Die;
        statusEffect.Initialize(config.MaxSpeed);
        
        GoldValue = config.GoldValue;
        PlayerDamage = config.PlayerDamage;
    }
    private void Update()
    {
        if (_dead) return;
        movement.Move(statusEffect.GetEffectiveSpeed());
    }
    private void Die()
    {
        if (_dead) return;
        _dead = true;
        
        if (animator)
        {
            // _animator.SetTrigger("Die");
        }
        
        // if (GetComponent<Collider2D>()) GetComponent<Collider2D>().enabled = false;

        OnEnemyDied?.Invoke(this);
    }
    
    private void ReachedEnd()
    {
        OnEnemyReachedEnd?.Invoke(this);
        Die();
    }

    private void OnDisable()
    {
        if (!movement || !animator) return;
        
        movement.OnDirectionChanged -= animator.SetDirection;
        movement.OnReachedEnd -= ReachedEnd;
    }
}
