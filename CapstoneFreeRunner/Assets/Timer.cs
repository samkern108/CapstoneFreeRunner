using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class Timer : MonoBehaviour {

	public static Timer self;

	public Text timerText;
	public Text challengeText;

	//this is inelegant, but it works. When you have time, you should rewrite it to use 1 double.
	public float timerMin = 0;
	public float timerSec = 0;

	public float challengeTimeMin = 0;
	public float challengeTimeSec = 0;
	public static bool paused = false;

	private string text;

	public void Start()
	{
		self = this;
	}

	public void ZeroTimer()
	{
		timerMin = 0;
		timerSec = 0;
	}

	public void LoadChallengeTime(float ts, float tm)
	{
		challengeTimeMin = tm;
		challengeTimeSec = ts;
		challengeText.text = tm + ":" +ts;
	}

	public static void PauseTimer(bool pause)
	{
		paused = pause;
	}

	void Update () {
		if (!paused) {
			timerSec += Time.deltaTime;

			if (timerSec >= 60) {
				timerSec %= 60;
				timerMin++;
			}

			//TODO: This line allocates 200B!! What the hell?  Find a workaround.
			text = timerMin + "." + Math.Round (timerSec, 1);
			timerText.text = text;
		}
	}
}
