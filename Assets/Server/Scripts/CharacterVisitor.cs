public interface CharacterVisitor
{
    void VisitPlayer(PlayerStats playerStats);
    void VisitEnemy(EnemyMobStats enemyMobStats);
    void VisitAlly(AllyMobStats allyMobStats);
    
}