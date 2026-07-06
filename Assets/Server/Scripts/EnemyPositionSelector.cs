using UnityEngine;

public interface EnemyPositionSelector
{
   static Vector2 RandomPosition(Rect mapBounds)
    {
        return new Vector2(
            Random.Range(mapBounds.min.x, mapBounds.max.x), 
            Random.Range(mapBounds.min.y, mapBounds.max.y));
    }
}