using RMC.DOTS.SystemGroups;
using RMC.DOTS.Systems.Input;
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

				foreach (var playerShootComponent in
					SystemAPI.Query<RefRO <PlayerShootComponent>>().WithAll<PlayerTag>())
				{
					var instanceEntity = ecb.Instantiate(playerShootComponent.ValueRO.Prefab);

					ecb.SetComponent<LocalTransform>(instanceEntity,
						new LocalTransform
							{
								Position = new Unity.Mathematics.float3(2, 2, 2),
								Rotation = quaternion.identity,
								Scale = 1
							});

				}
			}


			ecb.Playback(state.EntityManager);

		}
    }
}