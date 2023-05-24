using UnityEngine;

namespace AK.CompetitiveController
{
    [CreateAssetMenu(fileName = "SO_MoveConfig", menuName = "AK.CompetitiveController/MoveConfig")]
    public sealed class MoveConfig : ScriptableObject
    {       
        public float 
            Gravity = 20,
            JumpPower = 10,
            Speed = 8,
            AirFriction = 100,
            ColliderContactOffset = .04f;

        public int
            maxSolveColliders = 16;
        
        public LayerMask 
            groundLayers = 1 << 0;
    }
}