using UnityEngine;
using System.Collections.Generic;

public class FlowField
{
    public byte[,] costField;
    public ushort[,] integrationField;
    public Vector2[,] vectorField;

    private int width;
    private int height;
    private Vector2Int targetCell;

    public Vector2Int TargetCell => targetCell;

    public FlowField(int w, int h)
    {
        width = w;
        height = h;
        costField = new byte[width, height];
        integrationField = new ushort[width, height];
        vectorField = new Vector2[width, height];
    }

    // 1 per l'erba/strada, 255 per i muri che non sono raggiungibili
    public void GenerateCostField(bool[,] walkableMatrix)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                costField[x, y] = walkableMatrix[x, y] ? (byte)1 : (byte)255;
            }
        }
    }

    public void GenerateIntegrationField(Vector2Int target)
    {
        targetCell = target;

        // Reset mappa a un valore altissimo 
        for (int x = 0; x < width; x++)
        for (int y = 0; y < height; y++)
            integrationField[x, y] = 65535;

        // Se il bersaglio è fuori mappa non si puo fare nulla
        if (!IsInside(target)) return;

        Queue<Vector2Int> cellsToCheck = new Queue<Vector2Int>();

        // Il bersaglio ha distanza 0
        integrationField[target.x, target.y] = 0;
        cellsToCheck.Enqueue(target);

        Vector2Int[] straightDirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        // Algoritmo Dijkstra
        while (cellsToCheck.Count > 0)
        {
            Vector2Int current = cellsToCheck.Dequeue();

            foreach (var dir in straightDirs)
            {
                Vector2Int neighbor = current + dir;

                if (IsInside(neighbor) && costField[neighbor.x, neighbor.y] != 255)
                {

                    ushort costToNeighbor =
                        (ushort)(integrationField[current.x, current.y] + costField[neighbor.x, neighbor.y]);

                    // Se abbiamo trovato una via più breve, aggiorniamo e accodiamo
                    if (costToNeighbor < integrationField[neighbor.x, neighbor.y])
                    {
                        integrationField[neighbor.x, neighbor.y] = costToNeighbor;
                        cellsToCheck.Enqueue(neighbor);
                    }
                }
            }
        }
    }

    // matrice che contiene le direzioni per ogni posizione.
    public void GenerateVectorField()
    {
        Vector2Int[] allDirs =
        {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
            new Vector2Int(1, 1), new Vector2Int(1, -1), new Vector2Int(-1, 1), new Vector2Int(-1, -1)
        };

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Se è un muro o iraggiungibile
                if (costField[x, y] == 255 || integrationField[x, y] == 65535)
                {
                    vectorField[x, y] = Vector2.zero;
                    continue;
                }

                // Nella cella del target la freccia è ferma
                if (x == targetCell.x && y == targetCell.y)
                {
                    vectorField[x, y] = Vector2.zero;
                    continue;
                }

                ushort minDistance = integrationField[x, y];
                Vector2Int bestDir = Vector2Int.zero;

                // Cerca il vicino con la distanza, con integration filed minore
                foreach (var dir in allDirs)
                {
                    Vector2Int neighbor = new Vector2Int(x + dir.x, y + dir.y);

                    if (IsInside(neighbor) && costField[neighbor.x, neighbor.y] != 255)
                    {
                        if (integrationField[neighbor.x, neighbor.y] < minDistance)
                        {
                            minDistance = integrationField[neighbor.x, neighbor.y];
                            bestDir = dir;
                        }
                    }
                }

                vectorField[x, y] = new Vector2(bestDir.x, bestDir.y).normalized;
            }
        }
    }

    public Vector2 GetDirectionAt(Vector2Int gridPos)
    {
        if (IsInside(gridPos)) return vectorField[gridPos.x, gridPos.y];
        return Vector2.zero;
    }

    private bool IsInside(Vector2Int cell)
    {
        return cell.x >= 0 && cell.x < width && cell.y >= 0 && cell.y < height;
    }
}