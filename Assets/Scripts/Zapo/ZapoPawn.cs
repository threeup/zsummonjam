
using UnityEngine;

namespace zapo
{
    [RequireComponent(typeof(CharacterController))]
    public class ZapoPawn : MonoBehaviour
    {
        [Header("Player")]

        public bool IsJumping = false;
        public bool IsSprinting = false;


        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;
        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;
        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;
        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool IsGrounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        // vars

        protected float _speed;
        protected float _targetRotation = 0.0f;
        protected float _rotationVelocity;
        protected float _verticalVelocity;
        protected float _terminalVelocity = 53.0f;
        // timeout deltatime
        protected float _jumpTimeoutDelta;
        protected float _fallTimeoutDelta;
        private CharacterController _charcontroller;
        protected ZapoController _ctrlr;
        protected bool _hasCtrlr;


        protected virtual void Awake()
        {
            _charcontroller = GetComponent<CharacterController>();
        }

        protected virtual void Start()
        {
            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
        }

        public virtual void PossessedBy(ZapoController ctrlr)
        {
            _hasCtrlr = ctrlr != null;
            _ctrlr = ctrlr;
        }

        protected virtual void JumpAndGravity()
        {

        }

        protected virtual void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            IsGrounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);
        }

        public Vector3 RealCenter()
        {
            return transform.TransformPoint(_charcontroller.center);
        }
        public Vector3 RealForward()
        {
            return transform.forward;
        }

        public float GetHorizontalSpeed()
        {
            return new Vector3(_charcontroller.velocity.x, 0.0f, _charcontroller.velocity.z).magnitude;
        }

        public void ApplyMove(Vector3 dir, float speed, float vertVel)
        {
            _charcontroller.Move(dir * _speed * Time.deltaTime + new Vector3(0.0f, vertVel, 0.0f) * Time.deltaTime);
        }
        protected virtual void Update()
        {
            JumpAndGravity();
            GroundedCheck();
        }
    }
}
