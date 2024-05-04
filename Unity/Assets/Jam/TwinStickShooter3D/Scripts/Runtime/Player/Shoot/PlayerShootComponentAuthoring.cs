using Unity.Entities;
using UnityEngine;

namespace RMC.DOTS.Samples.Games.TwinStickShooter3D
{
    public class PlayerShootComponentAuthoring : MonoBehaviour
    {
        public GameObject BulletPrefab;
        public float BulletSpeed = 10;
        public float BulletFireRate = 10;
        
		public class PlayerShootComponentAuthoringBaker : Baker<PlayerShootComponentAuthoring>
        {
            public override void Bake(PlayerShootComponentAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                                
				AddComponent(entity, new PlayerShootComponent
				{
					BulletPrefab = GetEntity(authoring.BulletPrefab, TransformUsageFlags.Dynamic),
					BulletSpeed =  authoring.BulletSpeed,
					BulletFireRate = authoring.BulletFireRate
				});
			}
        }
    }
}