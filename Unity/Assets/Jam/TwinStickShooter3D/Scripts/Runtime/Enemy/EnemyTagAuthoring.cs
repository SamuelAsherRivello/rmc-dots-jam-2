using Unity.Entities;
using UnityEngine;

namespace RMC.DOTS.Samples.Games.TwinStickShooter3D
{
	public class EnemyTagAuthoring : MonoBehaviour
    {
        public class EnemyTagAuthoringBaker : Baker<EnemyTagAuthoring>
        {
            public override void Bake(EnemyTagAuthoring tagAuthoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                                
                AddComponent<EnemyTag>(entity);
            }
        }
    }
}