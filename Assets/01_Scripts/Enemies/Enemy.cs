using UnityEngine;

[RequireComponent(typeof(EnemyMovement))]
[RequireComponent(typeof(HealthSystem))]
[RequireComponent(typeof(EnemyStatusEffect))]
public class Enemy : MonoBehaviour
{
    [SerializeField] protected EnemyConfig config;
    [SerializeField] protected EnemyAnimator animator;
    [SerializeField] protected EnemyMovement movement;
    [SerializeField] protected HealthSystem healthSystem;
    [SerializeField] protected EnemyStatusEffect statusEffect;
    
    public static event System.Action<Enemy> OnEnemyDied;
    public static event System.Action<Enemy> OnEnemyReachedEnd;
    
    public float GoldValue { get; private set; }
    public int PlayerDamage { get; private set; }
    
    protected bool _dead = false;
    
    private void Awake()
    {
        if (!animator) animator = GetComponentInChildren<EnemyAnimator>();
        if (!movement) movement = GetComponent<EnemyPathMovement>();
        if (!healthSystem) healthSystem = GetComponent<HealthSystem>();
        if (!statusEffect) statusEffect = GetComponent<EnemyStatusEffect>();
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
        
        animator.SetLibrary(config.spriteLibraryAsset);
        movement.OnDirectionChanged+= animator.SetDirection;
        movement.OnReachedEnd += ReachedEnd;
        movement.SetPath(pathPoints);
        
        healthSystem.Initialize(config.maxHealth, config.canBeHealed);
        healthSystem.OnHealthDepleted += Die;
        statusEffect.Initialize(config.maxSpeed);
        
        GoldValue = config.goldValue;
        PlayerDamage = config.playerDamage;
    }
    private void Update()
    {
        if (_dead) return;
        movement.Move(statusEffect.GetEffectiveSpeed());
    }
    
    public float GetPathProgress()
    {
        return !movement ? 0f : movement.GetPathProgress();
    }
    
    public bool IsPredictedDead()
    {
        return _dead || (healthSystem && healthSystem.IsPredictedDead());
    }
    
    public virtual void TakeDamage(float damage)
    {
        if (_dead) return;

        if (!healthSystem) return;
        
        healthSystem.MakeDamage(damage);
    }

    public void PredictDamage(float damage)
    {
        if (IsPredictedDead()) return;
        healthSystem.PredictDamage(damage);
    }
    
    public void ReduceSpeed(float factor, float duration)
    {
        if (_dead) return;
        statusEffect.ApplySpeedReduction(factor, duration);
    }
    
    public virtual bool IsTargetable()
    {
        return !IsPredictedDead() && gameObject.activeInHierarchy;
    }
    
    private void Die()
    {
        if (_dead) return;
        _dead = true;
        
        if (animator)
        {
            // _animator.SetTrigger("Die");
        }
        
        OnEnemyDied?.Invoke(this);
    }
    
    private void ReachedEnd()
    {
        OnEnemyReachedEnd?.Invoke(this);
        Die();
    }

    protected void OnDisable()
    {
        if (!movement || !animator) return;
        
        movement.OnDirectionChanged -= animator.SetDirection;
        movement.OnReachedEnd -= ReachedEnd;
    }
}
