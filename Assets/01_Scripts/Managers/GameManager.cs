using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private WavesManager wavesManager;
    
    public ActionType CurrentActionType => gameConfig ? gameConfig.actionTypeSelected : ActionType.None;
    public TurretType SelectedTurretType => gameConfig ? gameConfig.turretTypeSelected : TurretType.None;
    
    
    private bool _isGameOver = false;
    private bool _isGameWon = false;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        InitializeGame();
    }

    public void InitializeGame()
    {
        
        _isGameOver = false;
        _isGameWon = false;
        
        if (!gameConfig)return;
        gameConfig.Initialize();
    }

    private void Start()
    {
        if (!wavesManager) return;
        wavesManager.StartNextWave();
    }

    public void EndGame(bool isWon)
    {
        if (_isGameOver) return;

        _isGameOver = true;
        _isGameWon = isWon;
        
        // UIManager.Instance.ShowGameOverScreen(isWon);
    }
    
    public void AddGold(int amount)
    {
        if (_isGameOver || !gameConfig) return;

        gameConfig.currentGold += amount;
    }

    public bool TrySpendGold(int amount)
    {
        if (_isGameOver || !gameConfig) return false;

        if (gameConfig.currentGold < amount) return false;
        
        gameConfig.currentGold -= amount;
        return true;
    }

    public void TakeDamage(int amount)
    {
        if (_isGameOver || !gameConfig) return;
        
        gameConfig.currentHealth -= amount;
        gameConfig.currentHealth = Mathf.Max(0, gameConfig.currentHealth);

        if (gameConfig.currentHealth <= 0)
        {
            EndGame(false);
        }
    }
    
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadNextLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

}
