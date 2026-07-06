public interface CharacterVisitor
{
    void VisitPlayer(PlayerEntity playerStats);
    void VisitEnemy(EnemyMobEntity enemyMobEntity);
    void VisitAlly(AllyMobEntity allyMobEntity);
    
}