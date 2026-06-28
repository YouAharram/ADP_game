using UnityEngine;
using Mirror;

public abstract class MobAI : NetworkBehaviour
{
    private CharacterController mobController;
    private CharacterDetector characterDetector;
    
    [SerializeField] public float chaseRange; 
    [SerializeField] public float hitRange;   
    
    protected CharacterController MobController => mobController;
    protected float ChaseRange => chaseRange;
    protected float HitRange => hitRange;
    protected Vector2 MyPosition => transform.position;
    protected CharacterDetector CharacterDetector => characterDetector;

    protected virtual void Start()
    {
        mobController = GetComponent<CharacterController>();
        characterDetector = GetComponent<CharacterDetector>();
    }
    
    void Update()
    {
        if (!isServer) return;

        CharacterStats characterDetected = characterDetector.CharacterInRange(ChaseRange);
        if (characterDetected != null && CanBeTriggered())  
        {
            Trigger(characterDetected);
        }
        else
        {
            MainGoal();
        }
    }

    protected virtual bool CanBeTriggered()
    {
        return true;
    }

    protected virtual void MainGoal() {}
    protected virtual void Trigger(CharacterStats characterDetected) {}
    
   
}