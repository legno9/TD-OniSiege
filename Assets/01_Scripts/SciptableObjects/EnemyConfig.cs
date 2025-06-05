using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyConfig", menuName = "Game/Enemy Config")]
public class EnemyConfig : ScriptableObject
{
    public float maxHealth = 100f;
    public float maxSpeed = 5f;
    public int goldValue = 10;
    public int playerDamage = 1;
}