using AK.CG.Common;
using AK.MassiveShooting;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace AK.InteractiveSmoke
{
    public class SmokeBehaviour : ShootBehaviour
    {
        private const int BatchCount = 64;

        [SerializeField] private SmokeConfig _smokeConfig;

        private CompShader _cs;
        private CompBuffer<float3> _points;
        private CompBuffer<float3> _colors;
        private CompBuffer<IndirectDrawIndexedArgs> _indirect;

        private void PropagatePoints()
        {
            using var points = new NativeArray<float3>(_smokeConfig.Count, Allocator.TempJob);
            using var colors = new NativeArray<float3>(_smokeConfig.Count, Allocator.TempJob);

            var propagate = new PropagateJob
            {
                Bounds = _smokeConfig.Bounds,
                Points = points,
                Colors = colors,
            }.Schedule(_smokeConfig.Count, BatchCount);

            propagate.Complete();

            _points = new CompBuffer<float3>(points);
            _colors = new CompBuffer<float3>(colors);
        }

        private void IndirectBuffer()
        {
            _indirect = new CompBuffer<IndirectDrawIndexedArgs>(1, ComputeBufferType.IndirectArguments);

            var args = new IndirectDrawIndexedArgs()
            {
                indexCountPerInstance = _smokeConfig.Mesh.GetIndexCount(0),
                instanceCount = (uint)_smokeConfig.Count,
                startIndex = _smokeConfig.Mesh.GetIndexStart(0),
                baseVertexIndex = _smokeConfig.Mesh.GetBaseVertex(0),
                startInstance = 0
            };

            _indirect.SetDataAndApply(0, args);
        }

        public override void Start()
        {
            base.Start();

            IndirectBuffer();

            PropagatePoints();

            _cs = new CompShader(_smokeConfig.Compute, _smokeConfig.Count, _smokeConfig.InputCount);
            _cs.SetBuffer("_Points", _points.Buffer);
            _cs.Shader.SetFloat("_Power", _smokeConfig.InputPower);

            _smokeConfig.Material.SetBuffer("_Points", _points.Buffer);
            _smokeConfig.Material.SetBuffer("_Colors", _colors.Buffer);
        }

        public override void Update()
        {
            base.Update();

            Interact();

            Graphics.DrawMeshInstancedIndirect(
                _smokeConfig.Mesh, 0, _smokeConfig.Material, new Bounds(Vector3.zero, Vector3.one * 1000), _indirect.Buffer);
        }

        private void Interact()
        {
            var count = _projectiles.Length + _impacts.Length;
            if (count == 0) return;

            using var inputs = new CompBuffer<float3>(count);
            for (int i = 0; i < _projectiles.Length; i++)
            {
                inputs.SetData(i, _projectiles[i].Position);
            }
            for (int i = 0; i < _impacts.Length; i++)
            {
                inputs.SetData(i, _impacts[i].Position);
            }
            inputs.ApplyData();

            _cs.SetBuffer("_Inputs", inputs.Buffer);
            _cs.Dispatch();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            _points.Dispose();
            _colors.Dispose();
            _indirect.Dispose();
        }

        #region GUI

        public override void OnGUI()
        {
            base.OnGUI();

            var stl = new GUIStyle(GUI.skin.label)
            {
                padding = new RectOffset(150, 125, 195, 100),
                fontSize = 24
            };

            var text =
                $"Smoke points: {_points.Buffer.count}";

            GUI.Label(new Rect(0, 0, Screen.width, Screen.height), text, stl);
        }

        #endregion
    }
}