using UnityEngine;

namespace AK.MassiveShooting
{
    [CreateAssetMenu(fileName = "SO_InstanceConfig", menuName = "AK.MassiveShooting/InstanceConfig")]
    public sealed class InstanceConfig : ScriptableObject
    {
        public Mesh Mesh;
        public Material Material;
    }
}
