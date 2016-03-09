using UnityEngine;
using System.Collections;

public class TutorialTextZone : MonoBehaviour {

    //public HintType hintType = HintType.warp;
    public GameObject hint;

	public void OnTriggerEnter2D(Collider2D col) {
		if (col.CompareTag ("Player")) {
            //TutorialText.self.DisplayTutorialText (hintType);
            hint.SetActive(true);
		}
	}

	public void OnTriggerExit2D(Collider2D col) {
		if (col.CompareTag ("Player")) {
            //TutorialText.self.SquelchTutorialText ();
            hint.SetActive(false);
		}
	}
}
