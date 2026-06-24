using UnityEngine;

public class AllyDetector : CharacterDetector
{
    [SerializeField] private LayerMask layerAlleati;

    public override CharacterStats CharacterInRange(float chaseRange)
    {
        return TrovaPiuVicino<AllyMobStats>(chaseRange, layerAlleati);
    }
}