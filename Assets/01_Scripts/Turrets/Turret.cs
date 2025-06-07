using UnityEngine;

[RequireComponent(typeof(TurretTargeting))]
[RequireComponent(typeof(Collider2D))]
public abstract class Turret : MonoBehaviour
{
    [SerializeField] protected TurretConfig _turretConfig;
    [SerializeField] protected TurretAnimator _animator;
    [SerializeField] protected TurretTargeting _targeting;
    
    public int _currentUpgradeCost { get; private set; }
    public int _currentSellValue { get; private set; }
    
    protected float _currentDamage;
    protected float _currentActionRange;
    protected float _currentActionRate;
    protected Enemy _currentTarget = null;
    protected GameObject _projectilePrefab = null;
    
    private int _currentLevel = 1;
    private float _actionTimer = 0f; 
    protected virtual void Awake()
    {
        if (!_animator) _animator = GetComponentInChildren<TurretAnimator>();
        if (!_targeting) _targeting = GetComponent<TurretTargeting>();
        
        
        _projectilePrefab = _turretConfig.projectilePrefab;
    }

    protected virtual void OnEnable()
    {
        ApplyCurrentLevelConfig();
    }

    protected virtual void Update()
    {
        _actionTimer += Time.deltaTime;
        
        _currentTarget = _targeting.FindTarget(_currentActionRange);
        
        if (!_currentTarget || _actionTimer < _currentActionRate) return;
        
        PerformAction();
        _actionTimer -= _currentActionRate;
        
        Vector2 directionToTarget = _currentTarget.transform.position - transform.position;
        _animator.SetDirection(directionToTarget.normalized);
    }
    
    private void ApplyCurrentLevelConfig()
    {
        if (!_turretConfig)
        {
            Debug.LogError("TurretConfig is null for Turret!", this);
            return;
        }

        TurretConfig.TurretLevelData levelData = _turretConfig.GetLevelData(_currentLevel);
        
        if (levelData == null)
        {
            Debug.LogError($"Turret: Could not find data for level {_currentLevel}.", this);
            return;
        }

        _currentDamage = levelData.damage;
        _currentActionRange = levelData.actionRange;
        _currentActionRate = levelData.actionRate;
        _currentUpgradeCost = levelData.upgradeCost;
        _currentSellValue = levelData.sellValue;
        
        _animator.SetLibrary(levelData.spriteLibraryAsset);
    }

    protected abstract void PerformAction();

    
    public void Upgrade()
    {
        if (_currentLevel >= _turretConfig.maxLevel)
        {
            Debug.Log("Turret is already at max level.", this);
            return;
        }
        
        _currentLevel++;
        ApplyCurrentLevelConfig();
    }
}
