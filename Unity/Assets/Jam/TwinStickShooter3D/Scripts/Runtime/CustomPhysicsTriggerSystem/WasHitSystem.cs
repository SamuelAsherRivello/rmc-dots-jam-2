﻿using System;
using RMC.DOTS.SystemGroups;
using RMC.DOTS.Systems.Audio;
using RMC.DOTS.Systems.DestroyEntity;
using RMC.DOTS.Systems.GameState;
using RMC.DOTS.Systems.PhysicsTrigger;
using RMC.DOTS.Systems.Scoring;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace RMC.DOTS.Samples.Games.TwinStickShooter3D
{
	/// <summary>
	/// This processes the tags previously created by the
	/// <see cref="CustomPhysicsTriggerSystem"/>
	/// </summary>
    [UpdateInGroup(typeof(UnpauseableSystemGroup))]
    public partial class WasHitSystem : SystemBase //CHanged from ISystem, just to have Action<>
    {
	    //  Events ----------------------------------------
	    public Action<Type, bool> OnWasHit;
	    
	    
	    //  Fields ----------------------------------------
	    private ComponentLookup<DestroyEntityComponent> _destroyEntityComponentLookup;
	    private ComponentLookup<GemWasCollectedTag> _gemWasCollectedTagLookup;
	    
	    
	    //  Unity Methods  --------------------------------
		protected override void OnCreate()
        {
            RequireForUpdate<PhysicsTriggerSystemAuthoring.PhysicsTriggerSystemIsEnabledTag>();
            RequireForUpdate<EndInitializationEntityCommandBufferSystem.Singleton>();
            RequireForUpdate<ScoringComponent>();
            RequireForUpdate<GameStateComponent>();
            
            _destroyEntityComponentLookup = GetComponentLookup<DestroyEntityComponent>();
            _gemWasCollectedTagLookup = GetComponentLookup<GemWasCollectedTag>();
        }
		
        protected override void OnUpdate()
        {
	        var ecb = SystemAPI.
		        GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>().
		        CreateCommandBuffer(World.Unmanaged);
	        
	        ScoringComponent scoringComponent = SystemAPI.GetSingleton<ScoringComponent>();

	        _destroyEntityComponentLookup.Update(this);
			_gemWasCollectedTagLookup.Update(this);

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
				
				scoringComponent.ScoreComponent01.ScoreCurrent += 1;
					
				var audioEntity = ecb.CreateEntity();
				ecb.AddComponent<AudioComponent>(audioEntity, new AudioComponent
				{
					AudioClipName = "Pickup01"
				});
				
				// Destroy the Gem
				DestroyEntitySystem.DestroyEntity(ref ecb, _destroyEntityComponentLookup, entity);
				OnWasHit?.Invoke(typeof(GemTag), true);
					
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

				// Instantiate the entity
				var instanceEntity = ecb.Instantiate(gemDropComponent.GemPrefab);
				
				// Move entity to initial position
				ecb.SetComponent<LocalTransform>(instanceEntity, new LocalTransform
				{
					Position = localTransform.Position + -localTransform.Forward() * 1.5f, //'in front' of the eyes
					Rotation = quaternion.identity,
					Scale = 1
				});
				
				// Destroy the enemy
				DestroyEntitySystem.DestroyEntity(ref ecb, _destroyEntityComponentLookup, entity);
				OnWasHit?.Invoke(typeof(EnemyTag), true);
				didDestroyEnemy = true;
			}
			
			
			////////////////////////////////
			// BULLET
			////////////////////////////////
			foreach (var (bulletTag, bulletWasHitTag, entity) 
			         in SystemAPI.Query<BulletTag, BulletWasHitTag>().
				         WithEntityAccess())
			{

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
				
				// HANDLE: Bullet
				DestroyEntitySystem.DestroyEntity(ref ecb, _destroyEntityComponentLookup, entity);
				OnWasHit?.Invoke(typeof(BulletTag), true);

			}
			
			SystemAPI.SetSingleton<ScoringComponent>(scoringComponent);
		}

        
        //  Methods ---------------------------------------


        //  Event Handlers --------------------------------
        
    }
}