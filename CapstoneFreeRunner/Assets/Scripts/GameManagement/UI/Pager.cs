using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Pager : MonoBehaviour {

	public GameObject pager;
	public Text pagerText;
	public static Pager self;

	string[] introText = new string[] 
	{
		"... that was too close.", 
		"Get Cam in here!", 
		"Sorry, boss. I think Cam got disintegrated.", 
		"Damn it! Now we have to start over from scratch..."
	};

	void Start()
	{
		self = this;
		//StartCoroutine ("IntroSequence");
	}

	public void DisplayPagerWithTextForTime(string text, float time)
	{
		pager.SetActive (true);
		pagerText.text = text;
		Invoke ("DisablePager", time);
	}

	public void DisablePager()
	{
		pager.SetActive (false);
		pagerText.text = "";
	}

	IEnumerator IntroSequence()
	{
		Debug.Log (introText.Length);
		pager.SetActive (true);
		for (int i = 0; i < introText.Length; i++) {
			for (int j = 0; j < introText[i].Length; j++) {
				pagerText.text = introText[i].Substring (0, j);
				yield return null;
			}
		}
	}
}
