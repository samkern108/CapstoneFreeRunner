using UnityEngine;
using System.Collections;

public class DamagePlayer : MonoBehaviour {

	//Remember that 2 damage is fatal.
	public int damagePlayer = 1;

	void OnTriggerEnter2D(Collider2D coll) {
		if (coll.gameObject.tag == "Player")
			coll.gameObject.SendMessage("PlayerHit", damagePlayer);
	}
}
