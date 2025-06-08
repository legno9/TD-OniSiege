using System.Collections.Generic;
using UnityEngine;
using System;


[CreateAssetMenu(fileName = "NewAllTurretsConfig", menuName = "Core/All Turrets Config", order = 0)]
public class AllTurretsConfig : ScriptableObject
{
    [Serializable]
    public struct TurretConfigEntry
    {
        public TurretType type;
        public TurretConfig config;
    }
    
    [SerializeField] private List<TurretConfigEntry> turretConfigsEntries = new();
    
    private Dictionary<TurretType, TurretConfig> _turretConfigMap;
    public IReadOnlyDictionary<TurretType, TurretConfig> TurretConfig => _turretConfigMap;


    private void OnEnable()
    {
        _turretConfigMap = new Dictionary<TurretType, TurretConfig>();

        foreach (var entry in turretConfigsEntries)
        {
            if (entry.type == TurretType.None)
            {
                Debug.LogWarning($"AllTurretsConfig: Entry with type 'None' found for config '{entry.config?.name}'. Skipping.", this);
                continue;
            }
            if (!entry.config)
            {
                Debug.LogWarning($"AllTurretsConfig: Null config found for type '{entry.type}'. Skipping.", this);
                continue;
            }

            if (!_turretConfigMap.TryAdd(entry.type, entry.config))
            {
                Debug.LogWarning($"AllTurretsConfig: Duplicate entry for type '{entry.type}' found. Only the first will be used.", this);
            }
            
        }
    }
}
