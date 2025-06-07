using UnityEngine;

public abstract class TurretTargeting : MonoBehaviour
{
    public abstract Enemy FindTarget(float range);
}
