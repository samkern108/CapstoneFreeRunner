using UnityEngine;
using System.Collections;

public class DamagePlayer : MonoBehaviour {

	//Remember that 2 damage is fatal.
	public int damagePlayer = 1;

	void OnTriggerEnter2D(Collider2D coll) {
		if (coll.gameObject.tag == "Player")
			coll.gameObject.SendMessage("PlayerHit", damagePlayer);
	}

	IEnumerator ShakeCamera() {
		for (float f = 1f; f >= .5f; f -= 0.1f) {
			CameraController.self.ShakeCamera (f);
			yield return null;
		}
	}
}
