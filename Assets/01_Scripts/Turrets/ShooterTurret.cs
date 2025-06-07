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
            _isAttacking = false;
            _actionTimer = 0;
            return;
        }
        _animator.TriggerAttack();
        _currentTarget.PredictDamage(_currentDamage);
    }

    protected void Shoot() //Called by the animator when the attack animation is ready to shoot
    {
        projectileSpawner.SpawnProjectile(_projectilePrefab, _currentTarget, _currentDamage);
        _actionTimer = 0;
        _isAttacking = false;
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
