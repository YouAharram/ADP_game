using UnityEngine;

public class AllyDetector : CharacterDetector
{
    [SerializeField] private LayerMask layerAlleati;

    public override CharacterStats CharacterInRange(double chaseRange)
    {
        return TrovaPiuVicino<AllyMobStats>((float)chaseRange, layerAlleati);
    }
}