using UnityEngine;
using System.Collections;

public class TutorialTextZone : MonoBehaviour {

    public HintType hintType = HintType.Warp;

	public void OnTriggerEnter2D(Collider2D col) {
		if (col.CompareTag ("Player")) {
            TutorialText.self.DisplayHint (hintType);
		}
	}

	public void OnTriggerExit2D(Collider2D col) {
		if (col.CompareTag ("Player")) {
            TutorialText.self.SquelchHint ();
		}
	}
}
