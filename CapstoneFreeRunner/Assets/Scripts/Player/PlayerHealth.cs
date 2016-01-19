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
			StartCoroutine ("ShakeCamera");
			PlayerAudioManager.self.PlayHit ();
			Invoke ("GetBetter", 1);
		}
	}

	IEnumerator ShakeCamera() {
		for (float f = 4f; f >= 1f; f -= 0.2f) {
			CameraController.self.ShakeCamera (f);
			yield return null;
		}
	}

	IEnumerator ZoomInCamera() {
		for (float f = 0f; f <= 3f; f += 0.05f) {
			CameraController.self.ZoomInCamera (f);
			yield return null;
		}
		CameraController.self.RestoreSize ();
	}
		
	public void PlayerDrainEnter()
	{
		if (!invulnerable) {
			PlayerController.state.drained = true;
			UIManager.self.DisplayDisableScreen ();
			PlayerAudioManager.self.PlayDisable ();
			StartCoroutine ("ZoomInCamera");
		}
	}

	public void PlayerDrainLeave(int timer)
	{
		Invoke ("ChargeUp", timer);
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
