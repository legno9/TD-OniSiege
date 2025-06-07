using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] protected ProjectileConfig projectileConfig;
    
    protected float _speed;
    protected Enemy _target;
    protected float _damage;
    
    protected bool _reachedTarget;

    public virtual void Initialize(Enemy target, float damage)
    {
        _target = target;
        _damage = damage;
        _reachedTarget = false;
        
        if (!projectileConfig) return;
        _speed = projectileConfig.projectileSpeed;
    }

    protected virtual void Update()
    {
        Move();
    }

    protected abstract void Move();
    protected abstract void ReachedTarget();
}
