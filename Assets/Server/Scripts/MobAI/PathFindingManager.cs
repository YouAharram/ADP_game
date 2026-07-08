using UnityEngine;
using System.Collections.Generic;
using System;

public class PathfindingManager
{
    private static PathfindingManager instance;
    public static PathfindingManager Instance => instance ??= new PathfindingManager();

    private PathfindingManager() { }
    
    private MapGraph graph;
    private float cellSize;
    
    public Queue<Vector2> GetPath(Vector2 startWorld, Vector2 targetWorld)
    {
        
        if (graph == null && MapObstacleGenerator.Instance != null)
        {
            graph = new MapGraph(MapObstacleGenerator.Instance.WalkableMatrix);
            cellSize = MapObstacleGenerator.Instance.CellSize;
        }

        if (graph == null) return null;
        
        Vector2Int startGrid = WorldToGrid(startWorld);
        Vector2Int targetGrid = WorldToGrid(targetWorld);

        
        // Funzioni per l'lagoritmo A*, che comprende anche direzioni oblique. 
        Func<Vector2Int, Vector2Int, double> distance = (n1, n2) =>
        {
            int dx = Mathf.Abs(n2.x - n1.x);
            int dy = Mathf.Abs(n2.y - n1.y);
            return (dx != 0 && dy != 0) ? 1.41421356 : 1.0; // sqrt(2) per la diagonale
        };

        Func<Vector2Int, double> estimate = (n) =>
        {
            int dx = Mathf.Abs(n.x - targetGrid.x);
            int dy = Mathf.Abs(n.y - targetGrid.y);
            int minD = Mathf.Min(dx, dy);
            int maxD = Mathf.Max(dx, dy);
            return maxD + (1.41421356 - 1.0) * minD; 
        };
        
        // Si ottiene il percorso dall'algoritmo
        var path = AStar.FindPath(graph, startGrid, targetGrid, distance, estimate);
        
        if (path == null) return null;

        // Convertiamo il Path generico di A* in una coda per il mob
        List<Vector2> worldPathList = new List<Vector2>();
        foreach (var node in path)
        {
            worldPathList.Add(GridToWorld(node));
        }

        // A* restituisce il path dal target all'origine, lo si inverte.
        worldPathList.Reverse(); 
        
        // RIMOUVE LO SFARFALLIO
        if (worldPathList.Count > 0)
            worldPathList.RemoveAt(0); // il mob è già lì, non serve come waypoint
        
        return new Queue<Vector2>(worldPathList);
    }

    public Vector2Int WorldToGrid(Vector2 worldPosition)
        => MapObstacleGenerator.Instance.WorldToGridCell(worldPosition);

    public Vector2 GridToWorld(Vector2Int gridNode)
        => MapObstacleGenerator.Instance.GridCellToWorld(gridNode);
}