using UnityEngine;
namespace zum
{
    public static class ZumMineralAttractingState
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
            ZumMineral mineral = (ZumMineral)owner;
            return mineral.HasPawn();
        }
        public static void OnEnter(object owner)
        {
            ZumMineral mineral = (ZumMineral)owner;
            mineral.GoKinematic(false);
        }

        public static void OnExit(object owner)
        {
        }
        public static void Update(float dt, object owner)
        {
            ZumMineral mineral = (ZumMineral)owner;
            mineral.SetDesiredPositionAsPawn();
            mineral.AdjustVelocityToTarget(mineral.AttractingSpeed);
            if (!mineral.HasPawn())
            {
                mineral.MineralMachine.Withdraw();
            }
            else
            {
                // try to be grabbed
                mineral.MineralMachine.Advance();
            }
        }
    }
}