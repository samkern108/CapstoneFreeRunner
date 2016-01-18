using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D col)
	{
		if(col.CompareTag("Player")) {
			Debug.Log ("Sup");
			PlayerController.state.startPosition = transform.position;
		}
	}
}
