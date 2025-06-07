using UnityEngine;

[RequireComponent(typeof(TurretProjectileSpawner))]
public class ShooterTurret : Turret
{
    [SerializeField] private TurretProjectileSpawner projectileSpawner;
    
    protected override void Awake()
    {
        base.Awake();
        if (!projectileSpawner) projectileSpawner = GetComponent<TurretProjectileSpawner>();
    }
    
    protected override void PerformAction()
    {
        if (!_currentTarget || _currentTarget.IsPredictedDead())
        {
            return;
        }
        
        _animator.TriggerAttack();
        _currentTarget.PredictDamage(_currentDamage);
    }

    protected void Shoot()
    {
        projectileSpawner.SpawnProjectile(_projectilePrefab, _currentTarget, _currentDamage);
    }
    
    protected override void OnEnable()
    {
        base.OnEnable();
        if (!_animator) return;
        _animator.OnAttackAnimationShoot += Shoot;
    }
    
    protected void OnDisable()
    {
        if (!_animator) return;
        _animator.OnAttackAnimationShoot -= Shoot;
    }
    
    
}
