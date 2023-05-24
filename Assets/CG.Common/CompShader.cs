using UnityEngine;

namespace AK.CG.Common
{
    /// <summary>
    /// ComputeShader wrapper
    /// </summary>
    public class CompShader
    {
        private readonly ComputeShader _shader;
        private readonly int _kernelIndex, _threadGroupsX, _threadGroupsY;

        public ComputeShader Shader => _shader;

        public CompShader(ComputeShader shader, int countX, string kernel = "CSMain") : this(shader, countX, 1, kernel) { }

        public CompShader(ComputeShader shader, int countX, int countY, string kernel = "CSMain")
        {
            _shader = shader;
            _kernelIndex = _shader.FindKernel(kernel);
            _shader.GetKernelThreadGroupSizes(_kernelIndex, out uint x, out uint y, out _);

            _threadGroupsX = Mathf.Max(1, Mathf.CeilToInt(countX / (float)x));
            _threadGroupsY = Mathf.Max(1, Mathf.CeilToInt(countY / (float)y));
        }

        public void SetBuffer(string name, ComputeBuffer buffer)
        {
            _shader.SetBuffer(_kernelIndex, name, buffer);
        }

        public void SetTexture(string name, Texture texture)
        {
            _shader.SetTexture(_kernelIndex, name, texture);
        }

        public void Dispatch()
        {
            _shader.Dispatch(_kernelIndex, _threadGroupsX, _threadGroupsY, 1);
        }
    }

}