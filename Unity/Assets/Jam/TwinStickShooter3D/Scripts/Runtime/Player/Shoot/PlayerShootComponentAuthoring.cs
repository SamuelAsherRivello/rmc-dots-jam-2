using RMC.DOTS.Systems.Spawn;
using Unity.Entities;
using UnityEngine;

namespace RMC.DOTS.Samples.Games.TwinStickShooter3D
{
    public class PlayerShootComponentAuthoring : MonoBehaviour
    {
        public GameObject Prefab;

		public class PlayerShootComponentAuthoringBaker : Baker<PlayerShootComponentAuthoring>
        {
            public override void Bake(PlayerShootComponentAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                                
				AddComponent(entity, new PlayerShootComponent
				{
					Prefab = GetEntity(authoring.Prefab, TransformUsageFlags.Dynamic),
				});
			}
        }
    }
}