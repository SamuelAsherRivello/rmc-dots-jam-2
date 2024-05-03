using Unity.Entities;
using UnityEngine;

namespace RMC.DOTS.Samples.Games.TwinStickShooter3D
{
	public class BulletTagAuthoring : MonoBehaviour
    {
        public class BulletTagAuthoringBaker : Baker<BulletTagAuthoring>
        {
            public override void Bake(BulletTagAuthoring tagAuthoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                                
                AddComponent<BulletTag>(entity);
                AddComponent<BulletNotInitializedTag>(entity);
            }
        }
    }
}