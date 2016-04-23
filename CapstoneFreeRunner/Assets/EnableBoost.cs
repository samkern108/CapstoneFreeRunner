using UnityEngine;
using System.Collections;

public class EnableBoost : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag ("Player")) {
			PlayerController.boostEnabled = true;
			Destroy (this.gameObject);
		}
	}
}
