using UnityEngine;
using System.Collections.Generic;
using UnityEngine.U2D.Animation;

[CreateAssetMenu(fileName = "NewTurretConfig", menuName = "Turret/Turret Config")]
public class TurretConfig : ScriptableObject
{
    [System.Serializable]
    public class TurretLevelData
    {
        [Header("Stats")]
        public float damage = 15f;
        public float actionRange = 100f;
        public float actionRate = 3f;  
        public int upgradeCost = 100;   
        public int sellValue = 100; 

        [Header("Visuals")]
        [Tooltip("Sprite Library for level.")]
        public SpriteLibraryAsset spriteLibraryAsset;
    }
    
    [Header("Base Turret Settings")]
    public int initialCost = 100;
    public int initialSellValue = 50;
    public int maxLevel = 3;
    
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    
    [Header("Level Data")]
    [Tooltip("List of data for each level.")]
    public List<TurretLevelData> levelsData;

    public TurretLevelData GetLevelData(int level)
    {
        if (level > 0 && level <= levelsData.Count)
        {
            return levelsData[level - 1];
        }
        return null;
    }
}
