using UnityEngine;
using XboxCtrlrInput;
using System.Collections;

public class InputWrapper : MonoBehaviour {

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

	public static bool GetBoost()
	{
		return Input.GetAxis ("Boost") > 0;
	}
}
