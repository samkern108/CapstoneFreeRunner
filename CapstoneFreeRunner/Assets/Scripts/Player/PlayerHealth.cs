using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour {

	private bool hit = false;

	public void PlayerHit()
	{
		if (hit) {
			UIManager.self.DisplayGameOverScreen ();
		}
		UIManager.self.DisplayHurtScreen ();
		hit = true;
		Invoke("GetBetter", 1);
	}

	private void GetBetter()
	{
		hit = false;
		UIManager.self.ClearHurtScreen ();
	}
}
