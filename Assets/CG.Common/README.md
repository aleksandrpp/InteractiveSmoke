Some wrappers for comfortable prototyping of simple сompute shader use cases.

***
## CompBuffer

This is a generic wrapper for ComputeBuffer that encapsulates a persistently allocated NativeArray of buffer data, that can be easily updated without the need to retrieve the data from the buffer. Also eliminating the need to calculate the data stride.


##### Create from the main thread

    var computeBuffer = new CompBuffer<float>(count);

    for (int i = 0; i < count; ++i)
    {
        computeBuffer.SetData(i, value);
    }

    computeBuffer.ApplyData();

##### Create from a NativeArray filled by jobs

    using var data = new NativeArray<float>(count, Allocator.TempJob);

    var job = new Job
    {
        Data = data,
    };

    job.Schedule(count, 64).Complete();
    var buffer = new CompBuffer<float3>(data);

***
## CompShader

This is a wrapper for ComputeShader that works with single-kernel shader files. It encapsulates the kernelIndex, threadGroupsX, and threadGroupsY, allowing for easy dispatch without passing arguments.

##### Create, set the buffer, and dispatch

    var computeShader = new CompShader(shader, countX, countY);
    computeShader.SetBuffer("_Buffer", computeBuffer);
    computeShader.Dispatch();

`v1.0.0`
<br>

https://github.com/aleksandrpp/CG.Common