using UnityEngine;
using UnityEngine.EventSystems;

public class MouseManager : MonoBehaviour
{
    public static MouseManager Instance { get; private set; }
    
    [SerializeField] private GameObject highlightPrefab;
    [SerializeField] private MapManager _currentMapManager;
    [SerializeField] private Camera _mainCamera;
    
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
        _currentHighlight = Instantiate(highlightPrefab);
        _currentHighlight.SetActive(false);

        if (!_mainCamera)
        {
            _mainCamera = Camera.main;
        }
    }

    private void Update()
    {
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
        
        Vector3Int? currentCellPosition = _currentMapManager.GetTileFromWorldPos(mouseWorldPos);

        if (!currentCellPosition.HasValue)
        {
            if (!_currentHighlight) return;
            _currentHighlight.SetActive(false);
            return;
        }

        UpdateHighlight(currentCellPosition.Value);

        if (Input.GetMouseButtonDown(0))
        {
            _currentMapManager.ProcessClickOnCell(currentCellPosition.Value);
        }
    }

    private void UpdateHighlight(Vector3Int cellPosition)
    {
        if (!_currentHighlight) return;
        if (cellPosition == _previousCellPosition) return;
        if (_currentMapManager.GetTileType(cellPosition) == TileType.Empty) return;
        
        _previousCellPosition = cellPosition;
        _currentHighlight.transform.position = _currentMapManager.GetTileWorldCenter(cellPosition);
        _currentHighlight.SetActive(true);
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
}