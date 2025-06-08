using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaDamageProjectile : Projectile
{
    private static readonly int Reached = Animator.StringToHash("Reached");

    [Header("Area Damage Settings")]
    [SerializeField] protected float hitThreshold = 0.2f;
    [SerializeField] protected Animator explosionAnimator;
    [SerializeField] protected SpriteRenderer mainSpriteRenderer;
    protected Vector3 _fixedTargetPosition;
    protected Vector3 _fixedDirection;
    protected float _areaEffectRadius;
    protected float projectileImpactAnimationDuration = 0.56f;

    public override void Initialize(Enemy target, float damage)
    {
        base.Initialize(target, damage);
        
        AreaEffectProjectileConfig areaProjectileConfig = projectileConfig as AreaEffectProjectileConfig;
        
        if (!areaProjectileConfig) return;
        _areaEffectRadius = areaProjectileConfig.areaEffectRadius;
        
        if (!target) return;
        _fixedTargetPosition = target.transform.position;
        _fixedDirection = (_fixedTargetPosition - transform.position).normalized;
        
        explosionAnimator.gameObject.SetActive(false);
        mainSpriteRenderer.enabled = true;
        
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

        if (explosionAnimator)
        {
            explosionAnimator.gameObject.SetActive(true);
            mainSpriteRenderer.enabled = false;
            
            Vector3 explosionScale = Vector3.one * _areaEffectRadius;
            explosionAnimator.transform.localScale = explosionScale;
            explosionAnimator.SetTrigger(Reached);
            
            StartCoroutine(Despawn(projectileImpactAnimationDuration));
        }
        else
        {
            StartCoroutine(Despawn(0.1f));
        }
    }

    private IEnumerator Despawn(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnPool.Instance.Despawn(transform);
    }
}
