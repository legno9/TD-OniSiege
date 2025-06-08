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
        Vector3Int tilePos = baseTilemap.WorldToCell(position);
        return IsInBounds(tilePos) ? tilePos : null;
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

        TileType tileType = GetTileType(tilePos);
        ActionType currentAction = GameManager.Instance.CurrentActionType;
        TurretType selectedTurret = GameManager.Instance.SelectedTurretType;

        switch (currentAction)
        {
            case ActionType.PlaceTurret:
                HandlePlaceTurret(tilePos, tileType, selectedTurret);
                break;

            case ActionType.UpgradeTurret:
                HandleUpgradeTurret(tilePos, tileType);
                break;

            case ActionType.SellTurret:
                HandleSellTurret(tilePos, tileType);
                break;

            case ActionType.None:
            default:
                break;
        }
    }
    
    private void HandlePlaceTurret(Vector3Int tilePos, TileType tileType, TurretType selectedTurret)
    {
        if (tileType != TileType.Grass || !IsBuildable(tilePos) || selectedTurret == TurretType.None) return;
        if (!GameManager.Instance.TrySpendGold(turretTypesConfig.TurretCosts[selectedTurret])) return;
        PlaceTower(tilePos, selectedTurret);
    }
    
    private void HandleUpgradeTurret(Vector3Int tilePos, TileType tileType)
    {
        Transform turret = GetTowerAt(tilePos);

        if (tileType != TileType.Turret || !turret) return;
        
        Turret turretComponent = turret.GetComponent<Turret>();
        if (!turretComponent) return;
        if (!GameManager.Instance.TrySpendGold(turretComponent._currentUpgradeCost)) return;
        if (!turretComponent.Upgrade())
        {
            GameManager.Instance.AddGold(turretComponent._currentUpgradeCost);
        }
        
    }
    
    private void HandleSellTurret(Vector3Int tilePos, TileType tileType)
    {
        Transform turret = GetTowerAt(tilePos);
        if (tileType == TileType.Turret && turret)
        {
            Turret turretComponent = turret.GetComponent<Turret>();
            if (!turretComponent) return;
            GameManager.Instance.AddGold(turretComponent._currentSellValue);
            RemoveTower(tilePos);
        }
    }
    
    private bool IsBuildable(Vector3Int tilePos)
    {
        TileType type = GetTileType(tilePos);
        int x = tilePos.x - _boundsXMin;
        int y = tilePos.y - _boundsYMin;
        if (IsInBounds(tilePos)) { return type == TileType.Grass && !_turretGrid[x, y]; }
        return false;
    }

    private void PlaceTower(Vector3Int tilePos, TurretType type)
    {
        int x = tilePos.x - _boundsXMin;
        int y = tilePos.y - _boundsYMin;
        if (!IsInBounds(tilePos)) return;
        if (_mapGrid[x, y] != TileType.Grass || _turretGrid[x, y]) return;
        
        _mapGrid[x, y] = TileType.Turret;
        GameObject turretPrefab = turretTypesConfig.TurretPrefabs[type];
                
        _turretGrid[x, y] = SpawnPool.Instance.Spawn(turretPrefab.transform, baseTilemap.GetCellCenterWorld(tilePos), 
            Quaternion.identity, turretPrefab.transform.localScale, transform);
    }
    
    private void RemoveTower(Vector3Int tilePos)
    {
        int x = tilePos.x - _boundsXMin;
        int y = tilePos.y - _boundsYMin;
        
        if (!IsInBounds(tilePos)) return;
        if (_mapGrid[x, y] != TileType.Turret) return;
        
        SpawnPool.Instance.Despawn(_turretGrid[x, y]);
        _mapGrid[x, y] = TileType.Grass;
        _turretGrid[x, y] = null;
    }
    
    private Transform GetTowerAt(Vector3Int tilePos)
    {
        int x = tilePos.x - _boundsXMin;
        int y = tilePos.y - _boundsYMin;
        return IsInBounds(tilePos) ? _turretGrid[x, y] : null;
    }
    
}
