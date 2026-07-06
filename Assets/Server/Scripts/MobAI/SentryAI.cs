/// <summary>
/// AI GENERICA per mob "sentinella": non si muove mai e non si cura di
/// edifici, attacca semplicemente chiunque venga rilevato in TriggerRange.
/// Sostituisce ArcherAI (che re-interrogava il Detector inutilmente invece
/// di usare il parametro gia' fornito da MobAI.Update).
///
/// Riusabile per qualsiasi futuro alleato/nemico stazionario: la differenza
/// tra un tipo e l'altro vive nei valori di MobStats/RangedAllyStats nel
/// prefab, non nel codice AI.
/// </summary>
public class SentryAI : MobAI
{
    // CanBeTriggered() default (sempre true) e MainGoal() default (nessuna
    // azione) ereditati da MobAI vanno gia' bene: da fermo non deve fare
    // nulla finche' non rileva qualcuno.

    protected override void Trigger(Entity characterDetected)
    {
        MobEntity.AttackCharacter(TargetInfo.FromEntity(characterDetected));
    }
}
