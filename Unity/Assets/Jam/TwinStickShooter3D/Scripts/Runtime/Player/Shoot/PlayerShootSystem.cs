﻿using RMC.DOTS.SystemGroups;
using RMC.DOTS.Systems.Audio;
using RMC.DOTS.Systems.GameState;
using RMC.DOTS.Systems.Input;
using RMC.DOTS.Systems.PhysicsVelocityImpulse;
using RMC.DOTS.Systems.Player;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Aspects;
using Unity.Transforms;

namespace RMC.DOTS.Samples.Games.TwinStickShooter3D
{
    /// <summary>
    /// This system moves the player in 3D space.
    /// </summary>
    [UpdateInGroup(typeof(PauseableSystemGroup))]
    public partial struct PlayerShootSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<InputComponent>();
            state.RequireForUpdate<PlayerShootSystemAuthoring.PlayerShootSystemIsEnabledTag>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
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
            
            
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            // Get the ArrowKeys for Look from the InputComponent. 
            float2 look = SystemAPI.GetSingleton<InputComponent>().LookFloat2;
            float deltaTime = SystemAPI.Time.DeltaTime;
            
            // If look keys are not pressed, skip this iteration
            if (math.length(look) < 0.0001f)
            {
                return;
            }

            foreach (var playerShootAspect 
                     in SystemAPI.Query<PlayerShootAspect>().WithAll<PlayerTag>())
            {
                // Check if the player can shoot based on the bullet fire rate
                if (playerShootAspect.CanShoot(deltaTime))
                {
                    // Instantiate the entity
                    var instanceEntity = ecb.Instantiate(playerShootAspect.BulletPrefab);
                    
                    // Move entity to initial position
                    ecb.SetComponent<LocalTransform>(instanceEntity, new LocalTransform
                    {
                        Position = playerShootAspect.Position + -playerShootAspect.Forward * 1.5f, //'in front' of the eyes
                        Rotation = quaternion.identity,
                        Scale = 1
                    });

                    // Push entity once
                    var bulletForce = -playerShootAspect.Forward * playerShootAspect.BulletSpeed;
                    ecb.AddComponent<PhysicsVelocityImpulseComponent>(instanceEntity, new PhysicsVelocityImpulseComponent
                    {
                        CanBeNegative = false,
                        MinForce = bulletForce,
                        MaxForce = bulletForce
                    });

                    // Play sound
                    var audioEntity = ecb.CreateEntity();
                    ecb.AddComponent<AudioComponent>(audioEntity, new AudioComponent
                    (
                        "Click01"
                    ));

                    // Update shoot cooldown
                    playerShootAspect.ResetShootCooldown(playerShootAspect.BulletFireRate);
                }
            }
        }
    }
}
