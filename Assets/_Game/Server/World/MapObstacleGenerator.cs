using UnityEngine;
using UnityEngine.Tilemaps;

public class MapObstacleGenerator : MonoBehaviour
{
    public static MapObstacleGenerator Instance { get; private set; }

    [Header("Configurazione Mappa")]
    [Tooltip("La Tilemap che rappresenta il pavimento su cui calcolare l'area di gioco")]
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private float cellSize = 0.1f;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private int paddingCells = 1;

    private int width;
    private int height;
    
    private Vector2Int originOffset; 

    private bool[,] walkableMatrix;
    public bool[,] WalkableMatrix => walkableMatrix;
    
    public int Width => width;
    public int Height => height;
    public float CellSize => cellSize;
    public Vector2Int OriginOffset => originOffset;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {
        GenerateObstacleMatrix();
    }

    private void GenerateObstacleMatrix()
    {
        if (groundTilemap == null)
        {
            Debug.LogError("ATTENZIONE: Nessuna Tilemap assegnata a MapObstacleGenerator!");
            return;
        }

        // cellBounds restituisce un rettangolo che racchiude tutte le tile dipinte
        groundTilemap.CompressBounds(); 
        BoundsInt bounds = groundTilemap.cellBounds;

        width = bounds.size.x;
        height = bounds.size.y;
        
        originOffset = (Vector2Int)bounds.position;

        Debug.Log($"Mappa rilevata. Origine: {originOffset}, Dimensione: {width}x{height}");

        walkableMatrix = new bool[width, height];

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                walkableMatrix[x, y] = true;

        Vector2 mapCenter = groundTilemap.localBounds.center + groundTilemap.transform.position;
        Vector2 mapSize = groundTilemap.localBounds.size;

        Collider2D[] obstacles = Physics2D.OverlapBoxAll(mapCenter, mapSize, 0, obstacleLayer);

        foreach (var obs in obstacles)
        {
            Bounds obsBounds = obs.bounds;
            
            Vector3Int minCell = groundTilemap.WorldToCell(obsBounds.min);
            Vector3Int maxCell = groundTilemap.WorldToCell(obsBounds.max);

            int minX = minCell.x - originOffset.x;
            int maxX = maxCell.x - originOffset.x;
            int minY = minCell.y - originOffset.y;
            int maxY = maxCell.y - originOffset.y;

            minX -= paddingCells;
            maxX += paddingCells;
            minY -= paddingCells;
            maxY += paddingCells;

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    if (x >= 0 && x < width && y >= 0 && y < height)
                    {
                        walkableMatrix[x, y] = false;
                    }
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying && walkableMatrix == null)
            GenerateObstacleMatrix();

        if (walkableMatrix == null) return;
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Gizmos.color = walkableMatrix[x, y] ? new Color(0, 1, 0, 0.4f) : new Color(1, 0, 0, 0.6f);
                
                Vector3Int cellPos = new Vector3Int(x + originOffset.x, y + originOffset.y, 0);
                Vector3 worldPos = groundTilemap.GetCellCenterWorld(cellPos);
                
                Gizmos.DrawCube(worldPos, groundTilemap.layoutGrid.cellSize * 0.8f);
            }
        }
    }
    
    public Vector2Int WorldToGridCell(Vector2 worldPosition)
    {
        Vector3Int cell = groundTilemap.WorldToCell(worldPosition);
        return new Vector2Int(cell.x - originOffset.x, cell.y - originOffset.y);
    }

    public Vector2 GridCellToWorld(Vector2Int gridCell)
    {
        Vector3Int cell = new Vector3Int(gridCell.x + originOffset.x, gridCell.y + originOffset.y, 0);
        return groundTilemap.GetCellCenterWorld(cell);
    }
}