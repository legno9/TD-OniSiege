using UnityEngine;


[CreateAssetMenu(fileName = "NewProjectileConfig", menuName = "Projectile/Projectile Config")]
public class ProjectileConfig : ScriptableObject
{
    [Header("Basic Settings")]
    public float projectileSpeed = 10f;
    
}
