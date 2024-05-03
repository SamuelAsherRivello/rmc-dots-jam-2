using Unity.Entities;
using UnityEngine;

namespace RMC.DOTS.Samples.Games.TwinStickShooter3D
{
    public class EnemyMoveComponentAuthoring : MonoBehaviour
    {
        public float MoveSpeed = 10.0f;

        public class EnemyMoveComponentAuthoringBaker : Baker<EnemyMoveComponentAuthoring>
        {
            public override void Bake(EnemyMoveComponentAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<EnemyMoveComponent>(entity, new EnemyMoveComponent {
                    MoveSpeed = authoring.MoveSpeed,
                });
            }
        }
    }
}