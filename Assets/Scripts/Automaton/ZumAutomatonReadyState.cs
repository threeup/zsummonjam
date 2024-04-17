using System;
using UnityEngine;
namespace zum
{
    public static class ZumAutomatonReadyState
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
            ZumAutomaton za = (ZumAutomaton)owner;
            return za.transform.position.y > ZumConstants.CLOUD;
        }
        public static void OnEnter(object owner)
        {
            ZumAutomaton za = (ZumAutomaton)owner;
            za.ScanTargetTimer.Launch();
            MoveToForwardPosition(za);
        }

        public static void OnExit(object owner)
        {
        }
        public static void Update(float dt, object owner)
        {
            ZumAutomaton za = (ZumAutomaton)owner;
            if (Math.Abs(za.transform.position.x) > ZumConstants.WALL ||
                Math.Abs(za.transform.position.z) > ZumConstants.WALL)
            {
                ClampToWall(za);
                MoveToSearchPosition(za);
            }
            za.MoveTowardTarget(0.8f * za.Speed, true);
            if (za.CanTargetOther())
            {
                za.AutomatonMachine.SetState(AutomatonStateType.TARGETING_OTHER);
            }
            else if (za.CanTargetDoodad())
            {
                za.AutomatonMachine.SetState(AutomatonStateType.RAIDING_DOODAD);
            }
        }

        private static void ClampToWall(ZumAutomaton za)
        {
            float nearWall = 0.999f * ZumConstants.WALL;
            float x = Math.Clamp(za.transform.position.x, -nearWall, nearWall);
            float z = Math.Clamp(za.transform.position.z, -nearWall, nearWall);
            za.transform.position = new Vector3(x, za.transform.position.y, z);
        }

        private static void MoveToForwardPosition(ZumAutomaton za)
        {
            Vector3 dir = za.transform.forward;
            za.SetDesiredPosition(new Vector3(dir.x * 20.0f, ZumConstants.CLOUD, dir.z * 20.0f));
        }
        private static void MoveToSearchPosition(ZumAutomaton za)
        {
            Vector2 dir = UnityEngine.Random.insideUnitCircle;
            za.SetDesiredPosition(new Vector3(dir.x * 20.0f, ZumConstants.CLOUD, dir.y * 20.0f));
        }
    }
}