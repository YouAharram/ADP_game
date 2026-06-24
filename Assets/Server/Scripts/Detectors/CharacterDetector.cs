using UnityEngine;

public abstract class CharacterDetector : MonoBehaviour
{ 
    public abstract CharacterStats CharacterInRange(float chaseRange);
   
    protected T TrovaPiuVicino<T>(float raggio, LayerMask layerMask) where T : CharacterStats
    {
        Collider2D[] collidersNelRaggio = Physics2D.OverlapCircleAll(transform.position, raggio, layerMask);

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