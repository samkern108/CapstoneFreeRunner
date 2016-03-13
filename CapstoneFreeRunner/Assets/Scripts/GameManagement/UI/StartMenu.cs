using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class StartMenu : MonoBehaviour {

	public struct MusicParameters
	{
		private float cutoffFrequency;
		private float distortionLevel;
		private bool reverbFilter;

		public MusicParameters(float cutoff, float distort, bool reverb)
		{
			cutoffFrequency = cutoff;
			distortionLevel = distort;
			reverbFilter = reverb;
		}
	}

	private AudioSource bgMusic;
	private AudioLowPassFilter lpFilter;
	private AudioDistortionFilter distFilter;
	private AudioReverbFilter reverbFilter;
	private MusicParameters defaultParams;
	private MusicParameters distortParams;

	public void Start()
	{
		bgMusic = GetComponent <AudioSource>();
		lpFilter = GetComponent <AudioLowPassFilter>();
		distFilter = GetComponent <AudioDistortionFilter>();
		reverbFilter = GetComponent <AudioReverbFilter>();
	
		PointerExitButton ();
	}

	public void ExitGame() {
		Application.Quit();
	}

	public void LoadCredits() {
		SceneManager.LoadScene ("Credits");
	}

	public void LoadGame() {
		SceneManager.LoadScene ("Day1");
	}

	/* Eventually, I want to make the music effect fade in and out rather than just "happening"
	 * IEnumerable Swoosh(MusicParameters parameters) {
		float dCO;
		float dDL;
		float dRF;
		while() {
			yield return null;
		}
	}*/

	public void PointerOverButton() {
		lpFilter.cutoffFrequency = 400f;
		distFilter.distortionLevel = .3f;
		reverbFilter.enabled = true;
	}

	public void PointerExitButton() {
		lpFilter.cutoffFrequency = 8000f;
		distFilter.distortionLevel = 0f;
		reverbFilter.enabled = false;
	}
}
