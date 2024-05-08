using RMC.DOTS.SystemGroups;
using RMC.DOTS.Systems.GameState;
using RMC.DOTS.Systems.PhysicsTrigger;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace RMC.DOTS.Samples.Games.TwinStickShooter3D
{
    
    /// <summary>
    ///
    /// 1. This 'listens to' <see cref="PhysicsTriggerSystem"/>
    /// and sets a tag on each entity that was hit.
    ///
    /// 2. Separately the <see cref="WasHitSystem"/> will 'listen' for these tags
    /// 
    /// </summary>
    [UpdateInGroup(typeof(UnpauseableSystemGroup))]
    public partial struct CustomPhysicsTriggerSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PhysicsTriggerSystemAuthoring.PhysicsTriggerSystemIsEnabledTag>();
            state.RequireForUpdate<BeginPresentationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<GameStateComponent>();
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
            
            
            var ecb = SystemAPI.
                GetSingleton<BeginPresentationEntityCommandBufferSystem.Singleton>().
                CreateCommandBuffer(state.WorldUnmanaged);
            
            //GEM
            foreach (var (physicsTriggerOutputComponent, entity) 
                     in SystemAPI.Query<RefRO<PhysicsTriggerOutputComponent>>().
                         WithEntityAccess().WithAll<GemTag>().WithNone<GemWasHitTag>())
            {
                ecb.AddComponent<GemWasHitTag>(entity);
            }
            
            
            //BULLET
            foreach (var (physicsTriggerOutputComponent, entity) 
                     in SystemAPI.Query<RefRO<PhysicsTriggerOutputComponent>>().
                         WithEntityAccess().WithAll<BulletTag>().WithNone<BulletWasHitTag>())
            {
                ecb.AddComponent<BulletWasHitTag>(entity);
            }

			//ENEMY
			foreach (var (physicsTriggerOutputComponent, entity)
					 in SystemAPI.Query<RefRO<PhysicsTriggerOutputComponent>>().
						 WithEntityAccess().WithAll<EnemyTag>().WithNone<EnemyWasHitTag>())
			{
				ecb.AddComponent<EnemyWasHitTag>(entity);
			}
		}
    }
}