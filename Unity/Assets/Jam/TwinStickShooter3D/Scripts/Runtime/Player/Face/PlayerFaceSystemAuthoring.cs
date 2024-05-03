using Unity.Entities;
using UnityEngine;

namespace RMC.DOTS.Samples.Games.TwinStickShooter3D
{
    public class PlayerFaceSystemAuthoring : MonoBehaviour
    {
        [SerializeField] 
        public bool IsSystemEnabled = true;
        
        public struct PlayerFaceSystemIsEnabledTag : IComponentData {}
        
        public class PlayerFaceSystemAuthoringBaker : Baker<PlayerFaceSystemAuthoring>
        {
            public override void Bake(PlayerFaceSystemAuthoring authoring)
            {
                if (authoring.IsSystemEnabled)
                {
                    Entity inputEntity = GetEntity(TransformUsageFlags.Dynamic);
                    AddComponent<PlayerFaceSystemIsEnabledTag>(inputEntity);
                }
            }
        }
    }
}