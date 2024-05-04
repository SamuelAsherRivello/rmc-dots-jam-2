using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace RMC.DOTS.Samples.Games.TwinStickShooter3D
{
    public struct EnemySpawnComponent : IComponentData
    {
        public Entity Prefab;
        public Vector3 SpawnPosition;
        public float SpawnIntervalInSeconds;
        public float InitialMoveSpeed;

        public float TimeLeftTillSpawnInSeconds;
    }
}