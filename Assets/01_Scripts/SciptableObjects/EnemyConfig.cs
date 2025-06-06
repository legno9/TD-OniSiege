using UnityEngine;
using UnityEngine.U2D.Animation;

[CreateAssetMenu(fileName = "NewEnemyConfig", menuName = "Game/Enemy Config")]
public class EnemyConfig : ScriptableObject
{
    [Header("Visuals")]
    public SpriteLibraryAsset SpriteLibraryAsset;

    [Header("Economy & Combat")]
    public int GoldValue = 10;
    public int PlayerDamage = 1;

    [Header("Movement")] 
    public float MaxSpeed = 5f;

    [Header("Health Setup")]
    public float MaxHealth = 100f;
    public bool CanBeHealed = true;
}