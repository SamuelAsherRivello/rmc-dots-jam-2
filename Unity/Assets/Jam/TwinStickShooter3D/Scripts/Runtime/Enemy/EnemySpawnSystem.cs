using System;
using RMC.DOTS.SystemGroups;
using RMC.DOTS.Systems.GameState;
using RMC.DOTS.Systems.Scoring;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using RMC.DOTS.Systems.Player;
using RMC.DOTS.Systems.Random;
using Unity.Mathematics;

namespace RMC.DOTS.Samples.Games.TwinStickShooter3D
{
    [UpdateInGroup(typeof(PauseableSystemGroup))]
    [BurstCompile]
    public partial struct EnemySpawnSystem : ISystem
    {
        //  Fields ----------------------------------------
        private ComponentLookup<LocalTransform> _localTransformLookup;

        //  Unity Methods  --------------------------------
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EnemySpawnComponent>();
            state.RequireForUpdate<ScoringComponent>();
            state.RequireForUpdate<GameStateComponent>();
            _localTransformLookup = state.GetComponentLookup<LocalTransform>();
            
            GameStateSystem gameStateSystem = state.World.GetExistingSystemManaged<GameStateSystem>();
            gameStateSystem.OnGameStateChanged += GameStateSystem_OnGameStateChanged;
        }

        
        /// <summary>
        /// Unity does not like me calling this when the scene ends
        /// TODO: Un-listen here or just skip this?
        /// Yes, if this system alone is destroyed?
        /// or NO, if the whole game is destroyed?
        /// </summary>
        public void OnDestroy(ref SystemState state)
        {
            try
            {
                GameStateSystem gameStateSystem = state.World.GetExistingSystemManaged<GameStateSystem>();
                gameStateSystem.OnGameStateChanged -= GameStateSystem_OnGameStateChanged;
            }
            catch (Exception)
            {
                //Debug.Log($"EnemySpawnSystem. OnDestroy Error: {e.Message}");
            }   
        }

        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Check GameStateComponent
            GameStateComponent gameStateComponent = SystemAPI.GetSingleton<GameStateComponent>();
            if (gameStateComponent.GameState != GameState.RoundStarted)
            {
                return;
            }

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
        
        //  Methods ---------------------------------------


        //  Event Handlers --------------------------------
        private void GameStateSystem_OnGameStateChanged(GameState gameState)
        {
            GameStateComponent gameStateComponent = SystemAPI.GetSingleton<GameStateComponent>();
            if (gameStateComponent.GameState == GameState.RoundStarted)
            {
                //Set difficulty
                var useThis = gameStateComponent.RoundData.RoundCurrent;
                var andOrThis = gameStateComponent.RoundData.RoundMax;
                Debug.Log($"EnemySpawnSystem. Increase Difficulty with {useThis} or {andOrThis}.");
            }
        }

    }
}