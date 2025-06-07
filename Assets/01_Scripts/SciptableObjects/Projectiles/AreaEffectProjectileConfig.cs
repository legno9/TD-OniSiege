using UnityEngine;


[CreateAssetMenu(fileName = "NewAreaProjectileConfig", menuName = "Game/Area Projectile Config")]
public class AreaEffectProjectileConfig : ProjectileConfig
{
    [Header("Area Settings")]
    public float areaEffectRadius = 5f;
}
