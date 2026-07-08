using UnityEngine;
using Mirror;

public class PlayerCameraFollow : NetworkBehaviour
{
    private Transform mainCameraTransform;
    
    [Header("Posizionamento Telecamera")]
    // Modifica questi tre valori dall'Inspector di Unity per decidere l'inquadratura perfetta
    // X = Spostamento laterale (di solito 0)
    // Y = Altezza (es: 5 per guardare dall'alto)
    // Z = Distanza da dietro (es: -10 per stare arretrati)
    [SerializeField] private Vector3 cameraOffset = new Vector3(0, 5f, -10f); 

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        Debug.Log("[CLIENT] Sono il Local Player. Aggancio la telecamera su di me.");

        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }
    }

    private void LateUpdate()
    {
        // Muoviamo la telecamera SOLO se questo personaggio è il nostro giocatore locale
        if (!isLocalPlayer) return;

        // Sicurezza: se la telecamera non era pronta all'inizio, la cerchiamo di nuovo in corsa
        if (mainCameraTransform == null)
        {
            if (Camera.main != null)
            {
                mainCameraTransform = Camera.main.transform;
            }
            else
            {
                return; // Aspetta il prossimo frame se non c'è una telecamera nella scena
            }
        }

        // Calcoliamo la posizione fissa basandoci sulla posizione del Player + l'offset
        mainCameraTransform.position = transform.position + cameraOffset;
        
        // Costringiamo la telecamera a guardare il centro del Player
        mainCameraTransform.LookAt(transform.position + Vector3.up * 1f); 
    }
}