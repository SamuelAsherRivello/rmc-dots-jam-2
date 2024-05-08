using RMC.DOTS.SystemGroups;
using RMC.DOTS.Systems.Audio;
using RMC.DOTS.Systems.DestroyEntity;
using RMC.DOTS.Systems.PhysicsTrigger;
using RMC.DOTS.Systems.Scoring;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace RMC.DOTS.Samples.Games.TwinStickShooter3D
{
	/// <summary>
	/// This processes the tags previously created by the
	/// <see cref="CustomPhysicsTriggerSystem"/>
	/// </summary>
    [UpdateInGroup(typeof(UnpauseableSystemGroup))]
    public partial struct WasHitSystem : ISystem
    {
	    private ComponentLookup<DestroyEntityComponent> _destroyEntityComponentLookup;
	    private ComponentLookup<GemWasCollectedTag> _gemWasCollectedTagLookup;
	    
		public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PhysicsTriggerSystemAuthoring.PhysicsTriggerSystemIsEnabledTag>();
            state.RequireForUpdate<EndInitializationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<ScoringComponent>();
            
            _destroyEntityComponentLookup = state.GetComponentLookup<DestroyEntityComponent>();
            _gemWasCollectedTagLookup = state.GetComponentLookup<GemWasCollectedTag>();
		}

		
        public void OnUpdate(ref SystemState state)
        {
	        var ecb = SystemAPI.
		        GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>().
		        CreateCommandBuffer(state.WorldUnmanaged);
	        
	        ScoringComponent scoringComponent = SystemAPI.GetSingleton<ScoringComponent>();
	        
			_destroyEntityComponentLookup.Update(ref state);
			_gemWasCollectedTagLookup.Update(ref state);

			
			bool didDestroyEnemy = false;
			
			////////////////////////////////
			// GEM
			////////////////////////////////
			foreach (var (gemTag, gemWasHitTag, entity)
			         in SystemAPI.Query<GemTag, GemWasHitTag>().
				         WithNone<GemWasCollectedTag>().
				         WithEntityAccess())
			{
				
				//HACK: Why do I need this to properly limit this local scope to run once?
				ecb.AddComponent<GemWasCollectedTag>(entity);
				
				// Destroy the Gem
				DestroyEntitySystem.DestroyEntity(ref ecb, _destroyEntityComponentLookup, entity);
				
				scoringComponent.ScoreComponent01.ScoreCurrent += 1;
					
				var audioEntity = ecb.CreateEntity();
				ecb.AddComponent<AudioComponent>(audioEntity, new AudioComponent
				{
					AudioClipName = "Pickup01"
				});
					
			}

			
			////////////////////////////////
			// ENEMY
			////////////////////////////////
			foreach (var (enemyTag, enemyWasHitTag, localTransform, gemDropComponent, entity)
				in SystemAPI.Query<EnemyTag, EnemyWasHitTag, LocalTransform, GemDropComponent>().
					WithNone<GemWasCreatedTag>().
				WithEntityAccess())
			{
				
				//HACK: Why do I need this to properly limit this local scope to run once?
				ecb.AddComponent<GemWasCreatedTag>(entity);
				
				// Destroy the enemy
				DestroyEntitySystem.DestroyEntity(ref ecb, _destroyEntityComponentLookup, entity);
				didDestroyEnemy = true;
				
				// Instantiate the entity
				var instanceEntity = ecb.Instantiate(gemDropComponent.GemPrefab);
				
				// Move entity to initial position
				ecb.SetComponent<LocalTransform>(instanceEntity, new LocalTransform
				{
					Position = localTransform.Position + -localTransform.Forward() * 1.5f, //'in front' of the eyes
					Rotation = quaternion.identity,
					Scale = 1
				});
			}
			
			
			////////////////////////////////
			// BULLET
			////////////////////////////////
			foreach (var (bulletTag, bulletWasHitTag, entity) 
			         in SystemAPI.Query<BulletTag, BulletWasHitTag>().
				         WithEntityAccess())
			{
				
				// HANDLE: Bullet
				DestroyEntitySystem.DestroyEntity(ref ecb, _destroyEntityComponentLookup, entity);


				// HANDLE: Enemy
				// HACK: Use bool to avoid double-hit. Not sure why I need this boolean.
				if (didDestroyEnemy)
				{
					var audioEntity = ecb.CreateEntity();
					ecb.AddComponent<AudioComponent>(audioEntity, new AudioComponent
					{
						AudioClipName = "Click02"
					});
				}
			}
			
			SystemAPI.SetSingleton<ScoringComponent>(scoringComponent);
		}
    }
}