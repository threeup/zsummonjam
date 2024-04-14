using System.Collections.Generic;
using UnityEngine;

namespace zum
{
    public enum AutomatonStateType
    {
        NACENT,
        READY,
        TARGETING_OTHER,
        TARGETING_BASE,
    }

    [RequireComponent(typeof(Rigidbody))]
    public class ZumAutomaton : MonoBehaviour
    {

        public ZapoStateMach<AutomatonStateType> AutomatonMachine = new ZapoStateMach<AutomatonStateType>();

        private Rigidbody _rb;
        private Vector3 _targetPos;

        public void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            AutomatonMachine.AdvanceMap = new Dictionary<AutomatonStateType, AutomatonStateType>{
                {AutomatonStateType.NACENT, AutomatonStateType.READY}
            };
            AutomatonMachine.WithdrawMap = new Dictionary<AutomatonStateType, AutomatonStateType>{
                {AutomatonStateType.TARGETING_OTHER, AutomatonStateType.READY},
                {AutomatonStateType.TARGETING_BASE, AutomatonStateType.READY}
            };

            AutomatonMachine.Initialize(this);
            ZumAutomatonNacentState.Bind(AutomatonMachine.GetStateByType(AutomatonStateType.NACENT));
            ZumAutomatonReadyState.Bind(AutomatonMachine.GetStateByType(AutomatonStateType.READY));
            ZumAutomatonTargetingBaseState.Bind(AutomatonMachine.GetStateByType(AutomatonStateType.TARGETING_OTHER));
            ZumAutomatonTargetingOtherState.Bind(AutomatonMachine.GetStateByType(AutomatonStateType.TARGETING_BASE));
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
            AutomatonMachine.MachineUpdate(Time.deltaTime);
        }
    }
}
