using System.Collections.Generic;
using UnityEngine;

namespace zum
{
    public enum ZumTeam
    {
        RED,
        BLUE,
        GREEN
    }
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(ZumCombatant))]
    public class ZumDoodad : MonoBehaviour
    {

        public ZumTeam Team;

        public ZapoTimer ScanTargetTimer;

        private Rigidbody _rb;
        private ZumCombatant _zc;
        private ZumMaterial _zm;

        public void Awake()
        {
            _zc = GetComponent<ZumCombatant>();
            _zm = GetComponent<ZumMaterial>();
            _rb = GetComponent<Rigidbody>();
            _rb.isKinematic = true;

        }

        public void AssignTeam(ZumTeam team)
        {
            Team = team;
            _zm.SetTargetColor(0.1f, 0.1f, 0.1f);
            switch (Team)
            {
                default:
                case ZumTeam.RED: _zm.SetRed(0.99f); break;
                case ZumTeam.BLUE: _zm.SetBlue(0.99f); break;
                case ZumTeam.GREEN: _zm.SetGreen(0.99f); break;
            }
        }


        public void Update()
        {
            if (!_zc.HasCollision() && _rb.isKinematic)
            {
                _rb.isKinematic = false;
                _rb.AddExplosionForce(100f, Vector3.zero, 100.0f);
            }
        }
    }
}
