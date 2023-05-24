using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace AK.MassiveShooting
{
    [BurstCompile]
    public struct OverlapJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<Projectile> Projectiles;
        [WriteOnly] public NativeArray<OverlapSphereCommand> Commands;

        public void Execute(int index)
        {
            var p = Projectiles[index];
            Commands[index] = new OverlapSphereCommand(p.Position, /*p.Scale / 2*/.1f, QueryParameters.Default);
        }
    }
}
