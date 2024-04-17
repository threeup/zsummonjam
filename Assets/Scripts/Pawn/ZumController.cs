using Unity.VisualScripting;
using UnityEngine;
using zapo;

/* works for pc and npc*/

namespace zum
{
    [RequireComponent(typeof(ZapoInputs))]
    public class ZumController : ZapoController
    {
        [Header("Player Pointing")]
        [Tooltip("If the character is pointing or not.")]
        public bool WantsPoint = false;

        [Header("Player Throwing")]
        [Tooltip("If the character is throwing or not.")]
        public bool WantsThrow = false;

        protected ZapoInputs _input;

        protected override void Start()
        {
            _input = GetComponent<ZapoInputs>();
            ZumBoss.Instance.RegisterController(this);
        }

        public void OnDestroy()
        {
            if (ZumBoss.HasInstance())
            {
                ZumBoss.Instance.DeregisterController(this);
            }
        }

        public override bool IsAnalogMovement
        {
            get
            {
                return _input.analogMovement;
            }
        }

        protected override void ReadInputs()
        {
            MoveVec = _input.move;
            LookVec = _input.look;
            WantsSprint = _input.sprint;
            WantsJump = _input.jump;
            WantsPoint = _input.actionOne;
            WantsThrow = _input.actionTwo;
        }

    }
}