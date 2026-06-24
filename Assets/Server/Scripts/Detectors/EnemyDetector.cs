using UnityEngine;

public class EnemyDetector : CharacterDetector
{
    [SerializeField] private LayerMask layerNemici;

    public override CharacterStats CharacterInRange(float chaseRange)
    {
        return TrovaPiuVicino<EnemyMobStats>(chaseRange, layerNemici);
    }
}