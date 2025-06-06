using UnityEngine;
using System.Collections;

public class EnemyStatusEffect : MonoBehaviour
{
    private float _baseSpeed;
    private float _currentSpeedMultiplier = 1.0f; 

    private float _speedReductionDuration = 0.0f;

    private Coroutine _speedEffectCoroutine;

    // private bool _isStunned = false;
    // private float _stunDuration = 0.0f;
    
    public void Initialize(float baseSpeed)
    {
        _baseSpeed = baseSpeed;
        _currentSpeedMultiplier = 1.0f;
        _speedReductionDuration = 0.0f;

        if (_speedEffectCoroutine == null) return;
        
        StopCoroutine(_speedEffectCoroutine);
        _speedEffectCoroutine = null;
    }
    
    public void ApplySpeedReduction(float reductionFactor, float duration)
    {
        float newMultiplier = Mathf.Clamp01(1.0f - reductionFactor); 
        
        if (Mathf.Approximately(newMultiplier, _currentSpeedMultiplier))
        {
            _speedReductionDuration = Mathf.Max(_speedReductionDuration, duration);
        }
        
        if (newMultiplier < _currentSpeedMultiplier || _speedReductionDuration <= 0f)
        {
            if (_speedEffectCoroutine != null)
            {
                StopCoroutine(_speedEffectCoroutine);
            }

            _currentSpeedMultiplier = newMultiplier;
            _speedReductionDuration = duration;
            _speedEffectCoroutine = StartCoroutine(SpeedReductionTimer());
        }
    }

    private IEnumerator SpeedReductionTimer()
    {
        yield return new WaitForSeconds(_speedReductionDuration);

        _currentSpeedMultiplier = 1.0f;
        _speedReductionDuration = 0.0f;
        _speedEffectCoroutine = null;
    }

    
    public float GetEffectiveSpeed()
    {
        return _baseSpeed * _currentSpeedMultiplier;
    }

    
    // public void ApplyStun(float duration)
    // {
    //     if (_isStunned) return;
    //     _isStunned = true;
    // }

}
