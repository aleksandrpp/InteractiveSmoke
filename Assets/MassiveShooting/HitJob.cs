using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace AK.MassiveShooting
{
    [BurstCompile]
    public struct HitJob : IJob
    {
        public NativeList<Projectile> Projectiles;

        [ReadOnly] public NativeArray<ColliderHit> Results;
        [ReadOnly] public Bounds Bounds;
        [ReadOnly] public float ImpactDuration;
        [ReadOnly] public float ImpactScale;

        [WriteOnly] public NativeList<Impact> Impacts;

        public void Execute()
        {
            var rnd = new Random(1);

            for (int i = 0; i < Projectiles.Length; i++)
            {
                if (!Bounds.Contains(Projectiles[i].Position)) 
                {
                    Projectiles.RemoveAtSwapBack(i);
                    continue;
                }

                if (Results[i].instanceID == 0) continue;

                var p = Projectiles[i];
                Impacts.Add(new Impact()
                {
                    Position = p.Position,
                    Rotation = quaternion.LookRotation(p.LastPosition - p.Position, math.up()),
                    Duration = ImpactDuration,
                    Scale = 1 + rnd.NextFloat() * ImpactScale
                });

                Projectiles.RemoveAtSwapBack(i);
            }
        }
    }
}
