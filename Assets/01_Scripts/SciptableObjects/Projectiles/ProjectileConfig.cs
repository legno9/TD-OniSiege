using UnityEngine;


[CreateAssetMenu(fileName = "NewProjectileConfig", menuName = "Game/Projectile Config")]
public class ProjectileConfig : ScriptableObject
{
    [Header("Basic Settings")]
    public float projectileSpeed = 10f;
    
}
