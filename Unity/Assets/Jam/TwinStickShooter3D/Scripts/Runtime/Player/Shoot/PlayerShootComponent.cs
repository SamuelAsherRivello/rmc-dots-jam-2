using Unity.Entities;

namespace RMC.DOTS.Samples.Games.TwinStickShooter3D
{
    public struct PlayerShootComponent : IComponentData
    {
	    // Public variables for managing shooting
		public Entity BulletPrefab;    // Prefab of the bullet entity
		public float BulletSpeed;      // Speed of the bullet
		public float BulletFireRate;   // Cooldown time between shots in seconds

		// Internal variables (expressed as "_") for managing shooting cooldown
		public bool _CanShoot;          // Flag indicating if the player can shoot
		public float _CooldownTimer;    // Timer for the shooting cooldown

		public static PlayerShootComponent Default => new PlayerShootComponent
		{
			BulletSpeed = 10f,         // Example default bullet speed
			BulletFireRate = 0.5f,     // Example default fire rate (one shot every 0.5 seconds)
			_CanShoot = true,           // Player can shoot initially
			_CooldownTimer = 0f         // Initial cooldown timer set to zero
		};
    }
}