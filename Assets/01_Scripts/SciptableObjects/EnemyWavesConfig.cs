using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewEnemyWavesConfig", menuName = "Game/Enemy Waves Config")]
public class EnemyWavesConfig : ScriptableObject
{
    [Header("Waves Settings")]
    public List<EnemyWaveType> waves;

    [System.Serializable]
    public class EnemyWaveType
    {
        [Tooltip("Delay in seconds before this wave starts after the previous one finishes.")]
        public float waveDelayAfterPrevious = 5f;
        
        [Tooltip("Reference to the prefab of the enemy to spawn.")]
        public GameObject enemyPrefab;
        
        [Tooltip("Number of this enemy type to spawn in this wave.")]
        public int count = 1;
        
        [Tooltip("Delay in seconds between spawning individual enemies in this wave.")]
        public float timeBetweenSpawns = 0.5f;
    }
}