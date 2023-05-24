using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace AK.MassiveShooting
{
    [BurstCompile]
    public struct MuzzleJob : IJob
    {
        public NativeList<Muzzle> Muzzles;
        public float DeltaTime;

        public void Execute()
        {
            for (int i = 0; i < Muzzles.Length; i++)
            {
                var muzzle = Muzzles[i];
                muzzle.Timer += DeltaTime;

                if (muzzle.Timer >= muzzle.Duration)
                {
                    Muzzles.RemoveAtSwapBack(i);
                }
                else
                {
                    Muzzles[i] = muzzle;
                }
            }
        }
    }
}
