using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameConfig gameConfig;
    
    public ActionType CurrentActionType => gameConfig ? gameConfig.actionTypeSelected : ActionType.None;
    public TurretType SelectedTurretType => gameConfig ? gameConfig.turretTypeSelected : TurretType.None;
    
    private bool _isGameOver = false;

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
        
        if (!gameConfig)return;
        gameConfig.Initialize();
    }

    public void StartWaves()
    {
        WavesManager.Instance.StartNextWave();
    }

    public void EndGame(bool isWon)
    {
        if (_isGameOver) return;

        _isGameOver = true;
        
        UIManager.Instance.SetEndGameUI(isWon);
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
        
        LoadLevel(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadLevel(int index)
    {
        if (index < 0 || index >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogError("Invalid scene index: " + index);
            return;
        }
        _isGameOver = false;
        SceneManager.LoadScene(index);
        UIManager.Instance.SetGameUI();
        gameConfig.Initialize();
    }
    
    public void LoadNextLevel()
    {
        _isGameOver = false;
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextIndex >= SceneManager.sceneCountInBuildSettings)
        {
            LoadMainMenu();
        }
        else
        {
            LoadLevel(nextIndex);
        }
    }
    
    public void LoadMainMenu()
    {
        _isGameOver = false;
        SceneManager.LoadScene(0);
        UIManager.Instance.SetMenuUI();
    }
    
    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

}

//Rango de torre, vida enemigos, efecto explosion ralentizacion, dinero, muerte
