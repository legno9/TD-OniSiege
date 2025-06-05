using UnityEngine;

public class WavesManager : MonoBehaviour
{
    [SerializeField] private MapManager levelMapManager;
    [SerializeField] private GameObject enemyPrefab;
    
    private Vector2[] _enemyPathPoints;
    
    private void Awake()
    {
        if (!levelMapManager)
        {
            Debug.LogError("WavesManager: No MapManager found in scene! Cannot get enemy path.", this);
            return;
        }

        _enemyPathPoints = levelMapManager.EnemyPathPointsPos;

        if (_enemyPathPoints == null || _enemyPathPoints.Length == 0)
        {
            Debug.LogError("WavesManager: Enemy path points are not initialized or empty! Check MapManager setup.", this);
        }
        else
        {
            Debug.Log($"WavesManager: Successfully loaded {_enemyPathPoints.Length} path points.");
            // StartNextWave();
        }
    }

    // public void SpawnEnemy(GameObject enemyPrefab, int startWaypointIndex = 0)
    // {
    //     if (_enemyPathPoints == null || _enemyPathPoints.Length == 0)
    //     {
    //         Debug.LogError("Cannot spawn enemy, no path points available.");
    //         return;
    //     }
    //
    //     // Instanciar el enemigo en la posición del primer waypoint
    //     GameObject newEnemyGO = Instantiate(enemyPrefab, _enemyPathPoints[startWaypointIndex], Quaternion.identity);
    //     Enemy newEnemy = newEnemyGO.GetComponent<Enemy>();
    //
    //     if (newEnemy != null)
    //     {
    //         newEnemy.SetPath(_enemyPathPoints); // Pasar la ruta completa al enemigo
    //         // También podrías pasarle solo el índice de inicio y el MapManager.
    //     }
    // }

}
