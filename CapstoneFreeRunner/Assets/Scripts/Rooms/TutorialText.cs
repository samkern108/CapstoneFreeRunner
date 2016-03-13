using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum HintType {Warp, Jump, Boost, BoostUp, BoostRight, BoostLeft, Sprint};

public class TutorialText : MonoBehaviour {

	public Image image;
	public Animator anim;
	public static TutorialText self;

	void Start () {
		image = this.GetComponent <Image>();
		anim = this.GetComponent <Animator>();
		image.enabled = false;
		self = this;
	}

	public void DisplayHint(HintType h) {
		if (InputWrapper.isGamepadConnected) {
			DisplayForGamepad (h);
		} else {
			DisplayForKeyboard (h);
		}
	}

	private void DisplayForGamepad(HintType h) {
		Animate (h);
	}

	private void DisplayForKeyboard(HintType h) {
		Debug.Log ("Icons for keyboard NYI");
		Animate (h);
	}

	private void Animate(HintType t)
	{
		image.enabled = true;
		Debug.Log (t + "   " + (int)t);
		anim.SetInteger ("state", (int)t);
	}

	public void SquelchHint() {
		image.enabled = false;
	}
}
