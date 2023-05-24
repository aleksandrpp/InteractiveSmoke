using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace AK.InteractiveSmoke
{
    [BurstCompile]
    public struct PropagateJob : IJobParallelFor
    {
        private const float NoiseScale = .2f;
        private const float NoiseClamp = .25f;

        [ReadOnly] public Bounds Bounds;

        [WriteOnly] public NativeArray<float3> Points;
        [WriteOnly] public NativeArray<float3> Colors;

        public void Execute(int index)
        {
            var rnd = new Random((uint)index + 1);
            float3 position;
            float color;

            // achtung
            while (true)
            {
                position = rnd.NextFloat3(Bounds.min, Bounds.max);
                color = noise.cnoise(position * NoiseScale);

                if (color > NoiseClamp)
                    break;
            }

            Points[index] = position;
            Colors[index] = color * .5f;
        }
    }
}