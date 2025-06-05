using UnityEngine;
using UnityEngine.Tilemaps;

public enum CellType
{
    Empty,       
    Grass,       
    Path,        
    Prop,        
    Tower
}

public class MapManager : MonoBehaviour
{
    
    [Header("Tilemap Layers")]
    [SerializeField] private Tilemap baseTilemap;
    [SerializeField] private Tilemap pathTilemap;
    [SerializeField] private Tilemap propsTilemap;
    [SerializeField] private int mapWidth = 32;
    [SerializeField] private int mapHeight = 18;
    
    [Header("Enemy Path")]
    [SerializeField] private GameObject enemyPathPoints;
    
    public Vector2[] EnemyPathPointsPos { get; private set; }
    
    private CellType[,] _mapGrid;
    private Turret[,] _towerGrid;
    
    private int _boundsXMin;
    private int _boundsYMin;

    private void Awake()
    {
        if (MouseManager.Instance)
        {
            MouseManager.Instance.RegisterMapManager(this);
        }
        InitializeMapData();
    }

    private void OnDestroy()
    {
        if (!MouseManager.Instance) return;
        MouseManager.Instance.UnregisterMapManager(this);
    }

    private void InitializeMapData()
    {
        if (!baseTilemap || !pathTilemap || !propsTilemap)
        {
            Debug.LogError("Tilemaps are not assigned in MapManager.");
            return;
        }
        
        BoundsInt bounds = baseTilemap.cellBounds;
        _boundsXMin = bounds.xMin;
        _boundsYMin = bounds.yMin;
        
        mapWidth = bounds.size.x;
        mapHeight = bounds.size.y;
        
        _mapGrid = new CellType[mapWidth, mapHeight];
        _towerGrid = new Turret[mapWidth, mapHeight];

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                Vector3Int cellPos = new Vector3Int(x + bounds.xMin, y + bounds.yMin, 0);

                if (pathTilemap.GetTile(cellPos))
                {
                    _mapGrid[x, y] = CellType.Path;
                }
                else if (propsTilemap.GetTile(cellPos))
                {
                    _mapGrid[x, y] = CellType.Prop;
                }
                else if (baseTilemap.GetTile(cellPos))
                {
                    _mapGrid[x, y] = CellType.Grass;
                }
                else
                {
                    _mapGrid[x, y] = CellType.Empty;
                }
            }
        }
        
        // Initialize enemy path points
        if (!enemyPathPoints)
        {
            Debug.Log("No path points found");
            return;
        }
        
        int enemyPathPointsCount = enemyPathPoints.transform.childCount;
        EnemyPathPointsPos = new Vector2[enemyPathPointsCount];
        
        for ( int i = 0; i < enemyPathPointsCount; i++)
        {
            Transform point = enemyPathPoints.transform.GetChild(i);
            Vector3 pointPos = point.position;
            EnemyPathPointsPos[i] = new Vector2(pointPos.x, pointPos.y);
        }
    }

    private bool IsInBounds(Vector3Int position)
    {
        int x = position.x - _boundsXMin;
        int y = position.y - _boundsYMin;

        return x >= 0 && x < mapWidth && y >= 0 && y < mapHeight;
    }
    
    public Vector3 GetTileWorldCenter(Vector3Int position)
    {
        return IsInBounds(position) ? baseTilemap.GetCellCenterWorld(position): Vector3.zero ;
    }
    
    public Vector3Int GetTileFromWorldPos(Vector3 position)
    {
        Vector3Int cellPos = baseTilemap.WorldToCell(position);
        return IsInBounds(cellPos) ? cellPos : Vector3Int.zero;
    }

    public CellType GetTileType(Vector3Int position)
    {
        int x = position.x - _boundsXMin;
        int y = position.y - _boundsYMin;
        return IsInBounds(position) ? _mapGrid[x, y] : CellType.Empty;
    }
    
    public void ProcessClickOnCell(Vector3Int cellPos)
    {
        if (!IsInBounds(cellPos))
        {
            Debug.LogWarning($"Clicked out of bounds at {cellPos}. Ignoring click.");
            return;
        }
        
        CellType type = GetTileType(cellPos);
        
        switch (type)
        {
            case CellType.Grass when IsBuildable(cellPos):
                Debug.Log($"Clicked on buildable grass at {cellPos}. Ready to place a tower.");
                break;
            case CellType.Tower:
            {
                Debug.Log($"Clicked on existing tower at {cellPos}. Selecting it.");
                Turret selectedTower = GetTowerAt(cellPos);
                if (selectedTower)
                {
                    // GameManager.Instance.SelectTowerForInteraction(selectedTower);
                }
        
                break;
            }
            case CellType.Empty:
            case CellType.Path:
            case CellType.Prop:
            default:
                Debug.Log($"Clicked on non-buildable area ({type}) at {cellPos}.");
                break;
        }
    }
    
    private bool IsBuildable(Vector3Int cellPos)
    {
        CellType type = GetTileType(cellPos);
        int x = cellPos.x - _boundsXMin;
        int y = cellPos.y - _boundsYMin;
        if (IsInBounds(cellPos)) { return type == CellType.Grass && _towerGrid[x, y] == null; }
        return false;
    }
    
    public void PlaceTower(Vector3Int cellPos, Turret turret)
    {
        int x = cellPos.x - _boundsXMin;
        int y = cellPos.y - _boundsYMin;
        if (IsInBounds(cellPos))
        {
            if (_mapGrid[x, y] == CellType.Grass && _towerGrid[x, y] == null)
            {
                _mapGrid[x, y] = CellType.Tower;
                _towerGrid[x, y] = turret;
                //GameManager.Instance.PlaceTurret;
                Debug.Log($"Tower placed at {cellPos}");
            }
            else
            {
                Debug.LogWarning($"Cannot place tower at {cellPos}. Cell type: {_mapGrid[x,y]}, Tower present: {_towerGrid[x,y] != null}");
            }
        }
        else
        {
            Debug.LogWarning($"Attempted to place tower out of map bounds at {cellPos}");
        }
    }
    
    public void RemoveTower(Vector3Int cellPos)
    {
        int x = cellPos.x - _boundsXMin;
        int y = cellPos.y - _boundsYMin;
        if (IsInBounds(cellPos))
        {
            if (_mapGrid[x, y] == CellType.Tower)
            {
                _mapGrid[x, y] = CellType.Grass;
                _towerGrid[x, y] = null;
                //GameManager.Instance.RemoveTurret;
                Debug.Log($"Tower removed from {cellPos}");
            }
            else
            {
                Debug.LogWarning($"No tower found at {cellPos} to remove, or cell type is not Tower.");
            }
        }
        else
        {
            Debug.LogWarning($"Attempted to remove tower out of map bounds at {cellPos}");
        }
    }
    
    private Turret GetTowerAt(Vector3Int cellPos)
    {
        int x = cellPos.x - _boundsXMin;
        int y = cellPos.y - _boundsYMin;
        return IsInBounds(cellPos) ? _towerGrid[x, y] : null;
    }
}
