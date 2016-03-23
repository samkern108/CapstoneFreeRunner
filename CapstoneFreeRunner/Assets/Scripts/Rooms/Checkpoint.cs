using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D col)
	{
		if(col.CompareTag("Player")) {
			PlayerController.state.respawnPosition = new Vector3(transform.position.x, transform.position.y, PlayerController.playerZLayer);
		}
	}
}
