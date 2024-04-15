using UnityEngine;
namespace zum
{
    public static class ZumAutomatonRaidingDoodadState
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
            return za.CanTargetDoodad();
        }
        public static void OnEnter(object owner)
        {
            ZumAutomaton za = (ZumAutomaton)owner;
            za.SetDoodadTarget();
        }

        public static void OnExit(object owner)
        {
            ZumAutomaton za = (ZumAutomaton)owner;
            za.ClearDoodadTarget();
        }
        public static void Update(float dt, object owner)
        {
            ZumAutomaton za = (ZumAutomaton)owner;
            za.MoveTowardTarget(0.25f, true);
            if (!za.HasDoodadTarget())
            {
                za.AutomatonMachine.Withdraw();
            }
        }
    }
}