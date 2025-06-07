using UnityEngine;
using System;

public class HealthSystem : MonoBehaviour, IDamageable, IHealable
{
    public event Action OnHealthDepleted;
    public event Action<float> OnHealthChanged;
    public float Health { get; private set; }
    public float MaxHealth { get; private set; } = 1;
    
    private bool _canBeHealed = true;
    private float _predictedHealth;
    private bool _healthDepleted = false;

    public void Initialize(float maxHealth, bool canBeHealed = true) 
    {
        _healthDepleted = false;
        _canBeHealed = canBeHealed;
        MaxHealth = maxHealth;
        Health = MaxHealth;
        _predictedHealth = MaxHealth;
    }

    private void Update()
    {
        if (_healthDepleted) return;
        if (Health >= 0) return;
        _healthDepleted = true;
        OnHealthDepleted?.Invoke();
    }

    public bool MakeDamage(float damage) 
    {
        if (_healthDepleted) return false;
        
        Health -= damage;
        OnHealthChanged?.Invoke(Health);

        if (!(Health <= 0)) return true;
        
        Health = 0;

        return true;
    }

    public bool MakeHeal (float heal) 
    {
        if (!_canBeHealed) return false;
        OnHealthChanged?.Invoke(Health);
        
        if (Health + heal >= MaxHealth) return true;
        Health += heal;
        
        return true;
    }
    
    public void PredictDamage(float damageAmount)
    {
        _predictedHealth -= damageAmount;
    }
    
    public void PredictHeal(float healAmount)
    {
        if (!_canBeHealed) return;
        _predictedHealth += healAmount;
    }
    
    public bool IsPredictedDead()
    {
        return _predictedHealth <= 0.0f;
    }
}
