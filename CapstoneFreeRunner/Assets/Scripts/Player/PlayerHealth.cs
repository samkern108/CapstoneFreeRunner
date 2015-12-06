﻿using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour {

	private bool hit = false;

	public void PlayerHit(int damage)
	{
		if (hit || damage >= 2) {
			UIManager.self.DisplayGameOverScreen ();
		}
		UIManager.self.DisplayHurtScreen ();
		hit = true;
		Invoke("GetBetter", 1);
	}

	public void PlayerDrain(int timer)
	{
		PlayerController.state.drained = true;
		Invoke("GetBetter", timer);
	}

	private void ChargeUp()
	{
		PlayerController.state.drained = false;
	}

	private void GetBetter()
	{
		hit = false;
		UIManager.self.ClearHurtScreen ();
	}
}
