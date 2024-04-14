using UnityEngine;

namespace zum
{
    [RequireComponent(typeof(Collider))]
    public class ZumCombatant : MonoBehaviour
    {

        private Collider _collider;
        public float WingPower;
        public float TorsoPower;
        public float FirePower;
        public void Start()
        {
            _collider = GetComponent<Collider>();
        }

        public void SetCombatStats(float wingPower, float torsoPower, float firePower)
        {
            WingPower = wingPower;
            TorsoPower = torsoPower;
            FirePower = firePower;
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent<ZumCombatant>(out var otherzc))
            {
                int winCount = 0;
                int loseCount = 0;
                if (WingPower > otherzc.WingPower) { winCount++; } else { loseCount++; }
                if (TorsoPower > otherzc.TorsoPower) { winCount++; } else { loseCount++; }
                if (FirePower > otherzc.FirePower) { winCount++; } else { loseCount++; }

                if (winCount < loseCount)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}