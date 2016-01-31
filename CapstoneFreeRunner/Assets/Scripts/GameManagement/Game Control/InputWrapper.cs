using UnityEngine;
using XboxCtrlrInput;
using System.Collections;

public class InputWrapper : MonoBehaviour {

	private static bool isGamepadConnected = false;
	//true is axis warping, false is button-press warping.
	private static bool warpType = false;

	private static float axisActivationPoint = 0.15f; 

	void Start()
	{
		isGamepadConnected = Input.GetJoystickNames ().Length > 0;
		Debug.Log ("Is Gamepad Connected:  " + isGamepadConnected);
	}

	public static float GetHorizontalAxis()
	{
		return Input.GetAxis ("Horizontal");
	}

	public static float GetVerticalAxis()
	{
		return Input.GetAxis ("Vertical");
	}

	public static bool GetJump()
	{
		return Input.GetButtonDown ("Jump");
	}

	public static bool GetAbortJump()
	{
		return Input.GetButtonUp ("Jump");
	}

	public static bool GetSprint()
	{
		return Input.GetAxis ("Sprint") > 0;
	}

	public static float GetWarpVertical()
	{
		return warpType ? GetWarpVerticalAxis () : GetWarpVerticalButton ();
	}

	public static float GetWarpHorizontal()
	{
		return warpType ? GetWarpHorizontalAxis () : GetWarpHorizontalButton ();
	}

	private static float GetWarpHorizontalButton()
	{
		if (!Input.GetButtonDown ("WarpOneShot")) {
			return 0;
		}
		return Input.GetAxis ("Horizontal");
	}

	private static float GetWarpVerticalButton()
	{
		if (!Input.GetButtonDown ("WarpOneShot")) {
			return 0;
		}
		return Input.GetAxis ("Vertical");
	}

	private static float GetWarpHorizontalAxis()
	{
		float axis = Input.GetAxis ("WarpHorizontal");
		return (axis >= axisActivationPoint || axis <= axisActivationPoint) ? axis : 0;
	}

	private static float GetWarpVerticalAxis()
	{
		float axis = Input.GetAxis ("WarpVertical");
		return (axis >= axisActivationPoint || axis <= -axisActivationPoint) ? axis : 0;
	}

	public static bool GetMenuOpen()
	{
		return Input.GetKeyDown (KeyCode.Escape);
	}
}
