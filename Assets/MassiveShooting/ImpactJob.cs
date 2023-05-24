using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace AK.MassiveShooting
{
    [BurstCompile]
    public struct ImpactJob : IJob
    {
        public NativeList<Impact> Impacts;
        public float DeltaTime;

        public void Execute()
        {
            for (int i = 0; i < Impacts.Length; i++)
            {
                var imp = Impacts[i];
                imp.Timer += DeltaTime;

                if (imp.Timer >= imp.Duration)
                {
                    Impacts.RemoveAtSwapBack(i);
                } else
                {
                    Impacts[i] = imp;
                }
            }
        }
    }
}
