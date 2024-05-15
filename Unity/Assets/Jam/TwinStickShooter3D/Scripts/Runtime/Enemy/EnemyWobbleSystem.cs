using RMC.DOTS.SystemGroups;
using RMC.DOTS.Systems.GameState;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace RMC.DOTS.Samples.Games.TwinStickShooter3D
{
    [UpdateInGroup(typeof(PauseableSystemGroup))]
    [BurstCompile]
    public partial struct EnemyWobbleSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EnemyTag>();
            state.RequireForUpdate<LocalTransform>();
            state.RequireForUpdate<GameStateComponent>();
        }
		
        public void OnUpdate(ref SystemState state)
        {
            // Check GameStateComponent
            GameStateComponent gameStateComponent = SystemAPI.GetSingleton<GameStateComponent>();
            if (gameStateComponent.GameState != GameState.RoundStarted)
            {
                return;
            }

            float deltaTime = SystemAPI.Time.DeltaTime;
            float elapsedTime = (float)state.WorldUnmanaged.Time.ElapsedTime;
            // Wobble effect parameters
            float wobbleFrequency = 2.0f; // Frequency of the wobble
            float wobbleAmplitude = 0.5f; // Amplitude of the wobble
            
            foreach (var (physicsVelocity, localTransform) in
                     SystemAPI.Query<RefRW<PhysicsVelocity>, RefRW<LocalTransform>>().WithAll<EnemyTag>())
            {

                // Calculate the wobble amount
                float wobbleAmount = math.sin( elapsedTime * wobbleFrequency) * wobbleAmplitude;

                // Apply the wobble to the localTransform
                localTransform.ValueRW.Position.x += wobbleAmount * deltaTime * physicsVelocity.ValueRO.Linear.x;
            }
        }
    }
}