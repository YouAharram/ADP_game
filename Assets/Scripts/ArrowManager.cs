using UnityEngine;

public class ArrowManager : MonoBehaviour
{
    public static ArrowManager Instance { get; private set; }

    public GameObject frecciaPrefab; 

    // TODO: renderlo un pool di frecce invece che creare e distruggere
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
    }

    public void SparaFreccia(Vector2 partenza, Vector2 arrivo, float speed) 
    {
        GameObject nuovaFreccia = Instantiate(frecciaPrefab, partenza, Quaternion.identity);
        
        Arrow scriptFreccia = nuovaFreccia.GetComponent<Arrow>();
        
        scriptFreccia.Scocca(arrivo, speed);
    }
}