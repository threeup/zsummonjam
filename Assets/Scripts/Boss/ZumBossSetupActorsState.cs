using UnityEngine;
namespace zum
{
    public static class ZumBossSetupActorsState
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
            return boss.HasEnoughMinerals();
        }
        public static void OnEnter(object owner)
        {
            ZumBoss boss = (ZumBoss)owner;
            boss.MakeHumanPlayers();
            boss.MakeNPCPlayers();
            boss.MakeDoodads();
        }

        public static void OnExit(object owner)
        {
        }

        public static void Update(float dt, object owner)
        {
            ZumBoss boss = (ZumBoss)owner;
            boss.MakePawns();
            boss.AssociatePawnsToTeams();
            boss.BossMachine.Advance();
        }
    }
}