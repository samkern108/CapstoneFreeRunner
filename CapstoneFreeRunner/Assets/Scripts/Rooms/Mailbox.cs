using UnityEngine;
using System.Collections;

public class Mailbox : MonoBehaviour {

	SpriteRenderer SR;
	bool delivered = false;

	void Start(){
		SR = gameObject.GetComponent<SpriteRenderer>();
        //gameObject.GetComponent<ParticleSystem>();
    }

	void OnTriggerEnter2D(Collider2D collider) {
		
		if (collider.CompareTag ("Player") && !delivered) {
			StatsTracker.self.DeliverPaper();
			SR.color = Color.blue;
            gameObject.GetComponent<ParticleSystem>().Emit(15);
            delivered = true;
		}
	}
}
