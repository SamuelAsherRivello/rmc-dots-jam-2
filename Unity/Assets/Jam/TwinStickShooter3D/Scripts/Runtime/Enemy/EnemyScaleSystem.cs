using RMC.DOTS.SystemGroups;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace RMC.DOTS.Samples.Games.TwinStickShooter3D
{
    public struct EnemyScaleSystemStartExecuteOnceTag : IComponentData {}
    public struct EnemyScaleSystemEndExecuteOnceTag : IComponentData {}
    
    [UpdateInGroup(typeof(PauseableSystemGroup))]
    [BurstCompile]
    public partial struct EnemyScaleSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<EnemyTag>();
            state.RequireForUpdate<LocalTransform>();
        }
		
        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().
                CreateCommandBuffer(state.WorldUnmanaged);

            var deltaTime = SystemAPI.Time.DeltaTime;
            float scaleSpeed = 10f;
            var fromScale = 0.01f;
            var toScale = 1;
            
            //TODO: Try to use "LitMotion" package (already imported) instead of this
            
            // Set to start scale
            foreach (var (localTransform, entity) in
                     SystemAPI.Query<RefRW<LocalTransform>>().
                         WithNone<EnemyScaleSystemStartExecuteOnceTag>().
                         WithAll<EnemyTag>().
                         WithEntityAccess())
            {

                localTransform.ValueRW.Scale = fromScale;
                // Add tag so we only do this once
                ecb.AddComponent<EnemyScaleSystemStartExecuteOnceTag>(entity);
            }
            
            
            // Scale to end scale
            foreach (var (localTransform, entity) in
                     SystemAPI.Query<RefRW<LocalTransform>>().
                         WithNone<EnemyScaleSystemEndExecuteOnceTag>().
                         WithAll<EnemyScaleSystemStartExecuteOnceTag, EnemyTag>().
                         WithEntityAccess())
            {
                
                localTransform.ValueRW.Scale = 
                    math.lerp(localTransform.ValueRW.Scale, toScale, deltaTime * scaleSpeed);
                
                if (math.distance(localTransform.ValueRW.Scale, toScale) < 0.001f)
                {
                    // Add tag so we only do this once
                    ecb.AddComponent<EnemyScaleSystemEndExecuteOnceTag>(entity);
                }
            }
        }

    }
}