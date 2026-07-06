using UnityEngine;
using Mirror;

public abstract class MobAI : NetworkBehaviour
{
    [SerializeField] private float triggerRange;
    
    private CharacterEntity mobEntity;
    private Detector detector; 
    
    private Vector2 targetPosition;

    protected CharacterEntity MobEntity => mobEntity;
    protected Vector2 MyPosition => GetComponent<Rigidbody2D>().position;
    protected Detector Detector => detector;
    
    public Vector2 TargetPosition 
    {
        get => targetPosition;
        set => targetPosition = value;
    }

    protected virtual void Start()
    {
        mobEntity = GetComponent<CharacterEntity>();
        detector = GetComponent<Detector>();
    }
    
    void Update()
    {
        if (!isServer) return;

        Entity characterDetected = detector.CharacterInRange(triggerRange);
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
    protected virtual void Trigger(Entity characterDetected) {}
    
   
}