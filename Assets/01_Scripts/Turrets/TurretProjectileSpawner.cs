using UnityEngine;

public class TurretProjectileSpawner : MonoBehaviour
{
    public void SpawnProjectile(GameObject projectilePrefab, Enemy target, float damage)
    {
        if (!projectilePrefab|| !target)
        {
            Debug.LogError("TurretProjectileSpawner: Invalid projectilePrefab or target!", this);
            return;
        }
        
        GameObject projectileGo = SpawnPool.Instance.Spawn(projectilePrefab.transform, transform.position, Quaternion.identity).gameObject;
        Projectile projectile = projectileGo.GetComponent<Projectile>();

        if (projectile)
        {
            projectile.Initialize(target, damage);
        }
        else
        {
            Debug.LogError($"TurretProjectileSpawner: Projectile prefab {projectileGo.name} does not have a Projectile component!", this);
            SpawnPool.Instance.Despawn(projectileGo.transform);
        }
    }
}
