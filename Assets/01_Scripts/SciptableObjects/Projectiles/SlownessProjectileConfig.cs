using UnityEngine;


[CreateAssetMenu(fileName = "NewAreaSlownessProjectileConfig", menuName = "Projectile/Area Slowness Config")]
public class SlownessProjectileConfig : AreaEffectProjectileConfig
{
    [Header("Slowness Settings")] 
    public float slownessFactor = 0.5f;
    public float slownessDuration = 0.5f;
    public float projectileDuration = 5f;
}
