using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace RMC.DOTS.Samples.Games.TwinStickShooter3D
{
    public struct EnemySpawnComponent : IComponentData
    {
        public Entity Prefab;
        public float3 SpawnPosition;
        public float SpawnIntervalInSeconds;
        public float InitialMoveSpeed;
        public float InitialTurnSpeed;

        public double NextSpawnTime;

        public EnemySpawnComponent(
            Entity newPrefab,
            float3 newSpawnPosition,
            float newSpawnIntervalInSeconds,
            float newInitialMoveSpeed,
            float newInitialTurnSpeed,
            double newNextSpawnTime)
        {
            Prefab = newPrefab;
            SpawnPosition = newSpawnPosition;
            SpawnIntervalInSeconds = newSpawnIntervalInSeconds;
            InitialMoveSpeed = newInitialMoveSpeed;
            InitialTurnSpeed = newInitialTurnSpeed;

            NextSpawnTime = newNextSpawnTime;
        }
    }
}