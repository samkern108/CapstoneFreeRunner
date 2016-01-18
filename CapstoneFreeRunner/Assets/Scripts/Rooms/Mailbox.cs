using UnityEngine;
using System.Collections;

public class Mailbox : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D collider) {
		
		if (collider.CompareTag ("Player")) {
			
		}
	}
}
