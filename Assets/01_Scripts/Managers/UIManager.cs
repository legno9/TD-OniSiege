using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private CanvasGroup gameUI;
    [SerializeField] private CanvasGroup menuUI;
    [SerializeField] private CanvasGroup pauseUI;
    [SerializeField] private CanvasGroup endGameUI;
    [SerializeField] private TextMeshProUGUI endGameText;
    [SerializeField] private TextMeshProUGUI enGameButtonText;
    [SerializeField] private ToggleGroup toggleGroup;
    [SerializeField] private Button waveButton;
    [SerializeField] private Button endGameButton;
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

    public void SetGameUI()
    {
        if (!gameUI) return;
        if (!menuUI) return;
        
        gameUI.alpha = 1f;
        gameUI.interactable = true;
        gameUI.blocksRaycasts = true;
        
        menuUI.alpha = 0f;
        menuUI.interactable = false;
        menuUI.blocksRaycasts = false;

        foreach (Toggle toggle in toggleGroup.ActiveToggles())
        {
            toggle.isOn = false;
        }
        
        if (waveButton)
        {
            waveButton.interactable = true;
        }
    }

    public void SetMenuUI()
    {
        if (!gameUI) return;
        if (!menuUI) return;
        
        gameUI.alpha = 0f;
        gameUI.interactable = false;
        gameUI.blocksRaycasts = false;
        
        menuUI.alpha = 1f;
        menuUI.interactable = true;
        menuUI.blocksRaycasts = true;

        endGameUI.alpha = 0f;
        endGameUI.interactable = false;
        endGameUI.blocksRaycasts = false;
    }

    public void SetPauseUI(bool isOn)
    {
        if (pauseUI)
        {
            Time.timeScale = isOn ? 0f : 1f;
            pauseUI.alpha = isOn ? 1f : 0f;
            pauseUI.interactable = isOn;
            pauseUI.blocksRaycasts = isOn;
        }
    }
    
    public void SetEndGameUI(bool isWon)
    {
        if (!endGameUI) return;
        
        endGameUI.alpha = 1f;
        endGameUI.interactable = true;
        endGameUI.blocksRaycasts = true;
        
        if (enGameButtonText)
        {
            enGameButtonText.text = isWon ? "Next" : "Restart";
        }
        
        if (endGameButton)
        {
            endGameButton.onClick.AddListener(() =>
            {
                if (isWon)
                {
                    GameManager.Instance.LoadNextLevel();
                    
                }
                else
                {
                    GameManager.Instance.RestartLevel();
                }
                
                Time.timeScale = 1f;
                endGameUI.alpha = 0f;
                endGameUI.interactable = false;
                endGameUI.blocksRaycasts = false;
            });
        }
        
        if (endGameText)
        {
            endGameText.text = isWon ? "You Win!" : "Game Over!";
        }
        
        Time.timeScale = 0f;
    }
}
