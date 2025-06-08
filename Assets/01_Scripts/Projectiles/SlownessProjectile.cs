using System.Collections.Generic;
using UnityEngine;


public class SlownessProjectile : Projectile
{
    private static readonly int Reached = Animator.StringToHash("Reached");
    
    [Header("Slowness Settings")]
    [SerializeField] protected float hitThreshold = 0.2f;
    [SerializeField] protected Animator effectAnimator;
    [SerializeField] protected SpriteRenderer mainSpriteRenderer;
    
    protected Vector3 _fixedTargetPosition;
    protected Vector3 _fixedDirection;
    protected float _areaEffectRadius;
    protected float _areaEffectDuration;
    protected float _slownessFactor;
    protected float _slownessDuration;
    protected bool _despawned = false;
    protected float _timer = 0f;

    public override void Initialize(Enemy target, float damage)
    {
        base.Initialize(target, damage);
        
        SlownessProjectileConfig slownessProjectileConfig = projectileConfig as SlownessProjectileConfig;
        
        if (!slownessProjectileConfig) return;
        _areaEffectRadius = slownessProjectileConfig.areaEffectRadius;
        _areaEffectDuration = slownessProjectileConfig.projectileDuration;
        _slownessFactor = slownessProjectileConfig.slownessFactor;
        _slownessDuration = slownessProjectileConfig.slownessDuration;
        
        if (!target) return;
        _fixedTargetPosition = target.transform.position;
        _fixedDirection = (_fixedTargetPosition - transform.position).normalized;
        
        effectAnimator.gameObject.SetActive(false);
        mainSpriteRenderer.enabled = true;
    }
    
    protected override void Update()
    {
        base.Update();
        
        if (!_reachedTarget && _despawned) return;
        
        _timer += Time.deltaTime;
        ReachedTarget();
        
        if (_timer < _areaEffectDuration) return;
        SpawnPool.Instance.Despawn(transform);
        _despawned = true;
    }
    
    protected override void Move()
    {
        if (_reachedTarget) return;
        
        transform.position += _fixedDirection * (_speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, _fixedTargetPosition) <= hitThreshold)
        {
            _reachedTarget = true;
            if (effectAnimator)
            {
                effectAnimator.gameObject.SetActive(true);
                mainSpriteRenderer.enabled = false;
            
                Vector3 explosionScale = Vector3.one * _areaEffectRadius;
                effectAnimator.transform.localScale = explosionScale;
                effectAnimator.SetTrigger(Reached);
            }
        }
        
        float angle = Mathf.Atan2(_fixedDirection.y, _fixedDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    protected override void ReachedTarget()
    {
        IReadOnlyList<Enemy> enemies = WavesManager.Instance.SpawnedEnemies;
        foreach (Enemy enemy in enemies)
        {
            if (!enemy) continue;

            float distance = Vector3.Distance(enemy.transform.position, transform.position);
            if (distance <= _areaEffectRadius)
            {
                enemy.ReduceSpeed(_slownessFactor, _slownessDuration);
            }
        }
    }
}

