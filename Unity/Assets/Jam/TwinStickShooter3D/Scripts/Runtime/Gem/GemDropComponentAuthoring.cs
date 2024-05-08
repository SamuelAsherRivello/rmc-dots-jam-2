using Unity.Entities;
using UnityEngine;

namespace RMC.DOTS.Samples.Games.TwinStickShooter3D
{
    public class GemDropComponentAuthoring : MonoBehaviour
    {
        public GameObject BulletPrefab;
        public float GemSpeed = 10;
        
		public class GemDropComponentAuthoringBaker : Baker<GemDropComponentAuthoring>
        {
            public override void Bake(GemDropComponentAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                                
                //NOTE: CONSTRUCTOR is used to specify the subset of values that is required
				AddComponent(entity, new GemDropComponent 
				(
					GetEntity(authoring.BulletPrefab, TransformUsageFlags.Dynamic), 
					authoring.GemSpeed)
				);
			}
        }
    }
}