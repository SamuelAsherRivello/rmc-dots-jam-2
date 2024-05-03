using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace RMC.DOTS.Samples.Games.TwinStickShooter3D
{
    public struct EnemyMoveComponent : IComponentData
    {
        public float MoveSpeed;
    }
}