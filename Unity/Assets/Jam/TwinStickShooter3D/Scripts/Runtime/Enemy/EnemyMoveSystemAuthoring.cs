using Unity.Entities;
using UnityEngine;

namespace RMC.DOTS.Samples.Games.TwinStickShooter3D
{
    public class EnemyMoveSystemAuthoring : MonoBehaviour
    {
        public struct EnemyMoveSystemIsEnabledTag : IComponentData { }

        public class EnemyMoveSystemAuthoringBaker : Baker<EnemyMoveSystemAuthoring>
        {
            public override void Bake(EnemyMoveSystemAuthoring authoring)
            {
            }
        }
    }
}