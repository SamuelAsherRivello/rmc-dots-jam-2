using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

namespace RMC.DOTS.Samples.Games.TwinStickShooter3D
{
    readonly partial struct HealthAspect : IAspect
    {
        readonly RefRW<HealthComponent> HealthComponent;

        public void DealDamage(float damage)
        {
            //Debug.Log($"Dealing damage={damage} to CurrentHealth={CurrentHealth}");
            HealthComponent.ValueRW.CurrentHealth = math.max(0.0f, HealthComponent.ValueRW.CurrentHealth - damage);
        }

        public bool IsDead => HealthComponent.ValueRO.CurrentHealth == 0.0f;

        public float CurrentHealth => HealthComponent.ValueRO.CurrentHealth;
    }
}
