using UnityEngine;
using System.Collections;

public class ActiveArea : MonoBehaviour {

	void Start () {
	
	}

	void OnTriggerExit2D (Collider2D collider) {
		if (collider.CompareTag ("Player")) {
			collider.gameObject.GetComponent <PlayerController>().Reset();
		}
	}
}
