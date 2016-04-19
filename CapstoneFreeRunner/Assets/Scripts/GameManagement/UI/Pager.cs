using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Pager : MonoBehaviour {

	public GameObject pager;
	public Text pagerText;

	public GameObject speaker;
	public Text speakerText;

	public static Pager self;

	private string currentText;

	private bool displaying = false;
	private bool hidden = false;

	void Start()
	{
		self = this;
		HideWindow ();
	}

	public void SetSpeaker(string text)
	{
		speakerText.text = text;
	}

	public void ScrollPagerWithText(string text)
	{
		if (hidden) {
			ShowWindows ();
		}
		currentText = text;
		if(!pager.activeSelf) pager.SetActive (true);
		StartCoroutine ("TextScroll", text);
		displaying = true;
	}

	private void DisplayPagerWithText(string text)
	{
		pagerText.text = text;
	}

	public void DisablePager()
	{
		if(pager.activeSelf) pager.SetActive (false);
		pagerText.text = "";
		displaying = false;
	}

	IEnumerator TextScroll(string text)
	{
		for (int i = 0; i < text.Length + 1; i++) {
			pagerText.text = text.Substring (0, i);
				yield return null;
		}
	}

	public void HideWindow()
	{
		pager.SetActive (false);
		speaker.SetActive (false);
		hidden = true;
	}

	public void ShowWindows()
	{
		pager.SetActive (true);
		speaker.SetActive (true);
		hidden = false;
	}
}
