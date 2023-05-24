using UnityEngine;

namespace AK.CompetitiveController
{
    public class Move : IMove
    {
        private readonly MoveConfig _config;
        private readonly Capsule _capsule;

        private Vector3
            _velocity,
            _position;

        private bool
            _grounded,
            _jumpPressed;

        private Quaternion _cameraRotation;
        private Vector2 _moveDelta;

        public Move(MoveConfig config, CapsuleCollider capsuleCollider, Vector3 position)
        {
            _config = config;
            _capsule = new Capsule(capsuleCollider, config);
            _position = position;
        }

        public Vector2 MoveDelta
        {
            get => _moveDelta;
            set => _moveDelta = value.normalized;
        }

        public Quaternion CameraRotation
        {
            get => _cameraRotation;
            set => _cameraRotation = value.normalized;
        }

        public bool JumpPressed
        {
            get => _jumpPressed;
            set => _jumpPressed = value;
        }

        public Vector3 Position => _position;
        public Vector3 Velocity => _velocity;
        public bool Grounded => _grounded;

        public void Update()
        {
            (_, _, _grounded) = _capsule.Trace(Vector3.down * .1f, _config.groundLayers);

            CalculateVelocity();

            _position += _velocity * Time.deltaTime;

            _capsule.ResolveCollisions(ref _position, ref _velocity, _config.groundLayers);
        }

        private void CalculateVelocity()
        {
            var forward = _cameraRotation * Vector3.forward;
            var right = _cameraRotation * Vector3.right;

            var direction = new Vector3(
                forward.x * _moveDelta.y + right.x * _moveDelta.x,
                0,
                forward.z * _moveDelta.y + right.z * _moveDelta.x);

            direction.Normalize();

            _velocity.y -= _config.Gravity * Time.deltaTime;

            if (!_grounded)
            {
                _velocity -= 1 / _config.AirFriction * Time.deltaTime * _velocity;
            }
            else
            {
                _velocity = _config.Speed * direction;

                if (_jumpPressed)
                {
                    _velocity.y += _config.JumpPower;
                }
            }
        }
    }
}