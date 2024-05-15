using RMC.DOTS.SystemGroups;
using RMC.DOTS.Systems.Player;
using RMC.DOTS.Systems.GameState;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Transforms;
using RMC.DOTS.Systems.FollowTarget;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace RMC.DOTS.Samples.Games.TwinStickShooter3D
{
    [UpdateInGroup(typeof(PauseableSystemGroup))]
    [BurstCompile]
    public partial struct EnemyMoveSystem : ISystem
    {
        private ComponentLookup<LocalTransform> _localTransformLookup;
        private LayerMask _layerMask;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EnemyTag>();
            state.RequireForUpdate<EnemyMoveComponent>();
            state.RequireForUpdate<LocalTransform>();
            state.RequireForUpdate<PhysicsMass>();
            state.RequireForUpdate<PhysicsVelocity>();
            _localTransformLookup = state.GetComponentLookup<LocalTransform>();
            state.RequireForUpdate<GameStateComponent>();
        }
		
        public void OnUpdate(ref SystemState state)
        {
            GameStateComponent gameStateComponent = SystemAPI.GetSingleton<GameStateComponent>();

            if (gameStateComponent.GameState != GameState.RoundStarted)
            {
                var ecb = SystemAPI.GetSingleton<BeginPresentationEntityCommandBufferSystem.Singleton>().
                    CreateCommandBuffer(state.WorldUnmanaged);

                foreach (var followerComponent in SystemAPI.Query<RefRW<FollowerComponent>>().WithAll<EnemyTag>())
                {
                    followerComponent.ValueRW.IsEnabled = false;
                }
            }
            else
            {
                var ecb = SystemAPI.GetSingleton<BeginPresentationEntityCommandBufferSystem.Singleton>().
                    CreateCommandBuffer(state.WorldUnmanaged);

                foreach (var followerComponent in SystemAPI.Query<RefRW<FollowerComponent>>().WithAll<EnemyTag>())
                {
                    followerComponent.ValueRW.IsEnabled = true;
                }
            }
        }
    }
}