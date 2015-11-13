using UnityEngine;
using System.Collections;

public class DamagePlayer : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D coll) {
		if (coll.gameObject.tag == "Player")
			coll.gameObject.SendMessage("PlayerHit");
	}
}
