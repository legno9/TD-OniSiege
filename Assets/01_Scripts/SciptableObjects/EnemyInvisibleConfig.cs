using UnityEngine;
using UnityEngine.U2D.Animation;

[CreateAssetMenu(fileName = "NewEnemyInvisibleConfig", menuName = "Enemy/Enemy Invisible Config")]
public class EnemyInvisibleConfig : EnemyConfig
{
    [Header("Revealed Settings")]
    public SpriteLibraryAsset revealedSpriteLibraryAsset;
}
