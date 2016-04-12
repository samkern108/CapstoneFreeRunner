using UnityEngine;
using System.Collections;

public class Mailbox : MonoBehaviour {

	SpriteRenderer SR;
	public GameObject mail;
    public Sprite mailBoxEmpty;
    public Sprite mailBoxFull;
    public GameObject mailParticles;
    public GameObject plusOneParticles;
	bool delivered = false;

	void Start(){
		SR = gameObject.GetComponent<SpriteRenderer>();
        SR.sprite = mailBoxEmpty;
        //gameObject.GetComponent<ParticleSystem>();
    }

	void OnTriggerEnter2D(Collider2D collider) {
		
		if (collider.CompareTag ("Player") && !delivered) {
			StatsTracker.self.DeliverPaper();
			//SR.color = Color.magenta;
            SR.sprite = mailBoxFull;
            mailParticles.GetComponent<ParticleSystem>().Emit(20);
            plusOneParticles.GetComponent<ParticleSystem>().Emit(1);
            delivered = true;
			mail.SetActive (false);
		}
	}
}
