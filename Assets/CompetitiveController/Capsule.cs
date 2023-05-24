using UnityEngine;

namespace AK.CompetitiveController
{
    public class Capsule
    {
        private readonly Transform _transform;
        private readonly CapsuleCollider _collider;
        private readonly Collider[] _colliders;

        private float _longSide;

        public Capsule(CapsuleCollider capsuleCollider, MoveConfig config)
        {
            _transform = capsuleCollider.transform;
            _collider = capsuleCollider;
            _colliders = new Collider[config.maxSolveColliders];

            ContactOffset = config.ColliderContactOffset;
        }

        private float ContactOffset
        {
            get => _collider.contactOffset;
            set
            {
                _collider.contactOffset = value;
                _longSide = Mathf.Sqrt(value * value * 2f);
            }
        }

        private (Vector3 point0, Vector3 point1) Points()
        {
            var center = _transform.position + _collider.center;
            var distanceToPoints = _collider.height / 2f - _collider.radius;
            return (
                center + Vector3.up * distanceToPoints,
                center - Vector3.up * distanceToPoints);
        }

        public void ResolveCollisions(ref Vector3 position, ref Vector3 velocity, int layerMask)
        {
            var (point0, point1) = Points();

            var count = Physics.OverlapCapsuleNonAlloc(point0, point1, _collider.radius, _colliders, layerMask,
                                                       QueryTriggerInteraction.Ignore);

            for (var i = 0; i < count; i++)
                if (Physics.ComputePenetration(_collider, position, Quaternion.identity, _colliders[i],
                    _colliders[i].transform.position, _colliders[i].transform.rotation, out var direction,
                    out var distance))
                {
                    direction.Normalize();
                    var penetrationVector = direction * distance;
                    position += penetrationVector;
                }
        }

        public (Vector3 normal, float fraction, bool traced) Trace(Vector3 direction, int layerMask)
        {
            var (point0, point1) = Points();
            var maxDistance = direction.magnitude + _longSide;

            if (Physics.CapsuleCast(point0, point1, _collider.radius * (1f - ContactOffset), direction.normalized,
                                    out var hit, maxDistance, layerMask))
            {
                return (hit.normal, hit.distance / maxDistance, true);
            }

            return (Vector3.zero, 1f, false);
        }
    }
}