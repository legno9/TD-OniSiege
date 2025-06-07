using System.Collections.Generic;
using UnityEngine;

public class AreaDamageProjectile : Projectile
{
    [Header("Area Damage Settings")]
    [SerializeField] protected float hitThreshold = 0.2f;
    protected Vector3 _fixedTargetPosition;
    protected Vector3 _fixedDirection;
    protected float _areaEffectRadius;

    public override void Initialize(Enemy target, float damage)
    {
        base.Initialize(target, damage);
        
        AreaEffectProjectileConfig areaProjectileConfig = projectileConfig as AreaEffectProjectileConfig;
        
        if (!areaProjectileConfig) return;
        _areaEffectRadius = areaProjectileConfig.areaEffectRadius;
        
        if (!target) return;
        _fixedTargetPosition = target.transform.position;
        _fixedDirection = (_fixedTargetPosition - transform.position).normalized;
    }
    
    protected override void Move()
    {
        if (_reachedTarget) return;
        
        transform.position += _fixedDirection * (_speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, _fixedTargetPosition) <= hitThreshold  && !_reachedTarget)
        {
            ReachedTarget();
            _reachedTarget = true;
            return;
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
                enemy.MakeDamage(_damage);
            }
        }

        // SpawnPool.Instance.Spawn(impactEffectPrefab, transform.position, Quaternion.identity);

        SpawnPool.Instance.Despawn(transform);
    }
}
