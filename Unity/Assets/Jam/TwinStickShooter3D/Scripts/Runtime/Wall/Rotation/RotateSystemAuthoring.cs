using Unity.Entities;
using UnityEngine;

namespace RMC.DOTS.Samples.Games.TwinStickShooter3D
{
    public class RotateSystemAuthoring : MonoBehaviour
    {
        [SerializeField] 
        public bool IsSystemEnabled = true;
        
        public struct RotateSystemIsEnabledTag : IComponentData {}
        
        public class RotateSystemAuthoringBaker : Baker<RotateSystemAuthoring>
        {
            public override void Bake(RotateSystemAuthoring authoring)
            {
                if (authoring.IsSystemEnabled)
                {
                    Entity inputEntity = GetEntity(TransformUsageFlags.Dynamic);
                    AddComponent<RotateSystemIsEnabledTag>(inputEntity);
                }
            }
        }
    }
}