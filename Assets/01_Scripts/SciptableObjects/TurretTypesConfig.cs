using System;
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
    }
    
    [SerializeField] private List<TurretPrefabEntry> turretPrefabEntries = new();

    private Dictionary<TurretType, GameObject> _turretPrefabsMap;
    public IReadOnlyDictionary<TurretType, GameObject> TurretPrefabs => _turretPrefabsMap;

    private void OnEnable()
    {
        _turretPrefabsMap = new Dictionary<TurretType, GameObject>();

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
        }
    }

    
}
