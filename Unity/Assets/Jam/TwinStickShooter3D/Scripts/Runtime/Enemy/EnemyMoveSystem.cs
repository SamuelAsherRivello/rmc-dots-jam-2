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

namespace RMC.DOTS.Samples.Games.TwinStickShooter3D
{
    [UpdateInGroup(typeof(PauseableSystemGroup))]
    [BurstCompile]
    public partial struct EnemyMoveSystem : ISystem
    {
        private ComponentLookup<LocalTransform> _localTransformLookup;

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
            // Check GameStateComponent
            GameStateComponent gameStateComponent = SystemAPI.GetSingleton<GameStateComponent>();
            if (gameStateComponent.GameState != GameState.RoundStarted)
            {
                return;
            }

            _localTransformLookup.Update(ref state);
            
            var playerEntities = state.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<PlayerTag>()).ToEntityArray(Allocator.Temp);

            float3 currentPlayerPosition = float3.zero;
            foreach (var playerEntity in playerEntities)
            {
                currentPlayerPosition = _localTransformLookup[playerEntity].Position;
            }

            float deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var (physicsVelocity, localTransform, physicsMass, enemyMoveComponent) in
                     SystemAPI.Query<RefRW<PhysicsVelocity>, RefRW<LocalTransform>, PhysicsMass, EnemyMoveComponent>().WithAll<EnemyTag>())
            {
                float3 enemyToPlayer = currentPlayerPosition - localTransform.ValueRO.Position;
                enemyToPlayer = math.normalizesafe(enemyToPlayer);

                physicsVelocity.ValueRW.ApplyLinearImpulse(in physicsMass, deltaTime * enemyMoveComponent.MoveSpeed * enemyToPlayer);

                quaternion targetRotation = quaternion.EulerXYZ(new float3(0.0f, math.atan2(enemyToPlayer.x, enemyToPlayer.z), 0.0f));
                localTransform.ValueRW.Rotation = math.slerp(localTransform.ValueRO.Rotation, targetRotation, enemyMoveComponent.TurnSpeed * deltaTime);
            }
        }
    }
}