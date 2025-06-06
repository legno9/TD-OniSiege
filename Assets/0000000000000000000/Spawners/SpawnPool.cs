using System.Collections.Generic;
using UnityEngine;

public class SpawnPool : MonoBehaviour {

    [System.Serializable]
    public class PrefabPool {
        public GameObject prefabGO;
        public int preloadAmount;

        private HashSet<Transform> _spawnedObjects = new HashSet<Transform>();
        private HashSet<Transform> _despawnedObjects = new HashSet<Transform>();

        private bool _preload = false;

        public PrefabPool(Transform prefab) {
            prefabGO = prefab.gameObject;
        }

        public Transform SpawnInstance(Vector3 pos, Quaternion rot, Vector3 scale, Transform parent = null) {
            Transform instance;

            if (_despawnedObjects.Count == 0) {
                instance = SpawnNew(pos, rot, parent);
            } else {
                HashSet<Transform>.Enumerator despawnedEnumerator = this._despawnedObjects.GetEnumerator();
                despawnedEnumerator.MoveNext();
                instance = despawnedEnumerator.Current;

                _despawnedObjects.Remove(instance);
                _spawnedObjects.Add(instance);

                if (instance)
                {
                    instance.parent = parent;
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
                }
            }

            instance.gameObject.SetActive(true);
            return instance;
        }

        public Transform SpawnNew(Vector3 pos, Quaternion rot, Transform parent = null) {
            GameObject instGO;
            instGO = GameObject.Instantiate(this.prefabGO, pos, rot, parent);
            if (parent != null) {
                instGO.transform.localPosition = pos;
                instGO.transform.localRotation = rot;
            }
            Transform inst = instGO.transform;

            // Start tracking the new instance
            this._spawnedObjects.Add(inst);

            return inst;
        }

        public void DespawnInstance(Transform trans) {
            this._spawnedObjects.Remove(trans);
            this._despawnedObjects.Add(trans);
            trans.parent = null;
            trans.gameObject.SetActive(false);
        }

        public void PreloadInstances() {
            if (!_preload) {

                _preload = true;

                _spawnedObjects = new HashSet<Transform>();
                _despawnedObjects = new HashSet<Transform>();

                for (int i = 0; i < preloadAmount; i++) {
                    Transform trans = SpawnNew(Vector3.zero, Quaternion.identity);
                    DespawnInstance(trans);
                }
            }
        }

    }


    public static SpawnPool Instance;

    [SerializeField]
    private List<PrefabPool> prefabPools = new List<PrefabPool>();
    public List<PrefabPool> PrefabPools {
        get {
            return prefabPools;
        }
    }

    private Dictionary<GameObject, PrefabPool> _prefabToPoolDict = new();
    private Dictionary<Transform, PrefabPool> _spawned = new();

    private void Awake() {
        if (!Instance) 
        {
            Instance = this;
        } 
        else if (Instance) 
        {
            Destroy(this);
        }
        
        DontDestroyOnLoad(gameObject);

        foreach (var p in prefabPools)
        {
            p.PreloadInstances();
            _prefabToPoolDict.TryAdd(p.prefabGO, p);
        }
    }

    public Transform Spawn(Transform prefab, Vector3 position, Quaternion rotation) {
        return Spawn(prefab, position, rotation, Vector3.one);
    }

    public Transform Spawn(Transform prefab, Transform parent) {
        return Spawn(prefab, Vector3.zero, Quaternion.identity, Vector3.one, parent);
    }

    public Transform Spawn(Transform prefab, Vector3 pos, Quaternion rot, Vector3 scale, Transform parent = null) {
        Transform inst;
        if (_prefabToPoolDict.TryGetValue(prefab.gameObject, out var prefabPool)) {
            inst = prefabPool.SpawnInstance(pos, rot, scale, parent);
            this._spawned.Add(inst, prefabPool);
            return inst;
        }

        PrefabPool newPrefabPool = new PrefabPool(prefab);
        CreatePrefabPool(newPrefabPool);

        inst = newPrefabPool.SpawnInstance(pos, rot, scale, parent);

        this._spawned.Add(inst, newPrefabPool);

        return inst;
    }

    public void CreatePrefabPool(PrefabPool prefabPool) {
        this._prefabToPoolDict.Add(prefabPool.prefabGO, prefabPool);
    }

    public void Despawn(Transform instance) {
        if (this._spawned.TryGetValue(instance, out var prefabPool)) {
            prefabPool.DespawnInstance(instance);
        }

        _spawned.Remove(instance);

    }
}
