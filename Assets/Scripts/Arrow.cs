using UnityEngine;
using UnityEngine.Pool;

public class Arrow : MonoBehaviour
{
    private Vector2 puntoArrivo;
    private float velocitaFreccia;
    private bool inVolo = false;

    private IObjectPool<Arrow> mangedPool;
    
    // Setter per avere il pool di frecce
    public void SetPool(IObjectPool<Arrow> pool) {
        mangedPool = pool;
    }

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
                ReturnToPool();
            }
        }
    }

    private void ReturnToPool() {
        inVolo = false;
        mangedPool.Release(this);
    }
}