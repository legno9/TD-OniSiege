using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    
    [SerializeField] private GameObject prefab;
    [SerializeField] private int maxAmount;
    [SerializeField] private float spawnTimer = 1f;
    [SerializeField] private bool loopSpawn = true;

    private int currentAmount;
    private int totalAmount = 0;

    protected virtual void Start()
    {
        StartCoroutine(SpawnCaller());
    }

    IEnumerator SpawnCaller()
    {
        while (true)
        {
            if (currentAmount < maxAmount)
            {
                if (!loopSpawn && totalAmount >= maxAmount)
                {
                    break;
                }
                
                yield return new WaitForSeconds(spawnTimer);
                Spawn(prefab);
                currentAmount++;
                totalAmount++;
            }
            else
            {
                yield return new WaitUntil(() => currentAmount < maxAmount);
            }
        }
    }

    protected virtual void Spawn(GameObject prefab){}

    protected virtual void Despawn(Transform instance)
    {
        if (currentAmount > 0)
        {
            currentAmount--;
            SpawnPool.Instance.Despawn(instance);
        }
    }

}
