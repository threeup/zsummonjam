using UnityEngine;
namespace zum
{
    public static class ZumAutomatonRaidingBaseState
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
            return za.CanTargetBase();
        }
        public static void OnEnter(object owner)
        {
            ZumAutomaton za = (ZumAutomaton)owner;
            za.SetBaseTarget();
        }

        public static void OnExit(object owner)
        {
        }
        public static void Update(float dt, object owner)
        {
            ZumAutomaton za = (ZumAutomaton)owner;
            za.MoveTowardTarget(0.25f, true);
            if (!za.HasBaseTarget())
            {
                za.AutomatonMachine.Withdraw();
            }
        }
    }
}