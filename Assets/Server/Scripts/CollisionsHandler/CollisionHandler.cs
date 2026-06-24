using UnityEngine;
public abstract class CollisionHandler : MonoBehaviour
{
    private CharacterStats collidingCharacter;
    public CharacterStats CollidingCharacter { get => collidingCharacter; }
}