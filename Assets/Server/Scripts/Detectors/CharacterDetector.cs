using UnityEngine;

public abstract class CharacterDetector : MonoBehaviour
{ 
    [SerializeField] private Vector2 centerOffset;
    public abstract CharacterStats CharacterInRange(float chaseRange);
   
    protected T TrovaPiuVicino<T>(float raggio, LayerMask layerMask) where T : CharacterStats
    {
        Vector2 origin = (Vector2)transform.position + centerOffset;
        Collider2D[] collidersNelRaggio = Physics2D.OverlapCircleAll(origin, raggio, layerMask);

        T bersaglioPiuVicino = null;
        float distanzaMinima = float.MaxValue;

        foreach (Collider2D collider in collidersNelRaggio)
        {
            T potenzialeBersaglio = collider.GetComponent<T>();

            if (potenzialeBersaglio != null)
            {
                float distanza = Vector2.Distance(transform.position, potenzialeBersaglio.transform.position);

                if (distanza < distanzaMinima)
                {
                    distanzaMinima = distanza;
                    bersaglioPiuVicino = potenzialeBersaglio;
                }
            }
        }

        return bersaglioPiuVicino;
    }
}