using UnityEngine;
using System.Collections;

public class Office : MonoBehaviour {

	public void OnTriggerEnter2D(Collider2D col) {
		if (col.CompareTag ("Player")) {
			if(DayManager.self.PaperRouteFinished()) {
				DayManager.self.LoadNextDay ();
			}
		}
	}
}
