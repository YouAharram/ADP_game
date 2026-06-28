using UnityEngine;

public abstract class CollisionHandler : MonoBehaviour
{
    private CharacterStats currentCollision;

    public abstract CharacterStats CollidingCharacter();
    
    protected T FindCollidingCharacter<T>(LayerMask targetLayer) where T : CharacterStats
    {
        if (currentCollision != null)
        {
            // se il tipo dell'oggetto con cui sono in contatto è uguale a quello del target lo restituisco
            if (((1 << currentCollision.gameObject.layer) & targetLayer) != 0)
            {
                return currentCollision as T; 
            }
        }

        return null;
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        CharacterStats stats = collision.gameObject.GetComponent<CharacterStats>();
        if (stats != null)
        {
            currentCollision = stats;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (currentCollision != null && collision.gameObject == currentCollision.gameObject)
        {
            currentCollision = null;
        }
    }
}