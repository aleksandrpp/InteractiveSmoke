using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace AK.MassiveShooting
{
    [BurstCompile]
    public struct ProjectileMatrixJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<Projectile> Projectiles;
        [WriteOnly] public NativeArray<Matrix4x4> Matrices;

        public void Execute(int index)
        {
            Matrices[index] = Projectiles[index].СomposeMatrix();
        }
    }

    [BurstCompile]
    public struct ImpactMatrixJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<Impact> Impacts;
        [WriteOnly] public NativeArray<Matrix4x4> Matrices;

        public void Execute(int index)
        {
            Matrices[index] = Impacts[index].СomposeMatrix();
        }
    }

    [BurstCompile]
    public struct MuzzleMatrixJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<Muzzle> Muzzles;
        [WriteOnly] public NativeArray<Matrix4x4> Matrices;

        public void Execute(int index)
        {
            Matrices[index] = Muzzles[index].СomposeMatrix();
        }
    }
}
