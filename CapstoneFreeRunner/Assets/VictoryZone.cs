using UnityEngine;
using System.Collections;

public class VictoryZone : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D collider) {
		
		if (collider.CompareTag ("Player")) {
			collider.GetComponent<PlayerHealth>().SetInvulnerable(true);
			UIManager.self.DisplayVictoryScreen();
		}
	}
}
