using Unity.Entities;

namespace RMC.DOTS.Samples.Games.TwinStickShooter3D
{
    public struct TweenScaleComponent : IComponentData
    {
        public float Speed;
        public float From;
        public float To;

        public TweenScaleComponent(float from, float to, float speed)
        {
            From = from;
            To = to;
            Speed = speed;
        }
    }
}