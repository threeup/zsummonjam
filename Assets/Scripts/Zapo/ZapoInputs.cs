using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace zapo
{
	public class ZapoInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool actionOne;
		public bool actionTwo;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if (cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed == true);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

		public void OnPoint(InputValue value)
		{
			ActionOneInput(value.isPressed == true);
		}


		public void OnThrow(InputValue value)
		{
			ActionTwoInput(value.isPressed == true);
		}

		public void OnMenu(InputValue value)
		{
			if (value.isPressed)
			{
#if UNITY_EDITOR
				if (EditorApplication.isPlaying)
				{
					EditorApplication.isPlaying = false;
				}
#else
        Application.Quit();
#endif
			}
		}


		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		}

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newState)
		{
			jump = newState;
		}

		public void SprintInput(bool newState)
		{
			sprint = newState;
		}

		public void ActionOneInput(bool newState)
		{
			actionOne = newState;
			SetCursorState(cursorLocked);
		}

		public void ActionTwoInput(bool newState)
		{
			actionTwo = newState;
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}

}