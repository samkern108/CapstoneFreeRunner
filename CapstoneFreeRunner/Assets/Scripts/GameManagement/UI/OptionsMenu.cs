using UnityEngine;
using System.Collections;

public class OptionsMenu : MonoBehaviour {

	public enum Key
	{
		MusicEnabled,
		SFXEnabled,
		ShowChallengeTimeEnabled,
		PlayCutscenes
	};

	public static bool musicEnabled = true;
	public static bool sfxEnabled = true;
	public static bool showChallengeTime = true;
	public static bool playCutscenes = true;

	public GameObject optionsMenu;
	public static OptionsMenu self;

	public void Start()
	{
		self = this;
	}

	public void ToggleOpen()
	{
		optionsMenu.SetActive (!optionsMenu.activeSelf);
	}

	public void ToggleMusicEnabled()
	{
		musicEnabled = !musicEnabled;
		SetPreference (Key.MusicEnabled, musicEnabled);
	}

	public void ToggleSFXEnabled()
	{
		sfxEnabled = !sfxEnabled;
		SetPreference (Key.SFXEnabled, sfxEnabled);
	}

	public void ToggleShowChallengeTime()
	{
		showChallengeTime = !showChallengeTime;
		SetPreference (Key.ShowChallengeTimeEnabled, showChallengeTime);
	}

	public void TogglePlayCutscenes()
	{
		playCutscenes = !playCutscenes;
		SetPreference (Key.PlayCutscenes, playCutscenes);
	}

	private void SetPreference(Key key, bool value)
	{
		PlayerPrefs.SetInt (key.ToString(), (value ? 1 : 0));
	}

	private void SetPreference(Key key, int value)
	{
		PlayerPrefs.SetInt (key.ToString(), value);
	}
}
