using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum HintType {jump, boostJump, warp, boostRun};

public class TutorialText : MonoBehaviour {

	public static TutorialText self;
	public Text hintText;
	private bool textDisplayed;

	void Start () {
		self = this;
		hintText.enabled = false;
	}

	public void DisplayTutorialText(HintType h) {
		hintText.enabled = true;

		if (InputWrapper.isGamepadConnected) {
			DisplayForGamepad (h);
		} else {
			DisplayForKeyboard (h);
		}
	}

	private void DisplayForGamepad(HintType h) {
		switch (h) {
		case HintType.jump:
			hintText.text = "Press A to Jump";
			break;
		case HintType.boostJump:
			hintText.text = "Press A While Jumping To Boost";
			break;
		case HintType.warp:
			hintText.text = "Run at a wall and press B to warp.";
			break;
		case HintType.boostRun:
			hintText.text = "Hold right trigger to sprint.";
			break;
		}
	}

	private void DisplayForKeyboard(HintType h) {
		switch (h) {
		case HintType.jump:
			hintText.text = "Press Space to Jump";
			break;
		case HintType.boostJump:
			hintText.text = "Press Space While Jumping To Boost";
			break;
		case HintType.warp:
			hintText.text = "Run at a wall and press F to warp.";
			break;
		case HintType.boostRun:
			hintText.text = "Hold shift to sprint.";
			break;
		}
	}

	public void SquelchTutorialText() {
		hintText.enabled = false;
	}
}
