using System;
using UnityEngine;
namespace zum
{
    public static class ZumAutomatonReadyState
    {
        public static float CLOUD = 9.0f;
        public static float WALL = 20.0f;
        public static void Bind(ZapoState basicState)
        {
            basicState.CanEnter = CanEnter;
            basicState.OnEnter = OnEnter;
            basicState.OnExit = OnExit;
            basicState.DoUpdate = Update;
        }
        public static bool CanEnter(object owner)
        {
            ZumAutomaton za = (ZumAutomaton)owner;
            return za.transform.position.y > CLOUD;
        }
        public static void OnEnter(object owner)
        {
            ZumAutomaton za = (ZumAutomaton)owner;
            za.ScanTargetTimer.Launch();
            MoveToSearchPosition(za);
        }

        public static void OnExit(object owner)
        {
        }
        public static void Update(float dt, object owner)
        {
            ZumAutomaton za = (ZumAutomaton)owner;
            if (Math.Abs(za.transform.position.x) > WALL || Math.Abs(za.transform.position.z) > WALL)
            {
                ClampToWall(za);
                MoveToSearchPosition(za);
            }
            za.MoveTowardTarget(0.25f, true);
            if (za.CanTargetOther())
            {
                za.AutomatonMachine.SetState(AutomatonStateType.TARGETING_OTHER);
            }
            else if (za.CanTargetBase())
            {
                za.AutomatonMachine.SetState(AutomatonStateType.RAIDING_BASE);
            }
        }

        private static void ClampToWall(ZumAutomaton za)
        {
            float x = Math.Clamp(za.transform.position.x, -0.999f * WALL, 0.999f * WALL);
            float z = Math.Clamp(za.transform.position.z, -0.999f * WALL, 0.999f * WALL);
            za.transform.position = new Vector3(x, za.transform.position.y, z);
        }

        private static void MoveToSearchPosition(ZumAutomaton za)
        {
            Vector2 dir = UnityEngine.Random.insideUnitCircle;
            za.SetDesiredPosition(new Vector3(dir.x * 20.0f, CLOUD, dir.y * 20.0f));
        }
    }
}