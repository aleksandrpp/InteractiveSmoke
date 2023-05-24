using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace AK.MassiveShooting
{
    [BurstCompile(FloatMode = FloatMode.Deterministic, CompileSynchronously = true)]
    public struct MoveJob : IJobParallelFor
    {
        public NativeArray<Projectile> Projectiles;
        [ReadOnly] public float DeltaTime;

        public void Execute(int index)
        {
            var p = Projectiles[index];

            var position = 
                p.Position + 
                (p.Position - p.LastPosition) + 
                p.Impulse * DeltaTime + 
                DeltaTime * DeltaTime * p.Gravity;
            
            p.Rotation = quaternion.LookRotation(math.normalize(position - p.Position), math.up());
            p.LastPosition = p.Position;
            p.Position = position;
            p.Impulse = float3.zero;

            Projectiles[index] = p;
        }
    }
}
