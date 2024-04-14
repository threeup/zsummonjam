using UnityEngine;
namespace zum
{
    public static class ZumBossSetupWorldState
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
            return boss.Minerals.Count == 0;
        }
        public static void OnEnter(object owner)
        {
            ZumBoss boss = (ZumBoss)owner;
            boss.StoreFactoryOrigin(ZumFactory.Instance.transform);
        }

        public static void OnExit(object owner)
        {
            ZumBoss boss = (ZumBoss)owner;
            boss.ResetFactoryPlace(10.0f);
        }

        public static void Update(float dt, object owner)
        {
            ZumBoss boss = (ZumBoss)owner;

            boss.MakeMinerals(7);
            boss.BossMachine.Advance();
        }
    }
}