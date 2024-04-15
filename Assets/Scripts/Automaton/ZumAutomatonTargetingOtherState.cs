using UnityEngine;
namespace zum
{
    public static class ZumAutomatonTargetingOtherState
    {
        public static void Bind(ZapoState basicState)
        {
            basicState.CanEnter = CanEnter;
            basicState.OnEnter = OnEnter;
            basicState.OnExit = OnExit;
            basicState.DoUpdate = Update;
        }
        public static bool CanEnter(object owner)
        {
            ZumAutomaton za = (ZumAutomaton)owner;
            return za.CanTargetOther();
        }
        public static void OnEnter(object owner)
        {
            ZumAutomaton za = (ZumAutomaton)owner;
            za.SetOtherTarget();
        }

        public static void OnExit(object owner)
        {
            ZumAutomaton za = (ZumAutomaton)owner;
            za.ClearOtherTarget();
        }
        public static void Update(float dt, object owner)
        {
            ZumAutomaton za = (ZumAutomaton)owner;
            za.MoveTowardTarget(0.5f, true);
            if (!za.HasOtherTarget())
            {
                za.AutomatonMachine.Withdraw();
            }

        }
    }
}