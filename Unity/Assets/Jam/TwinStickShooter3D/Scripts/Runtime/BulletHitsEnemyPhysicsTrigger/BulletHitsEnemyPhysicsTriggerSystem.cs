using RMC.DOTS.SystemGroups;
using RMC.DOTS.Systems.DestroyEntity;
using RMC.DOTS.Systems.PhysicsTrigger;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace RMC.DOTS.Samples.Games.TwinStickShooter3D
{
    [UpdateInGroup(typeof(UnpauseableSystemGroup))]
    public partial struct BulletHitsEnemyPhysicsTriggerSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PhysicsTriggerSystemAuthoring.PhysicsTriggerSystemIsEnabledTag>();
            state.RequireForUpdate<BeginPresentationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.
                GetSingleton<BeginPresentationEntityCommandBufferSystem.Singleton>().
                CreateCommandBuffer(state.WorldUnmanaged);
            
            //BULLET
            foreach (var (physicsTriggerOutputComponent, entity) 
                     in SystemAPI.Query<PhysicsTriggerOutputComponent>().
                         WithEntityAccess().WithNone<BulletWasHitTag>())
            {
                ecb.AddComponent<BulletWasHitTag>(entity);
            }

			//ENEMY
			foreach (var (physicsTriggerOutputComponent, entity)
					 in SystemAPI.Query<PhysicsTriggerOutputComponent>().
						 WithEntityAccess().WithNone<EnemyWasHitTag>())
			{
				ecb.AddComponent<EnemyWasHitTag>(entity);
			}
		}
    }
}