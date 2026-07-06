using UnityEngine;

public class TargetInfo
{
    public Vector2 Position;
    public Entity Entity;

    public TargetInfo(Vector2 position, Entity entity)
    {
        Position = position;
        Entity = entity;
    }

    public static TargetInfo FromEntity(Entity e) => new TargetInfo(e.transform.position, e);
    public static TargetInfo FromPosition(Vector2 p) => new TargetInfo(p, null);
}