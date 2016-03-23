using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class Timer : MonoBehaviour {

	public static Timer self;

	public Text timerText;
	public Text challengeText;

	//this is inelegant, but it works. When you have time, you should rewrite it to use 1 double.
	public static float timerMin = 0;
	public static float timerSec = 0;

	public static float challengeTimeMin = 0;
	public static float challengeTimeSec = 0;
	public static bool paused = false;

	public void Start()
	{
		self = this;
	}

	public static void ZeroTimer()
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
			UpdateTimer ();
		}
	}

	private void UpdateTimer()
	{
		timerSec += Time.deltaTime;

		if (timerSec >= 60) {
			timerSec %= 60;
			timerMin++;
		}

		timerText.text = timerMin + "." + Math.Round(timerSec,1);
	}
}
