using UnityEngine;
using System;

[RequireComponent(typeof(Collider))]
public class HealthSystem : MonoBehaviour, IDamageable, IHealable
{
    public event Action<Transform> OnHealthDepleted;
    public event Action<float> OnHealthChanged;
    public bool HealthDepleted { get; private set; }
    public float Health { get; private set; }
    public float MaxHealth { get; private set; }
    
    private bool _canBeHealed = true;
    private float _predictedHealth;

    public void Initialize(float maxHealth, bool canBeHealed = true) 
    {
        HealthDepleted = false;
        _canBeHealed = canBeHealed;
        MaxHealth = maxHealth;
        Health = MaxHealth;
        _predictedHealth = MaxHealth;
    }

    private void Update()
    {
        if (HealthDepleted) return;
        if (Health >= 0) return;
        HealthDepleted = true;
        OnHealthDepleted?.Invoke(transform);
    }

    public bool MakeDamage(float damage) 
    {
        if (HealthDepleted) return false;
        
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
