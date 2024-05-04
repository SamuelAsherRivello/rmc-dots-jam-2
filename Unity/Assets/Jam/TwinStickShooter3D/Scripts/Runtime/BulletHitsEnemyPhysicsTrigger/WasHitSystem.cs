using RMC.DOTS.SystemGroups;
using RMC.DOTS.Systems.Audio;
using RMC.DOTS.Systems.DestroyEntity;
using RMC.DOTS.Systems.PhysicsTrigger;
using RMC.DOTS.Systems.Scoring;
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
            state.RequireForUpdate<ScoringComponent>();
            
            _destroyEntityComponentLookup = state.GetComponentLookup<DestroyEntityComponent>();
		}

		
        public void OnUpdate(ref SystemState state)
        {
	        var ecb = SystemAPI.
		        GetSingleton<BeginFixedStepSimulationEntityCommandBufferSystem.Singleton>().
		        CreateCommandBuffer(state.WorldUnmanaged);
	        
	        ScoringComponent scoringComponent = SystemAPI.GetSingleton<ScoringComponent>();
	        
			_destroyEntityComponentLookup.Update(ref state);

			bool didDestroyEnemy = false;
			
			////////////////////////////////
			// ENEMY
			foreach (var (enemyTag, enemyWasHitTag, entity)
				in SystemAPI.Query<EnemyTag, EnemyWasHitTag>().
				WithEntityAccess())
			{
				DestroyEntitySystem.DestroyEntity(ref ecb, _destroyEntityComponentLookup, entity);

				didDestroyEnemy = true;
			}
			
			
			////////////////////////////////
			// BULLET
			foreach (var (bulletTag, bulletWasHitTag, entity) 
			         in SystemAPI.Query<BulletTag, BulletWasHitTag>().
				         WithEntityAccess())
			{
				if (didDestroyEnemy)
				{
					// Give points for killing enemy
					// TODO: I tried putting this in the "ENEMY" loop above and it
					// would reward multiple points per kill. perhaps a delay in the ecb?
					scoringComponent.ScoreComponent01.ScoreCurrent += 1;
				}

				DestroyEntitySystem.DestroyEntity(ref ecb, _destroyEntityComponentLookup, entity);
				
				var audioEntity = ecb.CreateEntity();
				ecb.AddComponent<AudioComponent>(audioEntity, new AudioComponent
				{
					AudioClipName = "Click02"
				});
			}
			
			SystemAPI.SetSingleton<ScoringComponent>(scoringComponent);
		}
    }
}