using UnityEngine;
using System.Collections.Generic;

public class FlowFieldManager : MonoBehaviour
{
    public static FlowFieldManager Instance { get; private set; }

    // si i flowfield generati, dove la chiave è la coordinata bersaglio.
    private Dictionary<Vector2Int, FlowField> fieldsCache = new Dictionary<Vector2Int, FlowField>();
    
    [SerializeField] private float cacheLifespan = 0.5f;
    private float cleanTimer;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Update()
    {
        // periodicamente di cancella cancella i vecchi campi generati per i giocaotri in movimento
        if (Time.time > cleanTimer)
        {
            fieldsCache.Clear();
            cleanTimer = Time.time + cacheLifespan;
        }
    }

    public Vector2 GetDirection(Vector2 mobWorldPos, Vector2 targetWorldPos)
    {
        if (MapObstacleGenerator.Instance == null) return Vector2.zero;

        Vector2Int targetGrid = MapObstacleGenerator.Instance.WorldToGridCell(targetWorldPos);
        Vector2Int mobGrid = MapObstacleGenerator.Instance.WorldToGridCell(mobWorldPos);

        // Se non abbiamo ancora generato il percorso per questa cella si cakola
        if (!fieldsCache.ContainsKey(targetGrid))
        {
            FlowField newField = new FlowField(MapObstacleGenerator.Instance.Width, MapObstacleGenerator.Instance.Height);
            
            // Step del flow field 
            newField.GenerateCostField(MapObstacleGenerator.Instance.WalkableMatrix);
            newField.GenerateIntegrationField(targetGrid);
            newField.GenerateVectorField();

            fieldsCache[targetGrid] = newField;
        }

        // Il mob legge la direzione in base alla propria direzione
        Vector2 direction = fieldsCache[targetGrid].GetDirectionAt(mobGrid);

        // Se la freccia è zero, ovvero siamo arrivato siamo fouri dalla mappa
        if (direction == Vector2.zero)
        {
            return (targetWorldPos - mobWorldPos).normalized;
        }

        return direction;
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying || MapObstacleGenerator.Instance == null) return;

        foreach (var kvp in fieldsCache)
        {
            FlowField field = kvp.Value;
            for (int x = 0; x < MapObstacleGenerator.Instance.Width; x++)
            {
                for (int y = 0; y < MapObstacleGenerator.Instance.Height; y++)
                {
                    Vector2 dir = field.vectorField[x, y];
                    if (dir != Vector2.zero)
                    {
                        Vector2 worldPos = MapObstacleGenerator.Instance.GridCellToWorld(new Vector2Int(x, y));
                        Gizmos.color = Color.cyan;
                        DrawGizmoArrow(worldPos, dir, MapObstacleGenerator.Instance.CellSize * 0.4f);
                    }
                }
            }
        }
    }

    private void DrawGizmoArrow(Vector2 pos, Vector2 direction, float length)
    {
        Vector2 endPos = pos + direction * length;
        Gizmos.DrawLine(pos, endPos);
        Vector2 right = Quaternion.Euler(0, 0, 140) * direction;
        Vector2 left = Quaternion.Euler(0, 0, -140) * direction;
        Gizmos.DrawLine(endPos, endPos + right * (length * 0.5f));
        Gizmos.DrawLine(endPos, endPos + left * (length * 0.5f));
    }
}