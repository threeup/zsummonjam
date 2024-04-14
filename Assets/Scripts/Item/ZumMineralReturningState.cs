using UnityEngine;
namespace zum
{
    public static class ZumMineralReturningState
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
            return !mineral.HasPawn();
        }
        public static void OnEnter(object owner)
        {
            ZumMineral mineral = (ZumMineral)owner;
            mineral.SetDesiredPositionAsOrigin();
        }

        public static void OnExit(object owner)
        {
        }
        public static void Update(float dt, object owner)
        {
            ZumMineral mineral = (ZumMineral)owner;
            mineral.MoveTowardTarget(mineral.MaxAttractingSpeed);
            if (mineral.DistanceToOriginSq() < 0.25f)
            {
                mineral.MineralMachine.Advance();
            }
        }
    }
}