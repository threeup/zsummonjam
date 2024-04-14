using UnityEngine;
namespace zum
{
    public static class ZumMineralNacentState
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
            return true;
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
            // check that it is in a real spot?
            mineral.MineralMachine.Advance();

        }
    }
}