using System.Collections.Generic;
using UnityEngine;
using zapo;

namespace zum
{
    public enum AutomatonStateType
    {
        NACENT,
        ASCEND,
        DESCEND,
        READY,
        TARGETING_OTHER,
        RAIDING_DOODAD,
    }

    [RequireComponent(typeof(Rigidbody))]
    public class ZumAutomaton : MonoBehaviour
    {

        public ZapoStateMach<AutomatonStateType> AutomatonMachine = new ZapoStateMach<AutomatonStateType>();

        public ZapoTimer ScanTargetTimer;
        private ZapoTimer CollisionTimer;
        public float ScanRange = 40.0f;

        public float Speed = 10.0f;

        private ZumCombatant _zc;
        private Rigidbody _rb;
        private Vector3 _targetPos;

        private Transform _doodadTarget;
        private Transform _knownDoodad;
        private Transform _otherTarget;
        private Transform _knownOther;

        public bool CanTargetOther() { return _knownOther != null; }
        public bool HasOtherTarget() { return _otherTarget != null; }
        public bool CanTargetDoodad() { return _knownDoodad != null; }
        public bool HasDoodadTarget() { return _doodadTarget != null; }

        public void Awake()
        {
            CollisionTimer = new ZapoTimer(0.5f, false, false);
            ScanTargetTimer = new ZapoTimer(2.0f, true, false);
            _zc = GetComponent<ZumCombatant>();
            _rb = GetComponent<Rigidbody>();
            AutomatonMachine.AdvanceMap = new Dictionary<AutomatonStateType, AutomatonStateType>{
                {AutomatonStateType.NACENT, AutomatonStateType.ASCEND},
                {AutomatonStateType.DESCEND, AutomatonStateType.ASCEND},
                {AutomatonStateType.ASCEND, AutomatonStateType.READY},
            };
            AutomatonMachine.WithdrawMap = new Dictionary<AutomatonStateType, AutomatonStateType>{
                {AutomatonStateType.TARGETING_OTHER, AutomatonStateType.READY},
                {AutomatonStateType.RAIDING_DOODAD, AutomatonStateType.READY}
            };

            AutomatonMachine.Initialize(this);
            ZumAutomatonNacentState.Bind(AutomatonMachine.GetStateByType(AutomatonStateType.NACENT));
            ZumAutomatonAscendState.Bind(AutomatonMachine.GetStateByType(AutomatonStateType.ASCEND));
            ZumAutomatonDescendState.Bind(AutomatonMachine.GetStateByType(AutomatonStateType.DESCEND));
            ZumAutomatonReadyState.Bind(AutomatonMachine.GetStateByType(AutomatonStateType.READY));
            ZumAutomatonRaidingDoodadState.Bind(AutomatonMachine.GetStateByType(AutomatonStateType.RAIDING_DOODAD));
            ZumAutomatonTargetingOtherState.Bind(AutomatonMachine.GetStateByType(AutomatonStateType.TARGETING_OTHER));
        }

        public void SetDoodadTarget()
        {
            _doodadTarget = _knownDoodad;
        }
        public void ClearDoodadTarget()
        {
            _doodadTarget = null;
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

        public void SetSpeed(float speed)
        {
            Speed = speed;
        }

        public void MoveTowardTarget(float speed, bool lookTowardMove)
        {
            if (_doodadTarget)
            {
                Vector3 pos = new Vector3(_doodadTarget.position.x, ZumConstants.CLOUD, _doodadTarget.position.z);
                SetDesiredPosition(pos);
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
            UpdateScan();
            UpdateCombatReaction();
        }

        public void UpdateCombatReaction()
        {
            if (CollisionTimer.IsCountingDown)
            {
                if (CollisionTimer.TimerTick(Time.deltaTime))
                {
                    _zc.LastPainLocation = null;
                }
            }
            else if (_zc.LastPainLocation is Vector3 loc)
            {
                transform.forward = (transform.position - loc).normalized;
                CollisionTimer.Launch();
                AutomatonMachine.SetState(AutomatonStateType.DESCEND);

            }

            if (!_zc.IsAlive())
            {
                Destroy(gameObject);
            }
        }

        private void UpdateScan()
        {
            if (ScanTargetTimer.TimerTick(Time.deltaTime))
            {
                Vector3 center = transform.position;
                GameObject[] gos = ZumFactory.Instance.GetSphereOverlapsAutomaton(center, ScanRange);
                float bestDotp = 0.0f;
                GameObject best = null;
                foreach (GameObject go in gos)
                {
                    if (go.transform == transform)
                    {
                        continue;
                    }
                    // if (go.transform == _otherTarget || go.transform == _doodadTarget)
                    // {
                    //     best = go;
                    //     bestDotp = 2.0f;
                    //     break;
                    // }
                    Vector3 diff = Vector3.Normalize(go.transform.position - center);
                    float dotp = ZapoMath.DotProduct(go, center, transform.forward);
                    if (dotp > bestDotp)
                    {
                        bestDotp = dotp;
                        best = go;
                    }
                }
                bool isKnownDoodad = best != null && best.GetComponent<ZumDoodad>() != null;
                bool isKnownAutomaton = best != null && best.GetComponent<ZumAutomaton>() != null;
                if (isKnownDoodad)
                {
                    _knownDoodad = best.transform;
                }
                else
                {
                    _knownDoodad = null;
                }
                if (isKnownAutomaton)
                {
                    _knownOther = best.transform;
                }
                else
                {
                    _knownOther = null;
                }
                SetOtherTarget();
            }
        }
    }
}
