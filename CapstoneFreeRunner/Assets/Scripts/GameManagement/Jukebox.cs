using UnityEngine;
using System.Collections;

public class Jukebox : MonoBehaviour {

	private static AudioSource bgMusic;
	private static AudioLowPassFilter lpFilter;
	private static AudioHighPassFilter hpFilter;
	private static AudioDistortionFilter distFilter;
	private static AudioReverbFilter reverbFilter;
	private static AudioChorusFilter chorusFilter;
	private static AudioEchoFilter echoFilter;

	public static Jukebox self;

	public void Start()
	{
		bgMusic = GetComponent <AudioSource>();
		lpFilter = GetComponent <AudioLowPassFilter>();
		hpFilter = GetComponent <AudioHighPassFilter>();
		distFilter = GetComponent <AudioDistortionFilter>();
		reverbFilter = GetComponent <AudioReverbFilter>();
		chorusFilter = GetComponent <AudioChorusFilter>();
		echoFilter = GetComponent <AudioEchoFilter>();
		self = this;

		PlayRegular ();
	}

	public void PlayRegular()
	{
		lpFilter.cutoffFrequency = 1000;
	}
		
	public void PlayWarp(float time)
	{
		/*lpFilter.enabled = true;
		reverbFilter.enabled = true;
		lpFilter.cutoffFrequency = 1400;

		Invoke ("PlayRegular", time);*/
	}

	public void PlayOffice()
	{
		//lpFilter.cutoffFrequency = 1200;
	}

	public void PlayBoost()
	{
		/*hpFilter.cutoffFrequency = 0;
		distFilter.distortionLevel = .5f;
		distFilter.enabled = true;
		chorusFilter.enabled = true;*/
	}

	public void PlaySprint()
	{
		/*distFilter.distortionLevel = .6f;
		distFilter.enabled = true;*/
	}
}
