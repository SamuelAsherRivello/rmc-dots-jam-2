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
using RMC.DOTS.Systems.Player;
using RMC.DOTS.Systems.Random;

namespace RMC.DOTS.Samples.Games.TwinStickShooter3D
{
    [UpdateInGroup(typeof(PauseableSystemGroup))]
    [BurstCompile]
    public partial struct EnemySpawnSystem : ISystem
    {
        private ComponentLookup<LocalTransform> _localTransformLookup;

        public void OnDestroy(ref SystemState state) { }

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EnemySpawnComponent>();
            state.RequireForUpdate<ScoringComponent>();
            _localTransformLookup = state.GetComponentLookup<LocalTransform>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _localTransformLookup.Update(ref state);
            var playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();
            float3 currentPlayerPosition = _localTransformLookup[playerEntity].Position;

            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var beginSimulationECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            ScoringComponent scoringComponent = SystemAPI.GetSingleton<ScoringComponent>();
            var deltaTime = SystemAPI.Time.DeltaTime;

            var randomComponentEntity = SystemAPI.GetSingletonEntity<RandomComponent>();
            var randomComponentAspect = SystemAPI.GetAspect<RandomComponentAspect>(randomComponentEntity);

            foreach (var enemySpawnComponent in SystemAPI.Query<RefRW<EnemySpawnComponent>>())
            {
                if (SystemAPI.Time.ElapsedTime < enemySpawnComponent.ValueRW.NextSpawnTime)
                    continue;

                EnemyMoveComponent newEnemyMoveComponent = new EnemyMoveComponent(
                    enemySpawnComponent.ValueRO.InitialMoveSpeed,
                    enemySpawnComponent.ValueRO.InitialTurnSpeed
                );

                float2 randomVectorOnGround = math.normalizesafe(new float2(
                    randomComponentAspect.NextFloat(-1.0f, 1.0f),
                    randomComponentAspect.NextFloat(-1.0f, 1.0f)
                )) * enemySpawnComponent.ValueRO.SpawnDistanceToPlayer;
                
                const float spawnHeight = 4.0f;
                float3 spawnVector = new float3(randomVectorOnGround.x, spawnHeight, randomVectorOnGround.y);

                float3 newEnemyPosition = currentPlayerPosition + spawnVector;

                Entity newEntity = beginSimulationECB.Instantiate(enemySpawnComponent.ValueRO.Prefab);
                beginSimulationECB.SetComponent(newEntity, LocalTransform.FromPosition(newEnemyPosition));
                beginSimulationECB.SetComponent(newEntity, newEnemyMoveComponent);
                
                enemySpawnComponent.ValueRW.NextSpawnTime = SystemAPI.Time.ElapsedTime + enemySpawnComponent.ValueRO.SpawnIntervalInSeconds;

                // Add to POSSIBLE points for each enemy
                scoringComponent.ScoreComponent01.ScoreMax += 1;
            }
            
            
            SystemAPI.SetSingleton<ScoringComponent>(scoringComponent);
        }
    }
}