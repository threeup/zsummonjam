using UnityEngine;
namespace zum
{
    public static class ZumMineralGrabbedState
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
            return mineral.HasPawn() && mineral.DistanceToPawnSq() < 0.25f;
        }
        public static void OnEnter(object owner)
        {
        }

        public static void OnExit(object owner)
        {
        }
        public static void Update(float dt, object owner)
        {
            ZumMineral mineral = (ZumMineral)owner;
            mineral.SetDesiredPositionAsPawn();
            if (mineral.DistanceToPawnSq() > 0.1f)
            {
                mineral.AdjustVelocityToTarget(mineral.AttachedSpeed);
            }
            else
            {
                mineral.AdjustVelocityToTarget(0.1f);
            }
            if (!mineral.HasPawn())
            {
                mineral.MineralMachine.Withdraw();
            }
        }
    }
}