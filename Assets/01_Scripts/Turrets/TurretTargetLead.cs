using System.Linq;

public class TurretTargetLead : TurretTargeting
{
    public override Enemy FindTarget(float range)
    {
       Enemy enemyLeader = WavesManager.Instance.SpawnedEnemies
            .Where(e => e && e.IsTargetable() && 
                        (e.transform.position - transform.position).sqrMagnitude <= range * range)
            .OrderByDescending(e => e.GetPathProgress())
            .FirstOrDefault();

        return enemyLeader;
    }
}
