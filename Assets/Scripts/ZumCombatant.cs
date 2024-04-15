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
        public int HP;
        public bool HasCollision() { return _collider.enabled; }
        public void Awake()
        {
            _collider = GetComponent<Collider>();
        }
        public void Start()
        {
            HP = 4;
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
                    HP -= 1;
                    if (HP <= 0)
                    {
                        _collider.enabled = false;
                    }
                }
                this.transform.forward = -this.transform.forward;
            }
        }
    }
}