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
            
            //Remove any existing BulletWasHitTag
            foreach (var (bulletTag, bulletWasHitTag, entity) 
                     in SystemAPI.Query<BulletTag, BulletWasHitTag>().
                         WithEntityAccess().WithNone<DestroyEntityComponent>())
            {
                //Debug.Log($"GamePickup ({entity.Index}) Set To REMOVE on TimeFrameCount: {Time.frameCount}");
                ecb.RemoveComponent<BulletWasHitTag>(entity);
            }
            
            //Add new BulletWasHitTag
            foreach (var (bulletTag, physicsTriggerOutputComponent, entity) 
                     in SystemAPI.Query<BulletTag, PhysicsTriggerOutputComponent>().
                         WithEntityAccess())
            {
                if (physicsTriggerOutputComponent.PhysicsTriggerType == PhysicsTriggerType.Enter &&
                    physicsTriggerOutputComponent.TimeFrameCountForLastCollision <= Time.frameCount - PhysicsTriggerOutputComponent.FramesToWait)
                { 
                    //Debug.Log($"GamePickup ({entity.Index}) Set To Enter on TimeFrameCount: {Time.frameCount}");
                    ecb.AddComponent<BulletWasHitTag>(entity);
                }
            }
        }
    }
}