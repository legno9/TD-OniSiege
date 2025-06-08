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
    
    public static event System.Action<Enemy> OnEnemyDiedToPlayer;
    public static event System.Action<Enemy> OnEnemyReachedEnd;
    
    public int GoldValue { get; private set; }
    public int PlayerDamage { get; private set; }

    private bool _dead = false;
    
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
        healthSystem.OnHealthDepleted += DieToPlayer;
        statusEffect.Initialize(config.maxSpeed);
        
        GoldValue = config.goldValue;
        PlayerDamage = config.playerDamage;
        _dead = false;
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
    
    public virtual void MakeDamage(float damage)
    {
        if (_dead) return;

        if (!healthSystem) return;
        
        healthSystem.MakeDamage(damage);
    }

    public void PredictDamage(float damage)
    {
        healthSystem.PredictDamage(damage);
    }
    
    public bool IsPredictedDead()
    {
        return _dead || (healthSystem && healthSystem.IsPredictedDead());
    }
    
    public virtual bool IsTargetable()
    {
        return !IsPredictedDead() && gameObject.activeInHierarchy;
    }
    
    public void ReduceSpeed(float factor, float duration)
    {
        if (_dead) return;
        statusEffect.ApplySpeedReduction(factor, duration);
    }
    
    private void DieToPlayer()
    {
        if (_dead) return;
        Dissapear();
        OnEnemyDiedToPlayer?.Invoke(this);
    }
    
    private void ReachedEnd()
    {
        if (_dead) return;
        Dissapear();
        OnEnemyReachedEnd?.Invoke(this);
    }
    
    private void Dissapear()
    {
        _dead = true;
        
        if (animator)
        {
            // _animator.SetTrigger("Die");
        }
    }

    protected void OnDisable()
    {
        if (!movement || !animator) return;
        
        movement.OnDirectionChanged -= animator.SetDirection;
        movement.OnReachedEnd -= ReachedEnd;
    }
}
