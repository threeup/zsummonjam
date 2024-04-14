using UnityEngine;
namespace zum
{
    public static class ZumBossNacentState
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
            ZumBoss boss = (ZumBoss)owner;
            return true;
        }
        public static void OnEnter(object owner)
        {
            ZumBoss boss = (ZumBoss)owner;
        }

        public static void OnExit(object owner)
        {
            ZumBoss boss = (ZumBoss)owner;

        }
        public static void Update(float dt, object owner)
        {
            ZumBoss boss = (ZumBoss)owner;
            boss.BossMachine.Advance();
        }
    }
}