using RMC.DOTS.SystemGroups;
using RMC.DOTS.Systems.Spawn;
using System.Collections;
using System.Collections.Generic;
using RMC.DOTS.Systems.Scoring;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor.U2D;
using UnityEngine;

namespace RMC.DOTS.Samples.Games.TwinStickShooter3D
{
    [UpdateInGroup(typeof(PauseableSystemGroup))]
    [BurstCompile]
    public partial struct EnemySpawnSystem : ISystem
    {
        public void OnDestroy(ref SystemState state) { }

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EnemySpawnComponent>();
            state.RequireForUpdate<ScoringComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            ScoringComponent scoringComponent = SystemAPI.GetSingleton<ScoringComponent>();
            var deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var enemySpawnComponent in SystemAPI.Query<RefRW<EnemySpawnComponent>>())
            {
                enemySpawnComponent.ValueRW.TimeLeftTillSpawnInSeconds -= deltaTime;
                if (enemySpawnComponent.ValueRW.TimeLeftTillSpawnInSeconds > 0.0f)
                    continue;

                EnemyMoveComponent newEnemyMoveComponent = new EnemyMoveComponent();
                newEnemyMoveComponent.MoveSpeed = enemySpawnComponent.ValueRO.InitialMoveSpeed;

                Entity newEntity = state.EntityManager.Instantiate(enemySpawnComponent.ValueRO.Prefab);
                state.EntityManager.SetComponentData(newEntity, LocalTransform.FromPosition(enemySpawnComponent.ValueRO.SpawnPosition));
                state.EntityManager.SetComponentData(newEntity, newEnemyMoveComponent);

                enemySpawnComponent.ValueRW.TimeLeftTillSpawnInSeconds = enemySpawnComponent.ValueRO.SpawnIntervalInSeconds;
                
                // Add to POSSIBLE points for each enemy
                scoringComponent.ScoreComponent01.ScoreMax += 1;
            }
            
            
            SystemAPI.SetSingleton<ScoringComponent>(scoringComponent);
        }
    }
}