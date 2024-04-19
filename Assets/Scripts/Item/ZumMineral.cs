using System.Collections.Generic;
using UnityEngine;

namespace zum
{
    public enum MineralStateType
    {
        NACENT,
        READY,
        ATTRACTING,
        GRABBED,
        RETURNING,
    }

    [RequireComponent(typeof(Rigidbody))]
    public class ZumMineral : MonoBehaviour
    {
        public bool HasOrigin() { return _originPos != Vector3.zero; }
        private Vector3 _originPos = Vector3.zero;
        private Vector3 _originRot = Vector3.zero;
        private Vector3 _targetPos = Vector3.zero;
        public ZapoStateMach<MineralStateType> MineralMachine = new ZapoStateMach<MineralStateType>();

        public bool HasPawn() { return _pawn != null; }

        public bool MagnetizeToPawn = false;

        public float MaxAttractingSpeed = 5.0f;
        public float AttachedSpeed = 10.0f;

        [SerializeField]
        private ZumPawn _pawn;

        private Rigidbody _rb;
        private ZumMaterial _zm;
        public void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _zm = GetComponent<ZumMaterial>();
            MineralMachine.AdvanceMap = new Dictionary<MineralStateType, MineralStateType>{
                {MineralStateType.NACENT, MineralStateType.READY},
                {MineralStateType.ATTRACTING, MineralStateType.GRABBED},
                {MineralStateType.RETURNING, MineralStateType.READY}
            };
            MineralMachine.WithdrawMap = new Dictionary<MineralStateType, MineralStateType>{
                {MineralStateType.ATTRACTING, MineralStateType.RETURNING},
                {MineralStateType.GRABBED, MineralStateType.RETURNING}
            };

            MineralMachine.Initialize(this);
            ZumMineralNacentState.Bind(MineralMachine.GetStateByType(MineralStateType.NACENT));
            ZumMineralReadyState.Bind(MineralMachine.GetStateByType(MineralStateType.READY));
            ZumMineralAttractingState.Bind(MineralMachine.GetStateByType(MineralStateType.ATTRACTING));
            ZumMineralGrabbedState.Bind(MineralMachine.GetStateByType(MineralStateType.GRABBED));
            ZumMineralReturningState.Bind(MineralMachine.GetStateByType(MineralStateType.RETURNING));
        }

        public void RegisterOrigin()
        {
            _originPos = transform.position;
            _originRot = transform.forward;
        }

        public void ReturnToOrigin()
        {
            transform.position = _originPos;
            transform.forward = _originRot;
        }

        public Color GetTargetColor()
        {
            return _zm.GetTargetColorAsRGB();
        }
        public float DistanceToPawnHandSq()
        {
            if (_pawn == null)
            {
                return 99999f;
            }
            return Vector3.SqrMagnitude(transform.position - _pawn.GetMineralTargetPos(this));
        }

        public float DistanceToOriginSq()
        {
            return Vector3.SqrMagnitude(transform.position - _originPos);
        }

        public float DotProductToPawnHand()
        {
            if (_pawn == null)
            {
                return -1.0f;
            }
            if (!_pawn.IsGrabbing)
            {
                return -1.0f;
            }
            return ZapoMath.DotProduct(gameObject, _pawn.GetMineralTargetPos(this), _pawn.RealForward());
        }


        public void AssociateTo(ZumPawn pawn)
        {
            _pawn = pawn;
        }

        public void RequestPawnGrab()
        {
            if (_pawn != null)
            {
                _pawn.GrabMineral(this);
            }
        }
        public void RequestPawnDisconnect()
        {
            if (_pawn != null)
            {
                _pawn.RemAttractedMineral(this);
            }
        }

        public void SetDesiredPositionAsPawn()
        {
            if (_pawn != null)
            {
                SetDesiredPosition(_pawn.GetMineralTargetPos(this));
            }

        }
        public void SetDesiredPositionAsOrigin()
        {
            SetDesiredPosition(_originPos);
        }

        public void SetDesiredPosition(Vector3 pos)
        {
            _targetPos = pos;
        }

        public void MoveTowardTarget(float speed)
        {
            Vector3 dir = (_targetPos - transform.position).normalized;
            _rb.linearVelocity = dir * speed;
        }

        public void GoKinematic(bool isKinematic)
        {
            _rb.isKinematic = isKinematic;
            _rb.useGravity = isKinematic;
        }

        public void Update()
        {
            MineralMachine.MachineUpdate(Time.deltaTime);
        }

    }
}
