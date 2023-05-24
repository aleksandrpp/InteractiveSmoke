using UnityEngine;

namespace AK.CompetitiveMan
{
    [CreateAssetMenu(fileName = "SO_ManConfig", menuName = "AK.CompetitiveMan/ManConfig")]
    public sealed class ManConfig : ScriptableObject
    {
        public Vector3
            CameraOffset = new(.55f, 1.75f, -1.5f);

        public float
            AimDistance = 25,
            XSens = 120,
            YSens = 120,
            ApplyAnimSpeed = 6,
            AnimThreshold = 1,
            FallDelay = .25f,
            MaxVerticalAngle = 85,
            MinVerticalAngle = -85;
    }
}