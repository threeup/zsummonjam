using UnityEngine;
namespace zum
{
    public static class ZumMineralReadyState
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
            if (!mineral.HasOrigin())
            {
                mineral.RegisterOrigin();
            }
            mineral.GoKinematic(true);
        }

        public static void OnExit(object owner)
        {
        }
        public static void Update(float dt, object owner)
        {
            ZumMineral mineral = (ZumMineral)owner;
            if (mineral.HasPawn())
            {
                mineral.MineralMachine.SetState(MineralStateType.ATTRACTING);
            }
        }
    }
}