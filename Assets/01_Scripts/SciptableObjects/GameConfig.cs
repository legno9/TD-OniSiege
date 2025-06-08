using UnityEngine;

[CreateAssetMenu(fileName = "NewGameConfig", menuName = "Core/ Game Config", order = 0)]
public class GameConfig : ScriptableObject
{
    [SerializeField] private int initialGold = 250;
    [SerializeField] private int initialHealth = 10;

    [HideInInspector] public int currentGold;
    [HideInInspector] public int currentHealth;
    
    public void Initialize()
    {
        currentGold = initialGold;
        currentHealth = initialHealth;
    }

}
