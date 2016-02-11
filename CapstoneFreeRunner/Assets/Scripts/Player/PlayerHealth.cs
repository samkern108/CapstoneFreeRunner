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
				DeathZoom ();
				PlayerController.PlayerInputEnabled = false;
				SetInvulnerable(true);
				return;
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

	private void DeathZoom() {
		CameraController.self.ZoomCamera(.2f, 40, -1);
	}

	private void IonBeamZoom() {
		CameraController.self.ZoomCamera(.5f, 20, 20);
	}
		
	public void PlayerDrainEnter()
	{
		if (!invulnerable) {
			PlayerController.state.drained = true;
			UIManager.self.DisplayDisableScreen ();
			PlayerAudioManager.self.PlayDisable ();
			IonBeamZoom ();
		}
	}

	public void PlayerDrainLeave(int timer)
	{
		Invoke ("ChargeUp", timer);
	}

	public void OnTriggerExit2D(Collider2D other) {
		if(other.gameObject.name == "PlayableArea"){
			PlayerHit(2);
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
