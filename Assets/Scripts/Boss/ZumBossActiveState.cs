using UnityEngine;
namespace zum
{
    public static class ZumBossActiveState
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
            return boss.PendingPlayers == 0 && boss.HasEnoughPawns();
        }
        public static void OnEnter(object owner)
        {
            Debug.Log("game on!");
        }

        public static void OnExit(object owner)
        {
        }
        public static void Update(float dt, object owner)
        {
            ZumBoss boss = (ZumBoss)owner;
            int pawnCount = 0;
            foreach (ZumController c in boss.Controllers)
            {
                if (c.PossessedPawn)
                {
                    pawnCount += 1;
                }
            }
            if (pawnCount <= 1 && boss.TimeInState() > 5.0f)
            {
                boss.BossMachine.SetState(BossStateType.GAMEOVER);
            }
        }
    }
}