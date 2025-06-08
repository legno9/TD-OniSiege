using UnityEngine;
using UnityEngine.EventSystems;

public class MouseManager : MonoBehaviour
{
    public static MouseManager Instance { get; private set; }
    
    [SerializeField] private GameObject highlightPrefab;
    [SerializeField] private MapManager _currentMapManager;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private TurretRangeVisualizer _turretRangeVisualizer;
    
    private GameObject _currentHighlight;
    private Vector3Int _previousCellPosition = Vector3Int.zero;
    
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
    
    private void Start()
    {
        if (!highlightPrefab) return;
        _currentHighlight = Instantiate(highlightPrefab, transform);
        _currentHighlight.SetActive(false);
    }

    private void Update()
    {
        if (!_mainCamera)
        {
            _mainCamera = Camera.main;
        }
        
        if (EventSystem.current.IsPointerOverGameObject())
        {
            if (_currentHighlight)
            {
                _currentHighlight.SetActive(false);
            }
            return;
        }
        
        if (!_currentMapManager || !_mainCamera) {return;}
        
        Vector3 mouseWorldPos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        
        Vector3Int? currentTilePosition = _currentMapManager.GetTileFromWorldPos(mouseWorldPos);

        if (!currentTilePosition.HasValue)
        {
            if (!_currentHighlight) return;
            _currentHighlight.SetActive(false);
            return;
        }

        UpdateHighlight(currentTilePosition.Value);

        if (Input.GetMouseButtonDown(0))
        {
            _currentMapManager.ProcessClickOnCell(currentTilePosition.Value);
        }
    }

    private void UpdateHighlight(Vector3Int tilePosition)
    {
        if (!_currentHighlight) return;
        if (tilePosition == _previousCellPosition) return;
        if (_currentMapManager.GetTileType(tilePosition) == TileType.Empty) return;
        
        _previousCellPosition = tilePosition;
        
         Vector3 tileCenter = _currentMapManager.GetTileWorldCenter(tilePosition);
         _currentHighlight.transform.position = tileCenter;
        _currentHighlight.SetActive(true);
        
        if (GameManager.Instance.SelectedTurretType != TurretType.None)
        {
            ShowTurretRange(GameManager.Instance.SelectedTurretType, tileCenter);
        }
        else
        {
            HideTurretRange();
        }
    }
    
    public void RegisterMapManager(MapManager mapManager)
    {
        if (!mapManager) return;

        _currentMapManager = mapManager;

        if (_currentHighlight)
        {
            _currentHighlight.SetActive(false);
        }
    }

    public void UnregisterMapManager(MapManager mapManager)
    {
        if (!mapManager || mapManager != _currentMapManager) return;
        _currentMapManager = null;
    }
    
    public void ShowTurretRange(TurretType turretType, Vector3 position)
    {
        if (_turretRangeVisualizer)
        {
            _turretRangeVisualizer.Show(turretType, position);
        }
    }
    
    public void HideTurretRange()
    {
        if (_turretRangeVisualizer)
        {
            _turretRangeVisualizer.Hide();
        }
    }
    
}