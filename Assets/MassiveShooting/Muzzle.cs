using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Mathematics;

namespace AK.MassiveShooting
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Muzzle : IMatrix
    {
        public float3 Position;
        public quaternion Rotation;
        public float Scale;
        public float Duration;
        public float Timer;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float4x4 СomposeMatrix()
        {
            return float4x4.TRS(Position, Rotation, new float3(Scale));
        }
    }
}
