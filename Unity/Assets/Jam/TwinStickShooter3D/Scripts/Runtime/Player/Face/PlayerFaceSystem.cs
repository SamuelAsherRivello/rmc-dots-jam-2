using RMC.DOTS.SystemGroups;
using RMC.DOTS.Systems.Input;
using RMC.DOTS.Systems.Player;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace RMC.DOTS.Samples.Games.TwinStickShooter3D
{
    /// <summary>
    /// This system moves the player in 3D space.
    /// </summary>
    [UpdateInGroup(typeof(PauseableSystemGroup))]
    public partial struct PlayerFaceSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<InputComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // First get the current input value from the PlayerMoveInput component. This component is set in the
            // GetPlayerInputSystem that runs earlier in the frame.
            float2 look = SystemAPI.GetSingleton<InputComponent>().LookFloat2;
            float deltaTime = SystemAPI.Time.DeltaTime;

            float3 lookComposite = new float3(look.x, look.y, 0);

            foreach (var (physicsVelocity, physicsMass, playerFaceComponent, playerTag) in
                     SystemAPI.Query<RefRW<PhysicsVelocity>, PhysicsMass, PlayerFaceComponent, PlayerTag>())
            {

				// TODO: Add code here to slowly rotation by PlayerFaceComponent.Value as the speed
				// Towards the direction of the lookComposite and use AngularSpeed or something to do it
				if (!math.all(lookComposite.xy == float2.zero))
				{
					
					float3 currentDirection = new float3(0, 0, 0); // Assuming facing forward initially.
					quaternion currentRotation = quaternion.LookRotationSafe(currentDirection, math.up());
					quaternion targetRotation = quaternion.LookRotationSafe(lookComposite, math.up());

					// Calculate the step size for rotation
					float step = playerFaceComponent.Value * deltaTime;

					// Slerp between the current and target rotation
					quaternion slerpedRotation = math.slerp(currentRotation, targetRotation, step);

					// Convert quaternion to euler angles in radians and then to degrees
					float3 euler = math.degrees(math.Euler(slerpedRotation));

					// Set the angular velocity towards target rotation
					physicsVelocity.ValueRW.Angular = euler;
				}
			}

		}
    }
}