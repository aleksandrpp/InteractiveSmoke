using AK.CompetitiveMan;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = Unity.Mathematics.Random;

namespace AK.MassiveShooting
{
    public class ShootBehaviour : ManBehaviour
    {
        private const int BatchCount = 64;
        private const int CommandPerJob = 32;

        [SerializeField] private Transform _muzzleTransform;
        [SerializeField] private ShootConfig _shootConfig;

        protected NativeList<Projectile> _projectiles;
        protected NativeList<Impact> _impacts;
        protected NativeList<Muzzle> _muzzles;

        private InputAction _shootAction;
        private Random _rnd;

        public override void Start()
        {
            base.Start();

            _muzzles = new NativeList<Muzzle>(Allocator.Persistent);
            _projectiles = new NativeList<Projectile>(Allocator.Persistent);
            _impacts = new NativeList<Impact>(Allocator.Persistent);

            _shootAction = _input.actions["Shoot"];
            _rnd = new Random(1);
        }

        public void Shoot(Vector3 position, Quaternion rotation)
        {
            _muzzles.Add(new Muzzle()
            {
                Position = position,
                Rotation = rotation,
                Duration = _shootConfig.MuzzleDuration,
                Scale = _rnd.NextFloat() * _shootConfig.MuzzleScale
            });

            _projectiles.Add(new Projectile()
            {
                Position = position,
                LastPosition = position,
                Impulse = rotation * Vector3.forward * _shootConfig.Impulse,
                Scale = _rnd.NextFloat(.7f, 1),
                Gravity = _shootConfig.Gravity
            });
        }

        #region GUI

        public override void OnGUI()
        {
            base.OnGUI();

            var stl = new GUIStyle(GUI.skin.label)
            {
                padding = new RectOffset(150, 125, 155, 100),
                fontSize = 24
            };

            var text =
                $"Projectiles: {_projectiles.Length}";

            GUI.Label(new Rect(0, 0, Screen.width, Screen.height), text, stl);
        }

        #endregion

        public override void Update()
        {
            base.Update();

            if (_shootAction.IsPressed())
            {
                Shoot(_muzzleTransform.position, _muzzleTransform.rotation);
            }

            Simulate();

            Render();
        }

        private void Simulate()
        {
            var projectiles = _projectiles.AsArray();
            var overlapCommands = new NativeArray<OverlapSphereCommand>(projectiles.Length, Allocator.TempJob);
            var hitResults = new NativeArray<ColliderHit>(projectiles.Length, Allocator.TempJob);

            var move = new MoveJob()
            {
                Projectiles = projectiles,
                DeltaTime = Time.deltaTime,
            };

            var overlap = new OverlapJob()
            {
                Projectiles = projectiles,
                Commands = overlapCommands
            };

            var hit = new HitJob()
            {
                Projectiles = _projectiles,
                Results = hitResults,
                Impacts = _impacts,
                Bounds = _shootConfig.Bounds,
                ImpactDuration = _shootConfig.ImpactDuration,
                ImpactScale = _shootConfig.ImpactScale
            };

            var impact = new ImpactJob()
            {
                Impacts = _impacts,
                DeltaTime = Time.deltaTime
            };

            var muzzle = new MuzzleJob()
            {
                Muzzles = _muzzles,
                DeltaTime = Time.deltaTime
            };

            var jh = move.Schedule(projectiles.Length, BatchCount);
            jh = overlap.Schedule(projectiles.Length, BatchCount, jh);
            jh = OverlapSphereCommand.ScheduleBatch(overlapCommands, hitResults, CommandPerJob, 1, jh);
            jh = overlapCommands.Dispose(jh);
            jh = hit.Schedule(jh);
            jh = hitResults.Dispose(jh);
            jh = impact.Schedule(jh);
            jh = muzzle.Schedule(jh);
            jh.Complete();
        }

        private void Render()
        {
            using var muzzles = new NativeArray<Matrix4x4>(_muzzles.Length, Allocator.TempJob);
            using var projectiles = new NativeArray<Matrix4x4>(_projectiles.Length, Allocator.TempJob);
            using var impacts = new NativeArray<Matrix4x4>(_impacts.Length, Allocator.TempJob);

            var muzzleMatrices = new MuzzleMatrixJob()
            {
                Muzzles = _muzzles.AsArray(),
                Matrices = muzzles
            };

            var projectileMatrices = new ProjectileMatrixJob()
            {
                Projectiles = _projectiles.AsArray(),
                Matrices = projectiles
            };

            var impactMatrices = new ImpactMatrixJob()
            {
                Impacts = _impacts.AsArray(),
                Matrices = impacts
            };

            var jh = muzzleMatrices.Schedule(_muzzles.Length, BatchCount);
            jh = projectileMatrices.Schedule(_projectiles.Length, BatchCount, jh);
            jh = impactMatrices.Schedule(_impacts.Length, BatchCount, jh);
            jh.Complete();

            Graphics.DrawMeshInstanced(_shootConfig.Muzzle.Mesh, 0, _shootConfig.Muzzle.Material, muzzles.ToArray());
            Graphics.DrawMeshInstanced(_shootConfig.Projectile.Mesh, 0, _shootConfig.Projectile.Material, projectiles.ToArray());
            Graphics.DrawMeshInstanced(_shootConfig.Impact.Mesh, 0, _shootConfig.Impact.Material, impacts.ToArray());
        }

        public virtual void OnDestroy()
        {
            _projectiles.Dispose();
            _impacts.Dispose();
        }
    }
}
