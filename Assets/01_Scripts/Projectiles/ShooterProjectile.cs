using UnityEngine;

public class ShooterProjectile : Projectile
{
    [Header("Shooter Projectile Settings")]
    [SerializeField] private float hitThreshold = 0.02f;
    
    protected override void Move()
    {
        if (!_target || _target.IsPredictedDead())
        {
            SpawnPool.Instance.Despawn(transform);
            return;
        }

        Vector3 direction = (_target.transform.position - transform.position).normalized;
        transform.position += direction * (_speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, _target.transform.position) <= hitThreshold)
        {
            ReachedTarget();
        }
        
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    protected override void ReachedTarget()
    {
        if (_target && !_target.IsPredictedDead())
        {
            _target.TakeDamage(_damage);
        }

        // SpawnPool.Instance.Spawn(impactEffectPrefab, transform.position, Quaternion.identity);

        SpawnPool.Instance.Despawn(transform);
    }
}
