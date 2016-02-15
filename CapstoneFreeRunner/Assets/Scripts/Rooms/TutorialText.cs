using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum HintType {jump, boostJump, warp};

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
		}
	}

	public void SquelchTutorialText() {
		hintText.enabled = false;
	}
}
