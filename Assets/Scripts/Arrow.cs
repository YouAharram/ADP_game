using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Vector2 puntoArrivo;
    private float velocitaFreccia;
    private bool inVolo = false;
    
    public void Scocca(Vector2 arrivo, float velocita) 
    {
        puntoArrivo = arrivo;
        velocitaFreccia = velocita;
        inVolo = true;

        // direzione freccia
        Vector2 direzione = puntoArrivo - (Vector2)transform.position;
        float angolo = Mathf.Atan2(direzione.y, direzione.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0,0,angolo);
    }

    void Update() 
    {
        if(inVolo) 
        {
            transform.position = Vector2.MoveTowards(transform.position, puntoArrivo, velocitaFreccia * Time.deltaTime);

            if((Vector2)transform.position == puntoArrivo) 
            {
                // todo: aggiungere danno ai nemici
                Destroy(gameObject);
            }
        }
    }
}