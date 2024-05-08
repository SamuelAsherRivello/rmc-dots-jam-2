using Unity.Entities;
using UnityEngine;

namespace RMC.DOTS.Samples.Games.TwinStickShooter3D
{
    public struct FollowerComponent : IComponentData
    {
        public LayerMask TargetsLayerMask;
        public float LinearSpeed;
        public float AngularSpeed;
        public float Radius;
    }
}