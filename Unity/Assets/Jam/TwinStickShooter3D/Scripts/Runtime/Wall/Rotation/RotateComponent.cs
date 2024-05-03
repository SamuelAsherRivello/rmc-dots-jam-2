using Unity.Entities;
using Unity.Mathematics;

namespace RMC.DOTS.Samples.Games.TwinStickShooter3D
{
    public struct RotateComponent : IComponentData
    {
        public float Speed;
        public float3 Direction;
    }
}