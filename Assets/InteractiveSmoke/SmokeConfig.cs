using UnityEngine;

namespace AK.InteractiveSmoke
{
    [CreateAssetMenu(fileName = "SO_SmokeConfig", menuName = "AK.InteractiveSmoke/SmokeConfig")]
    public sealed class SmokeConfig : ScriptableObject
    {
        public Bounds
            Bounds = new(new Vector3(0, 10, 0), new Vector3(20, 10, 20));

        public int
            Count = 100000,
            InputCount = 100;

        public float
            InputPower = 2.5f;

        public Mesh Mesh;
        public Material Material;
        public ComputeShader Compute;
    }
}