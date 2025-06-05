using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Animator))]
public class Enemy : MonoBehaviour
{
    [SerializeField] protected  EnemyConfig enemyConfig;
    [SerializeField] protected  Animator animator;
    
    public float MaxHealth { get; private set; }
    public float CurrentHealth { get; private set; }
    
    private Vector2[] _pathPoints;

    protected float _predictedHealth = 0.0f;
    protected float _distanceAlongPath = 0.0f;
    protected float _maxSpeed = 0.0f;
    protected float _currentSpeed = 0.0f;
    protected float _speedReductionDuration = 0.0f;
    protected float _goldValue = 0.0f;
    protected int _playerDamage = 0;
    protected float _totalPathLength = 0.0f;
    protected int _currentPathIndex = 0;
    protected bool _isDead = false;
    
    protected static readonly int IsMoving = Animator.StringToHash("IsMoving");
    
    protected virtual void Awake()
    {
        if (!animator)
        {
            animator = GetComponent<Animator>();
        }
        
        ApplyConfig(enemyConfig);

        if (_pathPoints is not { Length: > 0 }) return;
        transform.position = _pathPoints[0];
        
        if (!animator) return;
        animator.SetBool(IsMoving, true);
    }
    
    private void ApplyConfig(EnemyConfig configData)
    {
        if (!configData)
        {
            Debug.LogError("EnemyConfig is null for EnemyBase.", this);
            return;
        }

        MaxHealth = configData.maxHealth;
        CurrentHealth = MaxHealth;
        _goldValue = configData.goldValue;
        _predictedHealth = MaxHealth;
        _maxSpeed = configData.maxSpeed;
        _currentSpeed = _maxSpeed;
        _playerDamage = configData.playerDamage;
    }
    
    public void SetPathLength( Vector2[] pathPoints )
    {
        _pathPoints = pathPoints;
        _totalPathLength = 0.0f;
        
        if (_pathPoints is not { Length: > 1 }) return;
        
        for (int i = 0; i < _pathPoints.Length - 1; ++i)
        {
            _totalPathLength += Vector2.Distance(_pathPoints[i], _pathPoints[i + 1]);
        }
    }
    
    protected virtual void Update()
    {
        if (_isDead) return;
        
        Move(Time.deltaTime);
        
        if (CurrentHealth <= 0 && !_isDead)
        {
            _isDead = true;
            Die();
            return;
        }
        
        if (_speedReductionDuration <= 0.0f) return;
        
        _speedReductionDuration -= Time.deltaTime;
        
        if (_speedReductionDuration >= 0.0f) return;
        
        _currentSpeed = _maxSpeed;
    }
    
    protected virtual void Move(float deltaTime)
    {
        if (_pathPoints == null || _pathPoints.Length < 2) return;

        float distanceToMove = _currentSpeed * deltaTime;
        Vector2 lastMovementDirection = Vector2.zero;

        while (distanceToMove > 0.0f && _currentPathIndex < _pathPoints.Length - 1)
        {
            if (_currentPathIndex >= _pathPoints.Length - 1)
            {
                transform.position = _pathPoints[^1];
                _distanceAlongPath = _totalPathLength;
                _isDead = true;
                // Removed: GameObjectManager.Instance.Player.ReceiveDamage(_playerDamage);
                Debug.Log("Enemy reached end of path! Player would take damage.", this); 
                Destroy(gameObject);
                return;
            }

            Vector2 targetPoint = _pathPoints[_currentPathIndex + 1];
            Vector2 vectorToTarget = targetPoint - (Vector2)transform.position;
            float distanceToTarget = vectorToTarget.magnitude;

            if (distanceToTarget < 0.001f)
            {
                _currentPathIndex++;
                transform.position = targetPoint;
                continue;
            }

            Vector2 normalizedDirection = vectorToTarget.normalized;
            // if (_animator != null) { SetDirectionAnimation(normalizedDirection); }

            if (distanceToMove >= distanceToTarget)
            {
                transform.position = targetPoint;
                distanceToMove -= distanceToTarget;
                _distanceAlongPath += distanceToTarget;
                _currentPathIndex++;
                lastMovementDirection = normalizedDirection;
            }
            else
            {
                transform.position += (Vector3)(normalizedDirection * distanceToMove);
                _distanceAlongPath += distanceToMove;
                lastMovementDirection = normalizedDirection;
                distanceToMove = 0.0f;
                break;
            }
        }
    }

    public virtual void ReceiveDamage(float damage)
    {
        if (_isDead) return;
        
        CurrentHealth -= damage;
        CurrentHealth = Mathf.Max(0, CurrentHealth);  
        
        // Hit effect or sound
        Debug.Log($"Enemy took {damage} damage. Health: {CurrentHealth}", this);
    }
    
    public virtual void ReduceSpeed(float reductionFactor, float speedReductionDuration)
    {
        if (_isDead) return;
        
        float newSpeed = _maxSpeed * (1.0f - reductionFactor);
        if (newSpeed > _currentSpeed) return;
        
        _currentSpeed = newSpeed;
        _speedReductionDuration = speedReductionDuration;
        Debug.Log($"Enemy speed reduced to {_currentSpeed} for {_speedReductionDuration}s.", this);
    }


    public virtual void Die()
    {
        if (_isDead) return;
        if (animator)
        {
            // _animator.SetTrigger("Die");
        }
        //Colliders
        Debug.Log($"Enemy died! Player would gain {_goldValue} gold.", this);
        Destroy(gameObject);
    }
    
    public void PredictDamage(float damageAmount)
    {
        _predictedHealth -= damageAmount;
    }
    
    public bool IsPredictedDead()
    {
        return _predictedHealth <= 0.0f;
    }
}
