using UnityEngine;
using UnityEngine.U2D.Animation;

[CreateAssetMenu(fileName = "NewEnemyConfig", menuName = "Enemy/Enemy Config")]
public class EnemyConfig : ScriptableObject
{
    [Header("Visuals")]
    public SpriteLibraryAsset spriteLibraryAsset;

    [Header("Economy & Combat")]
    public int goldValue = 10;
    public int playerDamage = 1;

    [Header("Movement")] 
    public float maxSpeed = 5f;

    [Header("Health Setup")]
    public float maxHealth = 100f;
    public bool canBeHealed = true;
}