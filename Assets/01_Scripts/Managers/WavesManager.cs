using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WavesManager : MonoBehaviour
{
    public static WavesManager Instance { get; private set; }
    
    [SerializeField] private MapManager levelMapManager;
    [SerializeField] private EnemyWavesConfig waveConfigs;
    
    private readonly List<Enemy> _spawnedEnemies = new List<Enemy>();
    public IReadOnlyList<Enemy> SpawnedEnemies => _spawnedEnemies;
    
    private Vector2[] _enemyPathPoints;
    private int _currentWaveIndex = -1;
    private Coroutine _waveSpawnCoroutine;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (!levelMapManager)
        {
            levelMapManager = FindFirstObjectByType<MapManager>();
            if (!levelMapManager)
            {
                Debug.LogError("WavesManager: No MapManager found in scene! Cannot get enemy path. Disabling.", this);
                return;
            }
        }
        
        _enemyPathPoints = levelMapManager.EnemyPathPointsPos;
        
        if (_enemyPathPoints == null || _enemyPathPoints.Length < 2)
        {
            Debug.LogError("WavesManager: Enemy path points are not initialized or too few! Check MapManager setup. Disabling.", this);
            return;
        }
        
        Enemy.OnEnemyDied += HandleEnemyDied;
        Enemy.OnEnemyReachedEnd += HandleEnemyReachedEnd;
        
        StartNextWave();
    }

    private void OnDestroy()
    {
        Enemy.OnEnemyDied -= HandleEnemyDied;
        Enemy.OnEnemyReachedEnd -= HandleEnemyReachedEnd;

        if (_waveSpawnCoroutine != null)
        {
            StopCoroutine(_waveSpawnCoroutine);
        }
    }
    
    private void StartNextWave()
    {
        _currentWaveIndex++;

        if (_currentWaveIndex >= waveConfigs.waves.Count)
        {
            return;
        }
        
        _waveSpawnCoroutine = StartCoroutine(SpawnWaveCoroutine(waveConfigs.waves[_currentWaveIndex]));
    }

    private IEnumerator SpawnWaveCoroutine(EnemyWavesConfig.EnemyWaveType waveConfig)
    {
        if (!waveConfig.enemyPrefab)
        {
            Debug.LogWarning($"WavesManager: Enemy prefab is null in wave {_currentWaveIndex + 1}. Skipping wave.", this);
            _waveSpawnCoroutine = null;
            yield break;
        }
        
        yield return new WaitForSeconds(waveConfig.waveDelayAfterPrevious);

        for (int i = 0; i < waveConfig.count; i++)
        {
            SpawnEnemy(waveConfig.enemyPrefab);
            if (i < waveConfig.count - 1)
            {
                yield return new WaitForSeconds(waveConfig.timeBetweenSpawns);
            }
        }
        
        _waveSpawnCoroutine = null;
        StartNextWave();
    }
    
    private void SpawnEnemy(GameObject enemyPrefab)
    {
        Vector3 spawnPosition = _enemyPathPoints[0];
        
        GameObject newEnemyGo = SpawnPool.Instance.Spawn(enemyPrefab.transform, spawnPosition, Quaternion.identity, Vector3.one, transform).gameObject;
        Enemy newEnemy = newEnemyGo.GetComponent<Enemy>();
        
        if (newEnemy)
        {
            newEnemy.ApplyConfig(_enemyPathPoints);
            _spawnedEnemies.Add(newEnemy);
        }
        else
        {
            SpawnPool.Instance.Despawn(newEnemyGo.transform);
        }
    }
    
    private void HandleEnemyDied(Enemy enemyInstance)
    {
        _spawnedEnemies.Remove(enemyInstance);
        SpawnPool.Instance.Despawn(enemyInstance.transform);
        
        // GameManager.Instance.AddGold(goldValue);
    }

    private void HandleEnemyReachedEnd(Enemy enemyInstance)
    {
        // GameManager.Instance.TakeDamage(playerDamage);
        Debug.Log($"Enemy {enemyInstance.name} reached end. Player would take {enemyInstance.PlayerDamage} damage.");
    }

    

}
