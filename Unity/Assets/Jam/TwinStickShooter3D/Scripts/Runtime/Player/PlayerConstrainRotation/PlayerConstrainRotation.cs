// using RMC.DOTS.Systems.Player;
// using Unity.Burst;
// using Unity.Entities;
// using Unity.Mathematics;
// using Unity.Physics.Systems;
// using Unity.Transforms;
// using UnityEngine;
//
// namespace RMC.DOTS.Samples.Games.TwinStickShooter3D
// {
//     [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
//     [UpdateBefore(typeof(PhysicsSystemGroup))]
//     public partial struct PlayerConstrainRotation : ISystem
//     {
//         public void OnCreate(ref SystemState state)
//         {
//             state.RequireForUpdate<PlayerTag>();
//         }
//         
//
//         [BurstCompile]
//         public void OnUpdate(ref SystemState state)
//         {
//             foreach (var localTransform in  
//                      SystemAPI.Query<RefRW<LocalTransform>>().WithAll<PlayerTag>())
//             {
//                 
//                 // //Lock rotation, except y
//                 // localTransform.ValueRW.Rotation = quaternion.Euler(
//                 //     localTransform.ValueRW.Rotation.value.x,
//                 //     localTransform.ValueRW.Rotation.value.y,
//                 //     localTransform.ValueRW.Rotation.value.z
//                 //     );
//             }
//         }
//     }
// }