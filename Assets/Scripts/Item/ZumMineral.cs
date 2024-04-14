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
        private Vector3 _originPos = Vector3.zero;
        private Vector3 _originRot = Vector3.zero;
        private Vector3 _targetPos = Vector3.zero;
        public ZapoStateMach<MineralStateType> MineralMachine = new ZapoStateMach<MineralStateType>();

        public bool HasPawn() { return _pawn != null; }

        public bool MagnetizeToPawn = false;

        public float MaxAttractingSpeed = 5.0f;
        public float AttachedSpeed = 10.0f;

        private ZumPawn _pawn;

        private Rigidbody _rb;
        public void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            MineralMachine.AdvanceMap = new Dictionary<MineralStateType, MineralStateType>{
                {MineralStateType.NACENT, MineralStateType.READY},
                {MineralStateType.ATTRACTING, MineralStateType.GRABBED},
                {MineralStateType.RETURNING, MineralStateType.READY}
            };
            MineralMachine.WithdrawMap = new Dictionary<MineralStateType, MineralStateType>{
                {MineralStateType.ATTRACTING, MineralStateType.READY},
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
        public float DistanceToPawnSq()
        {
            if (_pawn != null)
            {
                return Vector3.SqrMagnitude(transform.position - _pawn.transform.position);
            }
            return 9999f;

        }
        public float DistanceToOriginSq()
        {
            return Vector3.SqrMagnitude(transform.position - _originPos);
        }

        public float DotProductToPawnGrab()
        {
            if (_pawn == null)
            {
                return -1.0f;
            }
            if (!_pawn.IsPointing)
            {
                return -1.0f;
            }
            return ZapoMath.DotProduct(gameObject, _pawn.GrabHandTransform, _pawn.RealForward());
        }


        public void AttractedBy(ZumPawn pawn)
        {
            _pawn = pawn;
        }

        public void RequestPawnDisconnect()
        {
            _pawn.RemAttractedMineral(this);
        }

        public void SetDesiredPositionAsPawn()
        {
            if (_pawn != null)
            {
                SetDesiredPosition(_pawn.transform.position);
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

        public void AdjustVelocityToTarget(float speed)
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

            //debug
            if (MagnetizeToPawn && !HasPawn())
            {
                _pawn = ZumBoss.Instance.GetRandomPawn();
            }
        }

    }
}
