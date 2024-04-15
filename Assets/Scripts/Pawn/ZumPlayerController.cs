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
        }

        public override void Dispossess()
        {
            base.Dispossess();
            if (cam != null)
            {
                cam.Follow = this.transform;
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
    }
}