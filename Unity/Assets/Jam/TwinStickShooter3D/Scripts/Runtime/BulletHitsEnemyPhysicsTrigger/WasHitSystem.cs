using RMC.DOTS.SystemGroups;
using RMC.DOTS.Systems.DestroyEntity;
using RMC.DOTS.Systems.PhysicsTrigger;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace RMC.DOTS.Samples.Games.TwinStickShooter3D
{
    [UpdateInGroup(typeof(UnpauseableSystemGroup))]
    public partial struct WasHitSystem : ISystem
    {
	    private ComponentLookup<DestroyEntityComponent> _destroyEntityComponentLookup;
	
		public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PhysicsTriggerSystemAuthoring.PhysicsTriggerSystemIsEnabledTag>();
            state.RequireForUpdate<BeginFixedStepSimulationEntityCommandBufferSystem.Singleton>();
            _destroyEntityComponentLookup = state.GetComponentLookup<DestroyEntityComponent>();
		}

		
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
	        var ecb = SystemAPI.
		        GetSingleton<BeginFixedStepSimulationEntityCommandBufferSystem.Singleton>().
		        CreateCommandBuffer(state.WorldUnmanaged);
	        
			_destroyEntityComponentLookup.Update(ref state);
			
			////////////////////////////////
			// BULLET
			foreach (var (bulletTag, bulletWasHitTag, entity) 
                in SystemAPI.Query<BulletTag, BulletWasHitTag>().
                WithEntityAccess())
			{
				DestroyEntitySystem.DestroyEntity(ref ecb, _destroyEntityComponentLookup, entity, 0);
			}

			////////////////////////////////
			// ENEMY
			foreach (var (enemyTag, enemyWasHitTag, entity)
				in SystemAPI.Query<EnemyTag, EnemyWasHitTag>().
				WithEntityAccess())
			{
				DestroyEntitySystem.DestroyEntity(ref ecb, _destroyEntityComponentLookup, entity, 0);
			}
		}
    }
}