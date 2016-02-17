using UnityEngine;
using System.Collections;

public class PlayerAudioManager : MonoBehaviour {

	public AudioSource AS_Vaporize;
	public AudioSource AS_SubBass;
	public AudioSource AS_BoostCharge;
	public AudioSource AS_BoostRelease;

	public static PlayerAudioManager self;

	void Start() {
		self = this;
	}

	public void PlayHit() {
		AS_Vaporize.Play ();
		AS_SubBass.Play ();
	}

	public void PlayDisable() {
		AS_SubBass.Play ();
	}

	public void PlayBoostCharge() {
		AS_BoostCharge.Play ();	
	}

	public void PlayBoostRelease() {
		if (AS_BoostCharge.isPlaying) {
			AS_BoostCharge.Stop ();
		}
		AS_BoostRelease.Play ();
	}
}
