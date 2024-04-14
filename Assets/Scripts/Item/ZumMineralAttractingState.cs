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
            // dist is huge if pawn not grabbing
            float dist = mineral.DistanceToPawnHandSq();
            // dotp is negative if pawn not grabbing
            float dotp = mineral.DotProductToPawnHand();
            if (dotp < 0.5f && dist > 1.0f)
            {
                mineral.RequestPawnDisconnect();
            }
            if (mineral.HasPawn())
            {
                mineral.AdjustVelocityToTarget(mineral.MaxAttractingSpeed * dotp);
                // try to be grabbed
                mineral.MineralMachine.Advance();
            }
            else
            {
                // no pawn
                mineral.MineralMachine.Withdraw();
            }
        }
    }
}