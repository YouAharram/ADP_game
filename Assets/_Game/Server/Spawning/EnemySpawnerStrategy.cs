using UnityEngine;

public interface EnemySpawnerStrategy
{
    static Vector2 FullRandomPosition()
    {
        bool [,] WalkableMatrix = MapObstacleGenerator.Instance.WalkableMatrix;
        int xPosition = 0, yPosition = 0;
        do
        {
            xPosition = Random.Range(0, MapObstacleGenerator.Instance.Width);
            yPosition = Random.Range(0, MapObstacleGenerator.Instance.Height); 
        } while (!WalkableMatrix[xPosition, yPosition]);

        return MapObstacleGenerator.Instance.GridCellToWorld(new Vector2Int(xPosition, yPosition));
    }

    static Vector2 EastRandomPosition()
    {
        bool [,] WalkableMatrix = MapObstacleGenerator.Instance.WalkableMatrix;
        int xPosition = 0, yPosition = 0;
        do
        {
            xPosition = Random.Range(MapObstacleGenerator.Instance.Width/2, MapObstacleGenerator.Instance.Width);
            yPosition = Random.Range(0, MapObstacleGenerator.Instance.Height); 
        } while (!WalkableMatrix[xPosition, yPosition]);

        return MapObstacleGenerator.Instance.GridCellToWorld(new Vector2Int(xPosition, yPosition));
    }
}