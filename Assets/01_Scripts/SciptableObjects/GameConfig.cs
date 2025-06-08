using UnityEngine;

[CreateAssetMenu(fileName = "NewGameConfig", menuName = "Core/ Game Config", order = 0)]
public class GameConfig : ScriptableObject
{
    [SerializeField] private int initialGold = 250;
    [SerializeField] private int initialHealth = 10;

    [HideInInspector] public int currentGold;
    [HideInInspector] public int currentHealth;
    
    [HideInInspector] public TurretType turretTypeSelected = TurretType.None;
    [HideInInspector] public ActionType actionTypeSelected = ActionType.None;
    
    public void Initialize()
    {
        currentGold = initialGold;
        currentHealth = initialHealth;
        turretTypeSelected = TurretType.None;
        actionTypeSelected = ActionType.None;
    }
    
    public void SetActionType(ActionType actionType)
    {
        actionTypeSelected = actionType;
    }
    public void SetTurretType(TurretType turretType)
    {
        turretTypeSelected = turretType;
    }
}
