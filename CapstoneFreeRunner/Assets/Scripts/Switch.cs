using UnityEngine;
using System.Collections;

public class Switch : MonoBehaviour {

	public GameObject activatedObject;

	void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.CompareTag ("Player")) {
			activatedObject.SendMessage("Activate");
		}
	}

}
