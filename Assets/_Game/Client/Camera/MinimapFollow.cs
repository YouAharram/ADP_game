using UnityEngine;
using Mirror;

// Questo script vive sul Player (prefab): ogni client, quando diventa
// il "local player", aggancia la MiniMapCamera presente nella scena
// alla propria posizione. In multiplayer non si puo' assegnare il
// player come riferimento statico in scena (e' un'istanza di rete
// creata a runtime), quindi il follow va fatto dal player stesso.
public class MinimapFollow : NetworkBehaviour
{
    [Tooltip("Nome esatto del GameObject della camera minimappa nella scena")]
    [SerializeField] private string minimapCameraName = "MiniMapCamera";

    [Tooltip("Distanza della minimap camera dal piano 2D (deve essere negativa)")]
    [SerializeField] private float zOffset = -10f;

    private Transform minimapCameraTransform;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        FindMinimapCamera();
    }

    private void FindMinimapCamera()
    {
        GameObject cam = GameObject.Find(minimapCameraName);
        if (cam != null)
        {
            minimapCameraTransform = cam.transform;
        }
        else
        {
            Debug.LogWarning($"[MinimapFollow] Nessun GameObject chiamato '{minimapCameraName}' trovato nella scena.");
        }
    }

    void LateUpdate()
    {
        if (!isLocalPlayer) return;

        if (minimapCameraTransform == null)
        {
            FindMinimapCamera();
            if (minimapCameraTransform == null) return;
        }

        minimapCameraTransform.position = new Vector3(transform.position.x, transform.position.y, zOffset);
    }
}
