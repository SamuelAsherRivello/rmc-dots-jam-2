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
                if (SystemAPI.Time.ElapsedTime < enemySpawnComponent.ValueRW.NextSpawnTime)
                    continue;

                EnemyMoveComponent newEnemyMoveComponent = new EnemyMoveComponent();
                newEnemyMoveComponent.MoveSpeed = enemySpawnComponent.ValueRO.InitialMoveSpeed;
                newEnemyMoveComponent.TurnSpeed = enemySpawnComponent.ValueRO.InitialTurnSpeed;

                Entity newEntity = state.EntityManager.Instantiate(enemySpawnComponent.ValueRO.Prefab);
                state.EntityManager.SetComponentData(newEntity, LocalTransform.FromPosition(enemySpawnComponent.ValueRO.SpawnPosition));
                state.EntityManager.SetComponentData(newEntity, newEnemyMoveComponent);

                enemySpawnComponent.ValueRW.NextSpawnTime = SystemAPI.Time.ElapsedTime + 1000.0f;

                // Add to POSSIBLE points for each enemy
                scoringComponent.ScoreComponent01.ScoreMax += 1;
            }
            
            
            SystemAPI.SetSingleton<ScoringComponent>(scoringComponent);
        }
    }
}