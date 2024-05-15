using Unity.Entities;

namespace RMC.DOTS.Samples.Games.TwinStickShooter3D
{
    public struct EnemySpawnComponent : IComponentData
    {
        public Entity Prefab;
        public float SpawnDistanceToPlayer;
        public float SpawnIntervalInSeconds;
        public float InitialMoveSpeed;
        public float InitialTurnSpeed;
        public float InitialHealth;

        public double NextSpawnTime;

        public EnemySpawnComponent(
            Entity newPrefab,
            float newSpawnDistanceToPlayer,
            float newSpawnIntervalInSeconds,
            float newInitialMoveSpeed,
            float newInitialTurnSpeed,
            float newInitialHealth,
            double newNextSpawnTime)
        {
            Prefab = newPrefab;
            SpawnDistanceToPlayer = newSpawnDistanceToPlayer;
            SpawnIntervalInSeconds = newSpawnIntervalInSeconds;
            InitialMoveSpeed = newInitialMoveSpeed;
            InitialTurnSpeed = newInitialTurnSpeed;
            InitialHealth = newInitialHealth;

            NextSpawnTime = newNextSpawnTime;
        }
    }
}