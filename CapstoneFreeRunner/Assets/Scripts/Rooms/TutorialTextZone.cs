using UnityEngine;
using System.Collections;

public class TutorialTextZone : MonoBehaviour {

	public HintType hintType = HintType.warp;

	public void OnTriggerEnter2D(Collider2D col) {
		if (col.CompareTag ("Player")) {
			TutorialText.self.DisplayTutorialText (hintType);
		}
	}

	public void OnTriggerExit2D(Collider2D col) {
		if (col.CompareTag ("Player")) {
			TutorialText.self.SquelchTutorialText ();
		}
	}
}
