using UnityEngine;
using UnityEngine.U2D.Animation;

[CreateAssetMenu(fileName = "NewEnemyInvisibleConfig", menuName = "Game/Enemy Invisible Config")]
public class EnemyInvisibleConfig : EnemyConfig
{
    [Header("Revealed Settings")]
    public SpriteLibraryAsset revealedSpriteLibraryAsset;
}
