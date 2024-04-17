using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif
using zapo;

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace zum
{
#if ENABLE_INPUT_SYSTEM 
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ZumPlayerController : ZumController
    {

        public Cinemachine.CinemachineVirtualCamera cam;

        public ZumThrowCursor ThrowCursor;

#if ENABLE_INPUT_SYSTEM 
        private PlayerInput _playerInput;
#endif

        public override void Possess(ZapoPawn p)
        {
            base.Possess(p);
            if (cam != null)
            {
                cam.Follow = ZapoHelpers.TransformByName(p.transform, "CamRoot");

            }
            if (ThrowCursor != null)
            {
                ThrowCursor.gameObject.transform.SetParent(ZapoHelpers.TransformByName(p.transform, "CamRoot"), false);
                var deltaPos = new Vector3(0.24f, 0.1f, 0.0f);
                ThrowCursor.gameObject.transform.SetLocalPositionAndRotation(deltaPos, Quaternion.identity);
                ThrowCursor.gameObject.transform.localScale = 5f * Vector3.one;
            }
        }

        public override void Dispossess()
        {
            base.Dispossess();
            if (cam != null)
            {
                cam.Follow = this.transform;
            }
            if (ThrowCursor != null)
            {
                ThrowCursor.gameObject.transform.parent.SetParent(this.transform, false);
            }
        }

        public override bool IsNPC
        {
            get
            {
                return false;
            }
        }

        public override bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
                return false;
#endif
            }
        }


        protected override void Start()
        {
            base.Start();
#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#else
            Debug.LogError("Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif
        }

        protected override void Update()
        {
            base.Update();
            if (ThrowCursor != null)
            {
                if (PossessedPawn is ZumPawn zp)
                {
                    ThrowCursor.SetCursorColor(zp.LaunchColor);
                    ThrowCursor.SetStrengthAndForward(zp.HasPrime ? zp.ThrowingAmount : -1, zp.transform.forward);
                }
            }
        }
    }
}