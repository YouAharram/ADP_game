using UnityEngine;

public class EnemyDetector : CharacterDetector
{
    [SerializeField] private LayerMask layerNemici;

    public override CharacterStats CharacterInRange(double chaseRange)
    {
        return TrovaPiuVicino<EnemyMobStats>((float)chaseRange, layerNemici);
    }
}