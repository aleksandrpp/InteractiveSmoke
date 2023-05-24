using Unity.Collections;
using Unity.Jobs;

namespace AK.CG.Common
{
    public struct SetAllJob<T> : IJobParallelFor where T : struct
    {
        [WriteOnly] public NativeArray<T> Data;
        [ReadOnly] public T Value;

        public void Execute(int index)
        {
            Data[index] = Value;
        }
    }
}