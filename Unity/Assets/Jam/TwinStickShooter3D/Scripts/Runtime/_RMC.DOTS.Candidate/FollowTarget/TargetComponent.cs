using Unity.Entities;
using UnityEngine;

namespace RMC.DOTS.Samples.Games.TwinStickShooter3D
{
    public struct TargetComponent : IComponentData
    {
        public LayerMask MemberOfLayerMask;
    }
}