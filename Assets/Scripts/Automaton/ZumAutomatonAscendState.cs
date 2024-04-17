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
            Vector3 forward = za.transform.forward;
            float forwardAmount = 8.0f;

            za.SetDesiredPosition(new Vector3(
                center.x + forward.x * forwardAmount,
                ZumConstants.CLOUD + 0.5f,
                center.z + forward.z * forwardAmount
            ));
            za.ScanTargetTimer.Stop();
        }

        public static void OnExit(object owner)
        {
        }
        public static void Update(float dt, object owner)
        {
            ZumAutomaton za = (ZumAutomaton)owner;

            za.MoveTowardTarget(3.0f + za.Speed, false);

            float desiredSizeFactor = Mathf.Max(0.05f, 1.0f - Mathf.Abs(7.0f - za.transform.position.y) / 9.0f);
            float maxSize = 0.6f;
            za.transform.localScale = desiredSizeFactor * maxSize * Vector3.one;

            za.AutomatonMachine.Advance();
        }
    }
}