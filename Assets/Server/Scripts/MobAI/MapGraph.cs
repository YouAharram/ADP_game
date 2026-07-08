using System.Collections.Generic;
using UnityEngine;

public class MapGraph : IHasNeighbours<Vector2Int>
{
    private bool[,] grid;
    private int width;
    private int height;

    private static readonly Vector2Int[] straightDirs =
    {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
    };

    private static readonly Vector2Int[] diagonalDirs =
    {
        new Vector2Int(1, 1), new Vector2Int(1, -1),
        new Vector2Int(-1, 1), new Vector2Int(-1, -1)
    };

    public MapGraph(bool[,] grid)
    {
        this.grid = grid;
        width = grid.GetLength(0);
        height = grid.GetLength(1);
    }

    public IEnumerable<Vector2Int> Neighbours(Vector2Int node)
    {
        foreach (var dir in straightDirs)
        {
            Vector2Int next = node + dir;
            if (IsWalkable(next))
                yield return next;
        }

        foreach (var dir in diagonalDirs)
        {
            Vector2Int next = node + dir;
            if (!IsWalkable(next)) continue;

            // entrambe le celle ortogonali
            // adiacenti alla diagonale devono essere libere
            Vector2Int horizontal = node + new Vector2Int(dir.x, 0);
            Vector2Int vertical = node + new Vector2Int(0, dir.y);
            if (IsWalkable(horizontal) && IsWalkable(vertical))
                yield return next;
        }
    }

    private bool IsWalkable(Vector2Int cell)
    {
        return cell.x >= 0 && cell.x < width &&
               cell.y >= 0 && cell.y < height &&
               grid[cell.x, cell.y];
    }
}