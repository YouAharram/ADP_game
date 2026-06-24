using UnityEngine;
using Mirror;

public abstract class MobAI : NetworkBehaviour
{
    private CharacterController mobController;

    void Start()
    {
        mobController = GetComponent<CharacterController>();
    }
    
}