using PlasticPipe.Client;
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

        public float SpawnDistanceToPlayer = 10.0f;
        public float InitialMoveSpeed = 4.0f;
        public float InitialTurnSpeed = 1.5f;
        public float SpawnIntervalInSeconds = 1.0f;

        public class EnemySpawnerSystemAuthoringBaker : Baker<EnemySpawnSystemAuthoring>
        {
            public override void Bake(EnemySpawnSystemAuthoring systemAuthoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new EnemySpawnComponent(
                    GetEntity(systemAuthoring.Prefab, TransformUsageFlags.Dynamic),
                    systemAuthoring.SpawnDistanceToPlayer,
                    systemAuthoring.SpawnIntervalInSeconds,
                    systemAuthoring.InitialMoveSpeed,
                    systemAuthoring.InitialTurnSpeed,
                    World.DefaultGameObjectInjectionWorld.Time.ElapsedTime + systemAuthoring.SpawnIntervalInSeconds
                ));
            }
        }
    }
}