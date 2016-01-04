using UnityEngine;
using System.Collections;

public class TimedActivation : MonoBehaviour {

	public bool startsOn = true;
	public float timeOff = .5f;
	public float timeOn = .5f;
	private bool isOn;

	void Start() {
		isOn = startsOn;
	}

	void Update () {
		if (isOn) {
			Invoke ("Toggle", timeOn);
		} else {
			Invoke ("Toggle", timeOff);
		}
	}

	public void Toggle() {
		this.SendMessage ("Activate", isOn);
	}
}
