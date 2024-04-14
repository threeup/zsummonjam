using UnityEngine;
using zapo;

namespace zum
{
    public class UICanvasControllerInput : MonoBehaviour
    {

        [Header("Output")]
        public ZapoInputs zapoInputs;

        public void VirtualMoveInput(Vector2 virtualMoveDirection)
        {
            zapoInputs.MoveInput(virtualMoveDirection);
        }

        public void VirtualLookInput(Vector2 virtualLookDirection)
        {
            zapoInputs.LookInput(virtualLookDirection);
        }

        public void VirtualJumpInput(bool virtualState)
        {
            zapoInputs.JumpInput(virtualState);
        }

        public void VirtualSprintInput(bool virtualState)
        {
            zapoInputs.SprintInput(virtualState);
        }

        public void VirtualActionOneInput(bool virtualState)
        {
            zapoInputs.ActionOneInput(virtualState);
        }
        public void VirtualActionTwoInput(bool virtualState)
        {
            zapoInputs.ActionTwoInput(virtualState);
        }

    }

}
