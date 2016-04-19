using UnityEngine;
using XboxCtrlrInput;
using System.Collections;
using System.Collections.Generic;

public class InputWrapper : MonoBehaviour {

	private static InputType currentInputType = InputType.Keyboard;
	private enum InputType
	{
		Keyboard, Xbox_MAC, Xbox_WIN, Xbox_LINUX, PS
	};

	//true is axis warping, false is button-press warping.
	private static bool warpType = false;

	public static bool isGamepadConnected = false;

	private float joystickCheckTimer = 0f;
	private float joystickCheckTimeout = 2f;

	private static float axisActivationPoint = 0.15f; 

	private static string JUMP = "Jump";
	private static string WARP = "Warp";
	private static string HAXIS = "Horizontal";
	private static string VAXIS = "Vertical";
	private static string SPRINT = "Sprint";
	private static string VWARP = "WarpVertical";
	private static string HWARP = "WarpHorizontal";
	private static string MENU = "Menu";

	private static void AssignKeyboardControls()
	{
		currentInputType = InputType.Keyboard;

		JUMP = "Jump";
		WARP = "Warp";
		HAXIS = "Horizontal";
		VAXIS = "Vertical";
		SPRINT = "Sprint";
		VWARP = "WarpVertical";
		HWARP = "WarpHorizontal";
		MENU = "Menu";
	}

	private static void AssignLinuxXBOXControls()
	{
		currentInputType = InputType.Xbox_LINUX;

		JUMP = "Jump_WIN";
		WARP = "Warp_WIN";
		HAXIS = "Horizontal";
		VAXIS = "Vertical";
		SPRINT = "Sprint_XBOBX";
		VWARP = "WarpVertical";
		HWARP = "WarpHorizontal";
		MENU = "Menu_WIN";
	}

	private static void AssignWinXBOXControls()
	{
		currentInputType = InputType.Xbox_WIN;

		JUMP = "Jump_WIN";
		WARP = "Warp_WIN";
		HAXIS = "Horizontal";
		VAXIS = "Vertical";
		SPRINT = "Sprint_XBOX";
		VWARP = "WarpVertical";
		HWARP = "WarpHorizontal";
		MENU = "Menu_WIN";
	}

	private static void AssignMacXBOXControls()
	{
		currentInputType = InputType.Xbox_MAC;

		JUMP = "Jump_MAC";
		WARP = "Warp_MAC";
		HAXIS = "Horizontal_GAMEPAD";
		VAXIS = "Vertical_GAMEPAD";
		SPRINT = "Sprint_XBOX";
		VWARP = "WarpVertical";
		HWARP = "WarpHorizontal";
		MENU = "Menu_MAC";
	}

	private static void AssignPSControls()
	{
		currentInputType = InputType.PS;

		JUMP = "Jump_PS";
		WARP = "Warp_PS";
		HAXIS = "Horizontal_GAMEPAD";
		VAXIS = "Vertical_GAMEPAD";
		SPRINT = "Sprint_PS";
		VWARP = "WarpVertical";
		HWARP = "WarpHorizontal";
		MENU = "Menu_PS";
	}



	void Start()
	{
		DetermineInputType ();
	}

	//TODO this is stupid 
	void Update()
	{
		if (GetMenuOpen ()) {
			OptionsMenu.self.ToggleOpen();
		}
		/*joystickCheckTimer += Time.deltaTime;
		if (joystickCheckTimer >= joystickCheckTimeout) {
			DetermineInputType ();
		}*/
	}

	public static void DetermineInputType()
	{
		string[] joysticks = Input.GetJoystickNames ();

		if (joysticks.Length > 0) {
			isGamepadConnected = true;
			AssignPSControls ();
		} else {
			AssignKeyboardControls ();
		}
	}

	public static float GetHorizontalAxis()
	{
		return Input.GetAxis (HAXIS);
	}

	public static float GetVerticalAxis()
	{
		return Input.GetAxis (VAXIS);
	}

	public static bool GetJump()
	{
		return Input.GetButtonDown (JUMP);
	}

	public static bool GetAbortJump()
	{
		return Input.GetButtonUp (JUMP);
	}

	public static bool GetSprint()
	{
		return Input.GetAxis (SPRINT) > 0;
	}

	public static float GetWarpVertical()
	{
		return warpType ? GetWarpVerticalAxis () : GetWarpVerticalButton ();
	}

	public static float GetWarpHorizontal()
	{
		return warpType ? GetWarpHorizontalAxis () : GetWarpHorizontalButton ();
	}

	public static bool GetWarpButton()
	{
		return Input.GetButtonDown (WARP);
	}

	private static float GetWarpHorizontalButton()
	{
		if (!Input.GetButtonDown (WARP)) {
			return 0;
		}
		return Input.GetAxis (HAXIS);
	}

	private static float GetWarpVerticalButton()
	{
		if (!Input.GetButtonDown (WARP)) {
			return 0;
		}
		return Input.GetAxis (VAXIS);
	}

	private static float GetWarpHorizontalAxis()
	{
		float axis = Input.GetAxis (HWARP);
		return (axis >= axisActivationPoint || axis <= axisActivationPoint) ? axis : 0;
	}

	private static float GetWarpVerticalAxis()
	{
		float axis = Input.GetAxis (VWARP);
		return (axis >= axisActivationPoint || axis <= -axisActivationPoint) ? axis : 0;
	}

	public static bool GetMenuOpen()
	{
		return Input.GetButtonDown (MENU);
	}

	public static bool Restart()
	{
		if (PlayerHealth.isDead) {
			return Input.GetButtonDown (JUMP);
		}
		return false;
	}
}
