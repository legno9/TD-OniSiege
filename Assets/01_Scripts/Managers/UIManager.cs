using TMPro;
using UnityEngine;
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI healthText;
    
    private int _lastGoldAmount = -1;
    private int _lastHealthAmount = -1;
        

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (!gameConfig) return;
        
        if (gameConfig.currentGold != _lastGoldAmount)
        {
            SetGoldText(gameConfig.currentGold);
            _lastGoldAmount = gameConfig.currentGold;
        }
        if (gameConfig.currentHealth != _lastHealthAmount)
        {
            SetHealthText(gameConfig.currentHealth);
            _lastHealthAmount = gameConfig.currentHealth;
        }
    }
    
    public void SetHealthText(int health)
    {
        if (healthText)
        {
            healthText.text = "Health: " + health;
        }
    }

    public void SetGoldText(int gold)
    {
        if (goldText)
        {
            goldText.text = "Gold: " + gold;
        }
    }
    
    public void SetUpgradeAction(bool isOn)
    {
        if (gameConfig)
        {
            gameConfig.SetActionType(isOn ? ActionType.UpgradeTurret : ActionType.None);
        }
    }
    public void SetSellAction(bool isOn)
    {
        if (gameConfig)
        {
            gameConfig.SetActionType(isOn ? ActionType.SellTurret : ActionType.None);
        }
    }
    public void SetShooterTurretType(bool isOn)
    {
        if (gameConfig)
        {
            gameConfig.SetActionType(isOn ? ActionType.PlaceTurret : ActionType.None);
            gameConfig.SetTurretType(isOn ? TurretType.Shooter : TurretType.None);
        }
    }
    public void SetAreaTurretType(bool isOn)
    {
        if (gameConfig)
        {
            gameConfig.SetActionType(isOn ? ActionType.PlaceTurret : ActionType.None);
            gameConfig.SetTurretType(isOn ? TurretType.AreaDamage : TurretType.None);
        }
    }
    
    public void SetSlowTurretType(bool isOn)
    {
        if (gameConfig)
        {
            gameConfig.SetActionType(isOn ? ActionType.PlaceTurret : ActionType.None);
            gameConfig.SetTurretType(isOn ? TurretType.Slowness : TurretType.None);
        }
    }
}
