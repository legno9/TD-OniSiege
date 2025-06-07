using UnityEngine;
using UnityEngine.Tilemaps;

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
    
    [Header("Turrets")]
    [SerializeField] private TurretTypesConfig turretTypesConfig;
    public Vector2[] EnemyPathPointsPos { get; private set; }
    
    private TileType[,] _mapGrid;
    private Transform[,] _turretGrid;
    
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
        
        _mapGrid = new TileType[mapWidth, mapHeight];
        _turretGrid = new Transform[mapWidth, mapHeight];

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                Vector3Int cellPos = new Vector3Int(x + bounds.xMin, y + bounds.yMin, 0);

                if (pathTilemap.GetTile(cellPos))
                {
                    _mapGrid[x, y] = TileType.Path;
                }
                else if (propsTilemap.GetTile(cellPos))
                {
                    _mapGrid[x, y] = TileType.Prop;
                }
                else if (baseTilemap.GetTile(cellPos))
                {
                    _mapGrid[x, y] = TileType.Grass;
                }
                else
                {
                    _mapGrid[x, y] = TileType.Empty;
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
    
    public Vector3Int? GetTileFromWorldPos(Vector3 position)
    {
        Vector3Int cellPos = baseTilemap.WorldToCell(position);
        return IsInBounds(cellPos) ? cellPos : null;
    }

    public TileType GetTileType(Vector3Int position)
    {
        int x = position.x - _boundsXMin;
        int y = position.y - _boundsYMin;
        return IsInBounds(position) ? _mapGrid[x, y] : TileType.Empty;
    }
    
    public void ProcessClickOnCell(Vector3Int tilePos)
    {
        if (!IsInBounds(tilePos))
        {
            Debug.LogWarning($"Clicked out of bounds at {tilePos}. Ignoring click.");
            return;
        }
        
        TileType type = GetTileType(tilePos);
        
        switch (type)
        {
            case TileType.Grass when IsBuildable(tilePos):
                PlaceTower(tilePos, TurretType.Slowness);
                break;
            case TileType.Turret:
            {
                Transform selectedTower = GetTowerAt(tilePos);
                if (selectedTower)
                {
                    // GameManager.Instance.SelectTowerForInteraction(selectedTower);
                    RemoveTower(tilePos);
                }
        
                break;
            }
            case TileType.Empty:
            case TileType.Path:
            case TileType.Prop:
            default:
                Debug.Log($"Clicked on non-buildable area ({type}) at {tilePos}.");
                break;
        }
    }
    
    private bool IsBuildable(Vector3Int cellPos)
    {
        TileType type = GetTileType(cellPos);
        int x = cellPos.x - _boundsXMin;
        int y = cellPos.y - _boundsYMin;
        if (IsInBounds(cellPos)) { return type == TileType.Grass && !_turretGrid[x, y]; }
        return false;
    }

    private void PlaceTower(Vector3Int cellPos, TurretType type)
    {
        int x = cellPos.x - _boundsXMin;
        int y = cellPos.y - _boundsYMin;
        if (IsInBounds(cellPos))
        {
            if (_mapGrid[x, y] == TileType.Grass && !_turretGrid[x, y])
            {
                _mapGrid[x, y] = TileType.Turret;
                GameObject turretPrefab = turretTypesConfig.TurretPrefabs[type];
                
                _turretGrid[x, y] = SpawnPool.Instance.Spawn(turretPrefab.transform, baseTilemap.GetCellCenterWorld(cellPos), 
                    Quaternion.identity, turretPrefab.transform.localScale, transform);
            }
            else
            {
                Debug.LogWarning($"Cannot place tower at {cellPos}. Cell type: {_mapGrid[x,y]}, Tower present: {_turretGrid[x,y]}");
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
            if (_mapGrid[x, y] == TileType.Turret)
            {
                SpawnPool.Instance.Despawn(_turretGrid[x, y]);
                _mapGrid[x, y] = TileType.Grass;
                _turretGrid[x, y] = null;
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
    
    private Transform GetTowerAt(Vector3Int cellPos)
    {
        int x = cellPos.x - _boundsXMin;
        int y = cellPos.y - _boundsYMin;
        return IsInBounds(cellPos) ? _turretGrid[x, y] : null;
    }
}
