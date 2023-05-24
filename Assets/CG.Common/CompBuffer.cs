using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;

namespace AK.CG.Common
{
    /// <summary>
    /// ComputeBuffer wrapper
    /// </summary>
    public class CompBuffer<T> : IDisposable where T : struct
    {
        private readonly ComputeBuffer _buffer;

        private NativeArray<T> _data;

        public ComputeBuffer Buffer => _buffer;

        public CompBuffer(int count, ComputeBufferType type = ComputeBufferType.Default)
        {
            _data = new NativeArray<T>(count, Allocator.Persistent);

            _buffer = new ComputeBuffer(count, UnsafeUtility.SizeOf<T>(), type);
        }

        public CompBuffer(NativeArray<T> data, ComputeBufferType type = ComputeBufferType.Default)
        {
            _data = new NativeArray<T>(data, Allocator.Persistent);

            _buffer = new ComputeBuffer(data.Length, UnsafeUtility.SizeOf<T>(), type);
            _buffer.SetData(_data);
        }

        public bool SetData(int index, T value)
        {
            if (index >= _data.Length) return false;

            _data[index] = value;
            return true;
        }

        public void ApplyData()
        {
            _buffer.SetData(_data);
        }

        public void SetDataAndApply(int index, T value)
        {
            if (SetData(index, value))
            {
                ApplyData();
            }
        }

        public void SetAll(T value)
        {
            var job = new SetAllJob<T>() 
            { 
                Data = _data, 
                Value = value 
            };

            job.Schedule(_data.Length, 64).Complete();
            ApplyData();
        }

        public void SetWriteTarget(int index)
        {
            Graphics.SetRandomWriteTarget(index, _buffer);
        }

        public void Dispose()
        {
            _buffer.Dispose();

            if (_data.IsCreated)
            {
                _data.Dispose();
            }
        }
    }
}