using UnityEngine;
using System.Collections.Generic;

public class SpawnPool : MonoBehaviour
{
    [System.Serializable]
    public class PrefabPool
    {
        [Tooltip("The prefab GameObject for this pool.")]
        public GameObject prefabGo;
        
        [Tooltip("The number of instances to create and disable at startup.")]
        public int preloadAmount;

        private Queue<Transform> _despawnedInstances;
        private HashSet<Transform> _spawnedInstances;

        private bool _hasPreloaded = false;
        
        public PrefabPool(GameObject prefab)
        {
            prefabGo = prefab;
            preloadAmount = 0;
            InitializePool(); 
        }
        public PrefabPool() //For Unity serialization, not used directly
        {
        }

        private void InitializePool()
        {
            _spawnedInstances = new HashSet<Transform>();
            _despawnedInstances = new Queue<Transform>();
            _hasPreloaded = false;
        }
        
        public Transform SpawnInstance(Vector3 pos, Quaternion rot, Vector3 scale, Transform parent = null)
        {
            if (_despawnedInstances == null || _spawnedInstances == null)
            {
                InitializePool();
            }

            var instance = _despawnedInstances.Count > 0 ? 
                _despawnedInstances.Dequeue() :
                CreateMinimalInstance();
            
            _spawnedInstances.Add(instance);
            instance.SetParent(parent);
            
            if (parent)
            {
                instance.localPosition = pos;
                instance.localRotation = rot;
            }
            else
            {
                instance.SetPositionAndRotation(pos, rot);
            }
            
            instance.localScale = scale;
            instance.gameObject.SetActive(true);
            
            return instance;
        }
        
        private Transform CreateMinimalInstance()
        {
            GameObject newGo = Instantiate(prefabGo, Vector3.zero, Quaternion.identity);
            Transform newInstance = newGo.transform;
            
            return newInstance;
        }
        
        public void DespawnInstance(Transform instanceToDespawn)
        {
            if (!instanceToDespawn) return;

            if (!_spawnedInstances.Contains(instanceToDespawn))
            {
                Debug.LogWarning($"Attempted to despawn {instanceToDespawn.name} which was not tracked");
                return;
            }

            _spawnedInstances.Remove(instanceToDespawn);
            _despawnedInstances.Enqueue(instanceToDespawn);

            instanceToDespawn.gameObject.SetActive(false);
            instanceToDespawn.SetParent(null);
        }
        
        public void PreloadInstances()
        {
            if (_hasPreloaded) return;
            
            InitializePool();

            _hasPreloaded = true;

            for (int i = 0; i < preloadAmount; i++)
            {
                Transform newInstance = CreateMinimalInstance();
                _spawnedInstances.Add(newInstance);
                DespawnInstance(newInstance);
            }
        }
    }

    public static SpawnPool Instance { get; private set; }

    [SerializeField]
    private List<PrefabPool> configuredPrefabPools = new List<PrefabPool>();

    private readonly Dictionary<GameObject, PrefabPool> _prefabToPoolDict = new();
    private readonly Dictionary<Transform, PrefabPool> _spawnedInstancesMap = new();

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        foreach (var prefabPool in configuredPrefabPools)
        {
            if (!prefabPool.prefabGo)
            {
                Debug.LogWarning($"SpawnPool: A PrefabPool entry has a null prefabGO. Skipping.");
                continue;
            }
            prefabPool.PreloadInstances();
            
            if (!_prefabToPoolDict.TryAdd(prefabPool.prefabGo, prefabPool))
            {
                Debug.LogWarning($"SpawnPool: Duplicate prefabGO '{prefabPool.prefabGo.name}' found in configured pools. Only the first entry will be used.");
            }
        }
    }
    
    public Transform Spawn(Transform prefabTransform, Vector3 position, Quaternion rotation)
    {
        return Spawn(prefabTransform, position, rotation, Vector3.one);
    }
    
    public Transform Spawn(Transform prefabTransform, Transform parent)
    {
        return Spawn(prefabTransform, Vector3.zero, Quaternion.identity, Vector3.one, parent);
    }
    
    public Transform Spawn(Transform prefabTransform, Vector3 pos, Quaternion rot, Vector3 scale, Transform parent = null)
    {
        if (!prefabTransform)
        {
            Debug.LogError("SpawnPool: Attempted to spawn a null prefabTransform!", this);
            return null;
        }

        if (!_prefabToPoolDict.TryGetValue(prefabTransform.gameObject, out var targetPool))
        {
            Debug.LogWarning($"SpawnPool: Prefab '{prefabTransform.name}' not pre-configured. Creating new pool.", prefabTransform);
            targetPool = new PrefabPool(prefabTransform.gameObject);
            CreatePrefabPool(targetPool);
        }

        Transform instance = targetPool.SpawnInstance(pos, rot, scale, parent);
        _spawnedInstancesMap.Add(instance, targetPool);
        return instance;
    }
    
    public void CreatePrefabPool(PrefabPool prefabPool)
    {
        if (prefabPool == null || !prefabPool.prefabGo)
        {
            Debug.LogError("SpawnPool: Attempted to create a null PrefabPool or a pool with a null prefabGO.", this);
            return;
        }
        if (!_prefabToPoolDict.TryAdd(prefabPool.prefabGo, prefabPool))
        {
            Debug.LogWarning($"SpawnPool: Prefab '{prefabPool.prefabGo.name}' is already registered in the pool.", this);
        }
    }
    
    public void Despawn(Transform instanceToDespawn)
    {
        if (!instanceToDespawn) return;

        if (_spawnedInstancesMap.TryGetValue(instanceToDespawn, out var prefabPool))
        {
            prefabPool.DespawnInstance(instanceToDespawn);
            _spawnedInstancesMap.Remove(instanceToDespawn);
        }
        else
        {
            Debug.LogWarning($"SpawnPool: Attempted to despawn '{instanceToDespawn.name}' not tracked by SpawnPool. It will be destroyed.");
            Destroy(instanceToDespawn.gameObject);
        }
    }
}
