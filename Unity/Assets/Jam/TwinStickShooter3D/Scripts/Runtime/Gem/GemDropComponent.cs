using Unity.Entities;

namespace RMC.DOTS.Samples.Games.TwinStickShooter3D
{
    /// <summary>
    /// Contains all the data needed for the enemy to drop a gem via
    /// <see cref="WasHitSystem"/>
    /// </summary>
    public struct GemDropComponent : IComponentData
    {
        public readonly Entity GemPrefab;   
        public readonly float GemSpeed;      

        public GemDropComponent(Entity gemPrefab, float gemSpeed)
        {
            GemPrefab = gemPrefab;
            GemSpeed = gemSpeed;
        }
    }
}