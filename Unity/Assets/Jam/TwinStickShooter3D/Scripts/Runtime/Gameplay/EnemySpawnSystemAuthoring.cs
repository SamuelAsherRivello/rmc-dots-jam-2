using RMC.DOTS.Systems.Spawn;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace RMC.DOTS.Samples.Games.TwinStickShooter3D
{
    /// <summary>
    /// Place this MonoBehaviour on a GameObject in the Scene
    /// To enable the <see cref="SpawnSystem"/>
    /// </summary>

    public class EnemySpawnSystemAuthoring : MonoBehaviour
    {
        public GameObject Prefab;

        public float SpawnIntervalInSeconds = 1.0f;

        public class EnemySpawnerSystemAuthoringBaker : Baker<EnemySpawnSystemAuthoring>
        {
            public override void Bake(EnemySpawnSystemAuthoring systemAuthoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new EnemySpawnComponent
                {
                    Prefab = GetEntity(systemAuthoring.Prefab, TransformUsageFlags.Dynamic),
                    SpawnPosition = systemAuthoring.transform.position,
                    SpawnIntervalInSeconds = systemAuthoring.SpawnIntervalInSeconds,

                    TimeLeftTillSpawnInSeconds = systemAuthoring.SpawnIntervalInSeconds,
                });
            }
        }
    }
}