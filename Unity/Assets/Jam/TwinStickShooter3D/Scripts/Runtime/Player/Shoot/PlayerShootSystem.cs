using RMC.DOTS.SystemGroups;
using RMC.DOTS.Systems.Input;
using RMC.DOTS.Systems.PhysicsVelocityImpulse;
using RMC.DOTS.Systems.Player;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

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

		}

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
			var ecb = new EntityCommandBuffer(Allocator.TempJob);

			// Accept spacebar or enter key
			bool wasPressedThisFrameAction1 = SystemAPI.GetSingleton<InputComponent>().WasPressedThisFrameAction1;
			bool wasPressedThisFrameAction2 = SystemAPI.GetSingleton<InputComponent>().WasPressedThisFrameAction2;

			bool willShoot = wasPressedThisFrameAction1 || wasPressedThisFrameAction2;

			if (willShoot)
			{

				foreach (var (playerShootComponent, localTransform) in
					SystemAPI.Query<RefRO <PlayerShootComponent>, LocalTransform>().WithAll<PlayerTag>())
				{
					var instanceEntity = ecb.Instantiate(playerShootComponent.ValueRO.Prefab);


					// Give a 'push' once in direction the player is facing
					var bulletForce = -localTransform.Forward() * playerShootComponent.ValueRO.Speed;
					var bulletForceNormalize = math.normalize(bulletForce);

					// Move to initial position
					ecb.SetComponent<LocalTransform>(instanceEntity,
						new LocalTransform
							{
								Position = localTransform.Position + bulletForceNormalize * 1.5f, //'in front' of the eyes
								Rotation = quaternion.identity,
								Scale = 1
							});



					ecb.AddComponent<PhysicsVelocityImpulseComponent>(instanceEntity,
						new PhysicsVelocityImpulseComponent
						{
							//FYI Was designed for randomness
							//Here, using it for non-randomness
							CanBeNegative = false,
							MinForce = bulletForce,
							MaxForce = bulletForce,
						});

				}
			}


			ecb.Playback(state.EntityManager);

		}
    }
}