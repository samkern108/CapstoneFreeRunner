using UnityEngine;
using System.Collections;

public class Mailbox : MonoBehaviour {

	SpriteRenderer SR;
	bool delivered = false;

	void Start(){
		SR = gameObject.GetComponent<SpriteRenderer>();
	}

	void OnTriggerEnter2D(Collider2D collider) {
		
		if (collider.CompareTag ("Player") && !delivered) {
			StatsTracker.self.DeliverPaper();
			SR.color = Color.blue;
			GetComponent <ParticleSystem> ().Stop ();
			delivered = true;
		}
	}
}
