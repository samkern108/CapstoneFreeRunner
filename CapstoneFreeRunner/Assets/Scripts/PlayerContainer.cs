using UnityEngine;
using System.Collections;

public class PlayerContainer : MonoBehaviour {
	
	void OnTriggerEnter2D(Collider2D collider) {

		if (collider.CompareTag ("Player")) {
			collider.transform.parent = transform;
		}
	}

	void OnTriggerExit2D (Collider2D collider) {
		if (collider.CompareTag ("Player")) {
			if(transform.parent != null) {
				collider.transform.parent = transform.parent;
			}
			else {
				collider.transform.parent = null;
			}
		}
	}
}
