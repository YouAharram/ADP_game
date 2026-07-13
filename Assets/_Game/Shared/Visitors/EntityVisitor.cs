public interface EntityVisitor
{
    void VisitPlayer(PlayerEntity playerEntity);
    void VisitEnemy(EnemyMobEntity enemyMobEntity);
    void VisitBuilding(BuildingEntity allyMobEntity);
    
}