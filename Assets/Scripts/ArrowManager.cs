using UnityEngine;
using UnityEngine.Pool;

public class ArrowManager : MonoBehaviour
{
    public static ArrowManager Instance { get; private set; }

    [Header("Riferimenti")]
    public Arrow frecciaPrefab; 
    
    [Header("Impostazioni Pool")]
    public int capacitaIniziale = 20;
    public int capacitaMassima = 20;

    private IObjectPool<Arrow> arrowPool;

    private void Awake() 
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 

        arrowPool = new ObjectPool<Arrow>(
            CreazioneNuovaFreccia,    // Cosa fa se il pool è vuoto e serve una freccia
            AzioneAlPrelievo,         // Cosa fa quando prendiamo una freccia dal deposito
            AzioneAlRilascio,         // Cosa fa quando una freccia torna nel deposito
            AzioneAllaDistruzione,    // cosa fa se superiamo la capacità massima
            true,                    
            capacitaIniziale,
            capacitaMassima
        );
    }

    private Arrow CreazioneNuovaFreccia() {
        Arrow nuovaFreccia = Instantiate(frecciaPrefab, transform);
        nuovaFreccia.SetPool(arrowPool);
        nuovaFreccia.gameObject.SetActive(false);
        return nuovaFreccia;
    }

    private void AzioneAlPrelievo(Arrow freccia) {
        freccia.gameObject.SetActive(true);
    }

    private void AzioneAlRilascio(Arrow freccia) {
        freccia.gameObject.SetActive(false);
    }

    private void AzioneAllaDistruzione(Arrow freccia) {
        Destroy(freccia.gameObject);
    }

    public void SparaFreccia(Vector2 partenza, Vector2 arrivo, float speed) 
    {
        Arrow scriptFreccia = arrowPool.Get();
        
        scriptFreccia.transform.position = partenza;
        scriptFreccia.transform.rotation = Quaternion.identity; // nessuna rotazione
        
        scriptFreccia.Scocca(arrivo, speed);
    }
}