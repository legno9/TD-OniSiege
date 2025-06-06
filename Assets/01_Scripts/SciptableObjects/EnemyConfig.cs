using UnityEngine;
using UnityEngine.U2D.Animation;

[CreateAssetMenu(fileName = "NewEnemyConfig", menuName = "Game/Enemy Config")]
public class EnemyConfig : ScriptableObject
{
    [Header("Visuals")]
    public SpriteLibraryAsset SpriteLibraryAsset;

    [Header("Economy & Combat")]
    public readonly int GoldValue = 10;
    public readonly int PlayerDamage = 1;

    [Header("Movement")] 
    public readonly float MaxSpeed = 5f;

    [Header("Health Setup")]
    public readonly float MaxHealth = 100f;
    public readonly bool CanBeHealed = true;
}