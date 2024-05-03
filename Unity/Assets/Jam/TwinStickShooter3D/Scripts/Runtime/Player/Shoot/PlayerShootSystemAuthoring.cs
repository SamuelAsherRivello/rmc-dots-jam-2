using Unity.Entities;
using UnityEngine;

namespace RMC.DOTS.Samples.Games.TwinStickShooter3D
{
    public class PlayerShootSystemAuthoring : MonoBehaviour
    {
        [SerializeField] 
        public bool IsSystemEnabled = true;
        
        public struct PlayerShootSystemIsEnabledTag : IComponentData {}
        
        public class PlayerShootSystemAuthoringBaker : Baker<PlayerShootSystemAuthoring>
        {
            public override void Bake(PlayerShootSystemAuthoring authoring)
            {
                if (authoring.IsSystemEnabled)
                {
                    Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                    AddComponent<PlayerShootSystemIsEnabledTag>(entity);
				}
            }
        }
    }
}

