using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using zapo;

namespace zum
{
    public class ZumPawn : ZapoPawn
    {
        public List<ZumMineral> Minerals = new();

        [SerializeField]
        private ZapoTimer AttractTimer;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;



        [Header("Player Pointing")]
        [Tooltip("If the character is pointing or not.")]
        public bool IsPointing = false;
        [Tooltip("Amount of Pointing Time")]
        public float PointingAmount = 0;

        [Header("Player Throwing")]
        [Tooltip("If the character is throwing or not.")]
        public bool IsThrowing = false;
        [Tooltip("Amount of Throwing Time")]
        public float ThrowingAmount = 0;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _animationBlend;


        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;
        private int _animIDPointing;
        private int _animIDThrowing;
        private int _animIDPointingAmount;
        private int _animIDThrowingAmount;


        private Animator _animator;
        private ZumController _zumCtrlr;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;
        private bool _hasZumCtrlr;

        public Transform GrabHandTransform;
        public Transform ThrowHandTransform;


        private void Awake()
        {
            AttractTimer = new ZapoTimer(0.5f, true, true);
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        protected override void Start()
        {
            base.Start();
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

            _hasAnimator = TryGetComponent(out _animator);

            AssignAnimationIDs();

        }

        public override void PossessedBy(ZapoController ctrlr)
        {
            base.PossessedBy(ctrlr);
            if (ctrlr is ZumController zc)
            {
                _zumCtrlr = zc;
            }
            _hasZumCtrlr = _zumCtrlr != null;
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
            _animIDPointing = Animator.StringToHash("IsPointing");
            _animIDThrowing = Animator.StringToHash("IsThrowing");
            _animIDPointingAmount = Animator.StringToHash("PointingAmount");
            _animIDThrowingAmount = Animator.StringToHash("ThrowingAmount");
        }

        protected override void GroundedCheck()
        {
            base.GroundedCheck();
            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, IsGrounded);
            }
        }

        private void PointingCheck()
        {
            if (!_hasZumCtrlr)
            {
                return;
            }
            IsPointing = _zumCtrlr.WantsPoint;
            IsThrowing = _zumCtrlr.WantsThrow && _ctrlr.MoveVec.magnitude < 0.5;
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDPointing, IsPointing);
                _animator.SetBool(_animIDThrowing, IsThrowing);
            }
            if (IsPointing) { PointingAmount += Time.deltaTime; } else { PointingAmount = 0; }
            if (IsThrowing) { ThrowingAmount += Time.deltaTime; } else { ThrowingAmount = 0; }
            if (_hasAnimator)
            {
                _animator.SetInteger(_animIDPointingAmount, (int)Mathf.Ceil(PointingAmount * 5f));
                _animator.SetInteger(_animIDThrowingAmount, (int)Mathf.Ceil(ThrowingAmount * 5f));
            }
        }

        private void CameraRotation()
        {
            if (!_hasCtrlr)
            {
                return;
            }
            Vector2 pawnLook = _ctrlr.LookVec;

            // if there is an input and camera position is not fixed
            if (pawnLook.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = _ctrlr.IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += pawnLook.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += pawnLook.y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ZapoMath.ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ZapoMath.ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }


        protected override void JumpAndGravity()
        {
            if (!_hasCtrlr)
            {
                return;
            }
            IsJumping = IsGrounded && _ctrlr.WantsJump && _jumpTimeoutDelta <= 0.0f;
            if (IsGrounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }


                // Jump
                if (IsJumping)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (IsGrounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = UnityEngine.Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], RealCenter(), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, RealCenter(), FootstepAudioVolume);
            }
        }

        private void Move()
        {
            if (!_hasCtrlr)
            {
                return;
            }
            IsSprinting = IsGrounded && _ctrlr.WantsSprint;
            Vector2 pawnMove = _ctrlr.MoveVec;
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = IsSprinting ? SprintSpeed : MoveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (pawnMove == Vector2.zero || IsThrowing) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = GetHorizontalSpeed();

            float speedOffset = 0.1f;
            float inputMagnitude = _ctrlr.IsAnalogMovement ? pawnMove.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // normalise input direction
            Vector3 inputDirection = new Vector3(pawnMove.x, 0.0f, pawnMove.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (pawnMove != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            ApplyMove(targetDirection, _speed, _verticalVelocity);
            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        public void AddAttractedMineral(ZumMineral min)
        {

            min.AttractedBy(this);
            Minerals.Add(min);
        }

        public void RemAttractedMineral(ZumMineral min)
        {
            min.AttractedBy(null);
            Minerals.Remove(min);
        }

        public void MakeMinion()
        {
            List<Color> colors = new List<Color> { Color.blue, Color.gray, Color.yellow };

            float legPower = 0.0f;
            float torsoPower = 0.0f;
            float armPower = 0.0f;
            foreach (Color c in colors)
            {
                legPower = Math.Max(legPower, c.r);
                torsoPower = Math.Max(torsoPower, c.r);
                armPower = Math.Max(armPower, c.r);
            }
            ZumFactory.Instance.CreateMinion(this, legPower, torsoPower, armPower);
        }

        protected override void Update()
        {
            // why do this often?
            _hasAnimator = TryGetComponent(out _animator);
            //--

            base.Update();
            PointingCheck();
            Move();

            if (IsPointing && AttractTimer.TimerTick(Time.deltaTime))
            {
                Vector3 center = GrabHandTransform.position;
                GameObject[] gos = ZumFactory.Instance.GetSphereOverlapsItem(center, 5.0f);
                foreach (GameObject go in gos)
                {
                    ZumMineral zm = go.GetComponent<ZumMineral>();
                    if (zm == null || zm.HasPawn())
                    {
                        continue;
                    }
                    float dotp = ZapoMath.DotProduct(go, GrabHandTransform, RealForward());
                    if (dotp > 0.5f)
                    {
                        AddAttractedMineral(zm);
                    }
                }

            }
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

    }
}