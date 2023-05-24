using AK.CompetitiveController;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AK.CompetitiveMan
{
    [RequireComponent(typeof(CapsuleCollider))]
    public class ManBehaviour : MonoBehaviour
    {
        private readonly int 
            _forwardHash = Animator.StringToHash("Forward"),
            _rightHash = Animator.StringToHash("Right"),
            _groundedHash = Animator.StringToHash("Grounded"),
            _speedHash = Animator.StringToHash("Speed");

        [SerializeField] protected PlayerInput _input;
        [SerializeField] private Transform _transform;
        [SerializeField] private MoveConfig _moveConfig;
        [SerializeField] private ManConfig _manConfig;
        [SerializeField] private CapsuleCollider _capsuleCollider;
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _pointTransform;

        private Transform _camTransform;
        private Vector2 _lookRotation;
        private Vector3 _lastPosition, _velocity, _position;
        private Quaternion _rotation;
        private float _fallTimer, _speed;
        private bool _grounded;
        private IMove _move;

        private InputAction _lookAction, _moveAction, _jumpAction;

        public virtual void Start()
        {
            _capsuleCollider.isTrigger = true;
            _camTransform = Camera.main!.transform;
            _position = _lastPosition = _transform.position;
            _rotation = _transform.rotation;

            _move = new Move(_moveConfig, _capsuleCollider, _position);

            _lookAction = _input.actions["Look"];
            _moveAction = _input.actions["Move"];
            _jumpAction = _input.actions["Jump"];

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public virtual void Update()
        {
            GetActions();

            _move.Update();
            _position = _move.Position;
            _velocity = _move.Velocity;
            _speed = new Vector3(_velocity.x, 0, _velocity.z).magnitude;
            _grounded = _move.Grounded;

            _transform.rotation = Quaternion.AngleAxis(_lookRotation.y, Vector3.up);
            _rotation = _transform.rotation * Quaternion.AngleAxis(_lookRotation.x, Vector3.right);

            Animate();
            Grounding();

            var lookRay = new Ray(
                _position + new Vector3(0, _manConfig.CameraOffset.y, 0),
                _rotation * Vector3.forward);

            _pointTransform.position = lookRay.GetPoint(_manConfig.AimDistance);

            _camTransform.position = lookRay.GetPoint(_manConfig.CameraOffset.z)
                + _rotation * new Vector3(_manConfig.CameraOffset.x, 0, 0);

            _camTransform.rotation = _rotation;

            _transform.position = _position;
        }

        private void GetActions()
        {
            var look = _lookAction.ReadValue<Vector2>() * Time.deltaTime;
            var rotation = _lookRotation + new Vector2(-look.y * _manConfig.YSens, look.x * _manConfig.XSens);
            rotation.x = ClampAngle(rotation.x, _manConfig.MinVerticalAngle, _manConfig.MaxVerticalAngle);
            _lookRotation = rotation;

            _move.MoveDelta = _moveAction.ReadValue<Vector2>();
            _move.CameraRotation = _rotation;
            _move.JumpPressed = _jumpAction.IsPressed();
        }

        private void Animate()
        {
            Vector3 delta = _transform.InverseTransformDirection(_position - _lastPosition);
            _lastPosition = _position;

            delta = delta.magnitude < _manConfig.AnimThreshold * Time.deltaTime
                ? Vector3.zero
                : new Vector3(delta.x, 0, delta.z).normalized;

            _animator.SetFloat(_forwardHash,
                Mathf.MoveTowards(_animator.GetFloat(_forwardHash), delta.z, Time.deltaTime * _manConfig.ApplyAnimSpeed));

            _animator.SetFloat(_rightHash,
                Mathf.MoveTowards(_animator.GetFloat(_rightHash), delta.x, Time.deltaTime * _manConfig.ApplyAnimSpeed));

            _animator.SetFloat(_speedHash, _speed);
        }

        private void Grounding()
        {
            var grounded = true;

            if (!_grounded)
            {
                _fallTimer += Time.deltaTime;
                if (_fallTimer >= _manConfig.FallDelay)
                    grounded = false;
            } else
            {
                _fallTimer = 0;
            }

            _animator.SetBool(_groundedHash, grounded);
        }

        #region GUI

        public virtual void OnGUI()
        {
            var stl = new GUIStyle(GUI.skin.label)
            {
                padding = new RectOffset(150, 125, 30 + 50, 100),
                fontSize = 24
            };

            var text =
                $"Velocity: {_speed:F2} (x {_velocity.x:F2}; y {_velocity.y:F2}; z {_velocity.z:F2})\n" +
                $"Grounding: {_grounded}";

            GUI.Label(new Rect(0, 0, Screen.width, Screen.height), text, stl);
        }

        #endregion

        private static float ClampAngle(float angle, float min, float max)
        {
            angle %= 360;
            if (angle >= -360 && angle <= 360)
            {
                if (angle < -360) angle += 360;
                if (angle > 360) angle -= 360;
            }

            return Mathf.Clamp(angle, min, max);
        }
    }
}