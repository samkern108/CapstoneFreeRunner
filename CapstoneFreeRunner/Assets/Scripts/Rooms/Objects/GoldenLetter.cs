using UnityEngine;
using System.Collections;

public class GoldenLetter : MonoBehaviour {

	public static int deliveredLetters;
	public static int totalLetters = -1;

	void Start () {
		/* don't worry about this yet
		 * if (totalLetters == -1) {
			totalLetters = GameObject
		}*/
	}

	void OnTriggerEnter2D(Collider2D col) {
		if (col.gameObject.tag == "Player") {
			deliveredLetters++;
			Destroy (this.gameObject);
		}
	}
}
