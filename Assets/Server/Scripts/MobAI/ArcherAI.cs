using UnityEngine;

public class ArcherAI : MobAI
{

    protected override void Trigger(CharacterStats characterDecteted)
    {
        if (CharacterDetector.CharacterInRange(HitRange) != null)
        {
            MobController.Attack();
        }
    }
    
}