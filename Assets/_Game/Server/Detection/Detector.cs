using System;
using UnityEngine;

public class Detector : MonoBehaviour
{     
    [SerializeField] private LayerMask layerCharacter;
    [SerializeField] private LayerMask layerCastle;
    [SerializeField] private Vector2 centerOffset;
    
    private Rigidbody2D rb;

    private void Start()
    {
        rb =  GetComponent<Rigidbody2D>();
    }

    public CharacterEntity CharacterInRange(float range)
    {
        return find<CharacterEntity>(range, layerCharacter);
    }
    
    public BuildingEntity BuildingInRange(float range)
    {
        return find<BuildingEntity>(range, layerCastle);
    }

    private T find<T>(float range, LayerMask layerMask) where T : Component
    {
        Vector2 origin = rb.position + centerOffset;
        Collider2D[] collidersNelRaggio = Physics2D.OverlapCircleAll(origin, range, layerMask);

        T bersaglioPiuVicino = null;
        float distanzaMinima = float.MaxValue;

        foreach (Collider2D collider in collidersNelRaggio)
        {
            T potenzialeBersaglio = collider.GetComponentInParent<T>();

            if (potenzialeBersaglio != null)
            {
                float distanza = Vector2.Distance(rb.position, potenzialeBersaglio.gameObject.GetComponent<Rigidbody2D>().position);

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