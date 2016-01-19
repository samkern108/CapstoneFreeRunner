using UnityEngine;
using System.Collections;

public class PlayerAudioManager : MonoBehaviour {

	public AudioSource AS_Vaporize;
	public AudioSource AS_SubBass;

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
}
