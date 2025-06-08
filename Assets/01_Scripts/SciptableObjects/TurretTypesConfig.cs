﻿using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTurretTypesConfig", menuName = "Core/Turret Types Config", order = 0)]
public class TurretTypesConfig : ScriptableObject
{
    [Serializable]
    public struct TurretPrefabEntry
    {
        public TurretType type;
        public GameObject turretPrefab;
        public int initialCost;
    }
    
    [SerializeField] private List<TurretPrefabEntry> turretPrefabEntries = new();

    private Dictionary<TurretType, GameObject> _turretPrefabsMap;
    private Dictionary<TurretType, int> _turretCostsMap;
    public IReadOnlyDictionary<TurretType, GameObject> TurretPrefabs => _turretPrefabsMap;
    public IReadOnlyDictionary<TurretType, int> TurretCosts => _turretCostsMap;


    private void OnEnable()
    {
        _turretPrefabsMap = new Dictionary<TurretType, GameObject>();
        _turretCostsMap = new Dictionary<TurretType, int>();

        foreach (var entry in turretPrefabEntries)
        {
            if (entry.type == TurretType.None)
            {
                Debug.LogWarning($"TurretTypesConfig: Entry with type 'None' found for prefab '{entry.turretPrefab?.name}'. Skipping.", this);
                continue;
            }
            if (!entry.turretPrefab)
            {
                Debug.LogWarning($"TurretTypesConfig: Null prefab found for type '{entry.type}'. Skipping.", this);
                continue;
            }

            if (!_turretPrefabsMap.TryAdd(entry.type, entry.turretPrefab))
            {
                Debug.LogWarning($"TurretTypesConfig: Duplicate entry for type '{entry.type}' found. Only the first will be used.", this);
            }
            
            if (!_turretCostsMap.TryAdd(entry.type, entry.initialCost))
            {
                Debug.LogWarning($"TurretTypesConfig: Duplicate cost entry for type '{entry.type}' found. Only the first will be used.", this);
            }
        }
    }

    
}
