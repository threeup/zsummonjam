using UnityEngine;

namespace zum
{
    [RequireComponent(typeof(Collider))]
    public class ZumCombatant : MonoBehaviour
    {

        private Collider _collider;
        public float AtkVsRed;
        public float AtkVsGreen;
        public float AtkVsBlue;
        public int HP;
        public bool HasCollision() { return _collider.enabled; }
        public Vector3? LastPainLocation = null;
        public bool IsAlive() { return HP > 0; }
        public void Awake()
        {
            _collider = GetComponent<Collider>();
            EnableCollision();
        }
        public void Start()
        {
            HP = 4;
        }

        public void SetCombatStats(float atkVsRed, float atkVsGreen, float atkVsBlue)
        {
            AtkVsRed = atkVsRed;
            AtkVsGreen = atkVsGreen;
            AtkVsBlue = atkVsBlue;
        }

        public void EnableCollision()
        {
            _collider.enabled = true;
        }


        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent<ZumCombatant>(out var otherzc))
            {
                if (otherzc.GetComponent<ZumDoodad>())
                {
                    // detonate on doodad
                    HP = 0;
                    Debug.Log("detonate");
                    return;
                }
                int winCount = 0;
                int loseCount = 0;
                if (AtkVsRed > otherzc.AtkVsRed) { winCount++; } else { loseCount++; }
                if (AtkVsGreen > otherzc.AtkVsGreen) { winCount++; } else { loseCount++; }
                if (AtkVsBlue > otherzc.AtkVsBlue) { winCount++; } else { loseCount++; }

                if (winCount < loseCount)
                {
                    LastPainLocation = collision.body.transform.position;
                    HP -= 1;
                    Debug.Log("ouch");
                }

            }
        }
    }
}