using UnityEngine;
using System.Collections;

public class OptionsMenu : MonoBehaviour {

	public enum Key
	{
		MusicEnabled,
		SFXEnabled,
		ShowChallengeTimeEnabled
	};

	public static bool musicEnabled = true;
	public static bool sfxEnabled = true;
	public static bool showChallengeTime = true;

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

	private void SetPreference(Key key, bool value)
	{
		PlayerPrefs.SetInt (key.ToString(), (value ? 1 : 0));
	}

	private void SetPreference(Key key, int value)
	{
		PlayerPrefs.SetInt (key.ToString(), value);
	}
}
