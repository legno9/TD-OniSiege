using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HealMaker : MonoBehaviour
{
    public float heal = 1;
    public event Action<Transform> OnHeal;
    private bool _triggered = false;
    
    private void OnTriggerEnter(Collider other)
    {
        if (_triggered) return;
        
        HealthSystem healthSystem = other.GetComponent<HealthSystem>();
        
        if (!healthSystem) return;
        
        bool healed = healthSystem.MakeHeal(heal);
        
        if (!healed) return; 
        
        _triggered = true;
        OnHeal?.Invoke(this.transform);
    }

    private void OnTriggerExit(Collider other) {
        
        _triggered = false;
    }

    public void OnDespawned()
    {
        _triggered = false;
    }
}
