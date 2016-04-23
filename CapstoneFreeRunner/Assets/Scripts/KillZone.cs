using UnityEngine;
using System.Collections;

public class KillZone : MonoBehaviour {

	public void OnTriggerEnter2D(Collider2D col)
	{
		if (col.gameObject.CompareTag ("Player")) {
			PlayerHealth.self.PlayerHit (2);
		}
	}
}
