using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour {

	public static bool isDead = false;
	private bool hit = false;
	private bool invulnerable = false;
	public GameObject deathParticle;

	public void PlayerHit(int damage)
	{
		if (!invulnerable) {
			if (hit || damage >= 2) {
				isDead = true;
				UIManager.self.DisplayGameOverScreen ();
				PlayerController.PlayerInputEnabled = false;
				deathParticle.SetActive (true);
				Invoke ("DisableDeathParticle", 5f);
				SetInvulnerable(true);
				GetComponent<SpriteRenderer> ().enabled = false;
				return;
			}
			UIManager.self.DisplayHurtScreen ();
			hit = true;
			StartCoroutine ("ShakeCamera");
			PlayerAudioManager.self.PlayHit ();
			Invoke ("GetBetter", 1);
		}
	}

	public void DisableDeathParticle()
	{
		deathParticle.SetActive (false);	
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
		isDead = false;
		hit = false;
		deathParticle.SetActive (false);
		GetComponent<SpriteRenderer> ().enabled = true;
		SetInvulnerable (false);
	}
}
