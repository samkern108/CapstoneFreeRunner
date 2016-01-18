using UnityEngine;
using System.Collections;

public class DrainPlayer : MonoBehaviour {

	public int drainTimer = 1;
	
	void OnTriggerEnter2D(Collider2D coll) {
		if (coll.gameObject.tag == "Player") {
			coll.gameObject.SendMessage ("PlayerDrain", drainTimer);
		}
	}
}
