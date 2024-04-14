using System.Collections.Generic;
using UnityEngine;

namespace zum
{
    public enum AutomatonStateType
    {
        NACENT,
        ASCEND,
        READY,
        TARGETING_OTHER,
        RAIDING_BASE,
    }

    [RequireComponent(typeof(Rigidbody))]
    public class ZumAutomaton : MonoBehaviour
    {

        public ZapoStateMach<AutomatonStateType> AutomatonMachine = new ZapoStateMach<AutomatonStateType>();

        public ZapoTimer ScanTargetTimer;
        public float ScanRange = 40.0f;

        private Rigidbody _rb;
        private Vector3 _targetPos;

        private Transform _baseTarget;
        private Transform _knownBase;
        private Transform _otherTarget;
        private Transform _knownOther;

        public bool CanTargetOther() { return _knownOther != null; }
        public bool HasOtherTarget() { return _otherTarget != null; }
        public bool CanTargetBase() { return _knownBase != null; }
        public bool HasBaseTarget() { return _baseTarget != null; }

        public void Awake()
        {
            ScanTargetTimer = new ZapoTimer(2.0f, true, false);
            _rb = GetComponent<Rigidbody>();
            AutomatonMachine.AdvanceMap = new Dictionary<AutomatonStateType, AutomatonStateType>{
                {AutomatonStateType.NACENT, AutomatonStateType.ASCEND},
                {AutomatonStateType.ASCEND, AutomatonStateType.READY},
            };
            AutomatonMachine.WithdrawMap = new Dictionary<AutomatonStateType, AutomatonStateType>{
                {AutomatonStateType.TARGETING_OTHER, AutomatonStateType.READY},
                {AutomatonStateType.RAIDING_BASE, AutomatonStateType.READY}
            };

            AutomatonMachine.Initialize(this);
            ZumAutomatonNacentState.Bind(AutomatonMachine.GetStateByType(AutomatonStateType.NACENT));
            ZumAutomatonAscendState.Bind(AutomatonMachine.GetStateByType(AutomatonStateType.ASCEND));
            ZumAutomatonReadyState.Bind(AutomatonMachine.GetStateByType(AutomatonStateType.READY));
            ZumAutomatonRaidingBaseState.Bind(AutomatonMachine.GetStateByType(AutomatonStateType.RAIDING_BASE));
            ZumAutomatonTargetingOtherState.Bind(AutomatonMachine.GetStateByType(AutomatonStateType.TARGETING_OTHER));
        }

        public void SetBaseTarget()
        {
            _baseTarget = _knownBase;
        }
        public void ClearBaseTarget()
        {
            _baseTarget = null;
        }
        public void SetOtherTarget()
        {
            _otherTarget = _knownOther;
        }
        public void ClearOtherTarget()
        {
            _otherTarget = null;
        }


        public void SetDesiredPosition(Vector3 pos)
        {
            _targetPos = pos;
        }

        public void MoveTowardTarget(float speed, bool lookTowardMove)
        {
            if (_baseTarget)
            {
                SetDesiredPosition(_baseTarget.position);
            }
            else if (_otherTarget)
            {
                SetDesiredPosition(_otherTarget.position);
            }
            Vector3 dir = (_targetPos - transform.position).normalized;
            if (lookTowardMove)
            {
                transform.LookAt(_targetPos);
            }
            _rb.linearVelocity = dir * speed;
        }

        public void GoKinematic(bool isKinematic)
        {
            _rb.isKinematic = isKinematic;
            _rb.useGravity = isKinematic;
        }

        public void Update()
        {
            AutomatonMachine.MachineUpdate(Time.deltaTime);
            if (ScanTargetTimer.TimerTick(Time.deltaTime))
            {
                Vector3 center = transform.position;
                GameObject[] gos = ZumFactory.Instance.GetSphereOverlapsAutomaton(center, ScanRange);
                float bestDotp = 0.0f;
                Transform bestT = null;
                foreach (GameObject go in gos)
                {
                    if (go.transform == transform)
                    {
                        continue;
                    }
                    if (go.transform == _otherTarget)
                    {
                        bestT = go.transform;
                        bestDotp = 2.0f;
                        break;
                    }
                    Vector3 diff = Vector3.Normalize(go.transform.position - center);
                    float dotp = ZapoMath.DotProduct(go, center, transform.forward);
                    if (dotp > bestDotp)
                    {
                        bestDotp = dotp;
                        bestT = go.transform;
                    }
                }
                _knownOther = bestT;
                SetOtherTarget();
            }
        }
    }
}
