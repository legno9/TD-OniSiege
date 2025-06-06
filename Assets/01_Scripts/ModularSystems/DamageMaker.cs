using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DamageMaker : MonoBehaviour
{
    public float damage = 1;
    public bool canDamageEquals = false;
    public event Action<Transform> OnDamaged;

    private bool _triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (_triggered) return;
        
        if (GetRootParent(other.transform).name == GetRootParent(this.transform).name)
        {
            if (!canDamageEquals)
            {
                return;
            }
        }

        HealthSystem healthSystem = other.GetComponent<HealthSystem>();
        
        if (!healthSystem) return;
        bool damaged = healthSystem.MakeDamage(damage);

        if (!damaged) return;
        _triggered = true;
        OnDamaged?.Invoke(this.transform);
    }

    private Transform GetRootParent(Transform objectTransform)
    {
        while (objectTransform.parent)
        {
            objectTransform = objectTransform.parent;
        }
        return objectTransform;
    }

    private void OnTriggerExit(Collider other) 
    {
        _triggered = false;
    }

    public void OnDespawned()
    {
        _triggered = false;
    }

}
