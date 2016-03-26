using UnityEngine;
using System.Collections;

public class Mailbox : MonoBehaviour {

	SpriteRenderer SR;
	public GameObject mail;
	bool delivered = false;

	void Start(){
		SR = gameObject.GetComponent<SpriteRenderer>();
        //gameObject.GetComponent<ParticleSystem>();
    }

	void OnTriggerEnter2D(Collider2D collider) {
		
		if (collider.CompareTag ("Player") && !delivered) {
			StatsTracker.self.DeliverPaper();
			SR.color = Color.magenta;
            GetComponent<ParticleSystem>().Emit(15);
            delivered = true;
			mail.SetActive (false);
		}
	}
}
