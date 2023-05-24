using UnityEngine;

namespace AK.MassiveShooting
{
    [CreateAssetMenu(fileName = "SO_ShootConfig", menuName = "AK.MassiveShooting/ShootConfig")]
    public sealed class ShootConfig : ScriptableObject
    {
        public Bounds
            Bounds = new(new Vector3(0, 150, 0), new Vector3(300, 200, 300));

        public InstanceConfig
            Muzzle,
            Projectile,
            Impact;

        public float
            Impulse = 30,
            MuzzleScale = 2,
            MuzzleDuration = .1f,
            ImpactScale = 7,
            ImpactDuration = 1;

        public Vector3
            Gravity = new(0, -2, 0);
    }
}
