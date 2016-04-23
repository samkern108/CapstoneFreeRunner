using UnityEngine;
using System.Collections;

public class ActivateCutscene : MonoBehaviour {

	public GameObject cutscene;

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag ("Player")) {
			cutscene.SetActive (true);
		}
	}
}
