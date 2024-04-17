using UnityEngine;
namespace zum
{
    public static class ZumBossGameOverState
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
            ZumBoss boss = (ZumBoss)owner; return true;
        }
        public static void OnEnter(object owner)
        {
            ZumBoss boss = (ZumBoss)owner;
            Debug.Log("score is blah");
        }

        public static void OnExit(object owner)
        {
            ZumBoss boss = (ZumBoss)owner;
            boss.DestroyNPCs();
            boss.DestroyMinions();

        }
        public static void Update(float dt, object owner)
        {
            ZumBoss boss = (ZumBoss)owner;
            if (boss.TimeInState() > 5.0f)
            {
                boss.DestroyMinerals();
                boss.BossMachine.Advance();
            }
        }
    }
}