using RMC.DOTS.SystemGroups;
using RMC.DOTS.Systems.Input;
using RMC.DOTS.Systems.Player;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;

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
            float2 moveComposite = float2.zero;
            
            moveComposite.x = look.x;
            moveComposite.y = look.y;
            
            foreach (var (physicsVelocity, mass, playerMoveComponent) in 
                     SystemAPI.Query<RefRW<PhysicsVelocity>,PhysicsMass, PlayerFaceComponent>().WithAll<PlayerTag>())
            {
                float3 moveComposite3D = new float3(moveComposite.x, 0f, moveComposite.y) * (deltaTime * playerMoveComponent.Value);
                physicsVelocity.ValueRW.ApplyLinearImpulse(in mass, moveComposite3D);
            }
        }
    }
}