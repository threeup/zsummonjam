using UnityEngine;
namespace zum
{
    public static class ZumAutomatonAscendState
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
            ZumAutomaton za = (ZumAutomaton)owner;
            Vector3 center = za.transform.position;

            za.SetDesiredPosition(new Vector3(center.x, 11.0f, center.z));
            za.ScanTargetTimer.Stop();
        }

        public static void OnExit(object owner)
        {
        }
        public static void Update(float dt, object owner)
        {
            ZumAutomaton za = (ZumAutomaton)owner;

            za.MoveTowardTarget(4.0f, false);

            float desiredSizeFactor = Mathf.Max(0.05f, 1.0f - Mathf.Abs(10.0f - za.transform.position.y) / 9.0f);
            float maxSize = 0.6f;
            za.transform.localScale = desiredSizeFactor * maxSize * Vector3.one;

            za.AutomatonMachine.Advance();
        }
    }
}