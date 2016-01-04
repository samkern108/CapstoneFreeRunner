using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour {

	private bool hit = false;
	private bool invulnerable = false;

	public void PlayerHit(int damage)
	{
		if (!invulnerable) {
			if (hit || damage >= 2) {
				UIManager.self.DisplayGameOverScreen ();
				PlayerController.PlayerInputEnabled(false);
				SetInvulnerable(true);
			}
			UIManager.self.DisplayHurtScreen ();
			hit = true;
			Invoke ("GetBetter", 1);
		}
	}

	public void PlayerDrain(int timer)
	{
		if (!invulnerable) {
			PlayerController.state.drained = true;
			UIManager.self.DisplayDisableScreen ();
			Invoke ("ChargeUp", timer);
		}
	}

	private void ChargeUp()
	{
		PlayerController.state.drained = false;
		UIManager.self.ClearDisableScreen ();
	}

	private void GetBetter()
	{
		hit = false;
		UIManager.self.ClearHurtScreen ();
	}

	public void SetInvulnerable(bool invuln)
	{
		invulnerable = invuln;
	}

	public void Reset()
	{
		hit = false;
		SetInvulnerable (false);
	}
}
