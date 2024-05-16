using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace RMC.DOTS.Samples.Games.TwinStickShooter3D.TwinStickShooter3D_Version02_DOTS
{
    public struct HealthComponent : IComponentData
    {
        public float CurrentHealth;
        public float MaxHealth;

        public HealthComponent(float newCurrentHealth, float newMaxHealth)
        {
            CurrentHealth = newCurrentHealth;
            MaxHealth = newMaxHealth;
        }
    }
}