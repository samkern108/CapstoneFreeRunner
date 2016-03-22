using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Pager : MonoBehaviour {

	public GameObject pager;
	public Text pagerText;

	public GameObject speaker;
	public Text speakerText;

	public GameObject bossTag;
	public GameObject paulTag;
	public GameObject youTag;

	public static Pager self;
	public CutsceneController cutsceneCallback;

	private string currentText;

	private bool skipTextStage;
	private bool displaying = false;
	private bool hidden = false;

	void Start()
	{
		self = this;
		HideWindow ();
	}

	void Update()
	{
		if(displaying && InputWrapper.GetJump()) {
			SkipText();
		}
		if (displaying && InputWrapper.GetWarpButton()) {
			cutsceneCallback.EndCutscene ();
			DisablePager ();
		}
	}

	private void SkipText()
	{
		if (!skipTextStage)
			DisplayPagerWithText (currentText);
		else
			EndDisplay ();
	}

	public void SetSpeaker(string text)
	{
		speakerText.text = text;
		if (text.ToLower ().Equals ("boss")) {
			bossTag.SetActive (true);
			paulTag.SetActive (false);
			youTag.SetActive (false);
		} else if (text.ToLower ().Equals ("you")) {
			paulTag.SetActive (false);
			bossTag.SetActive (false);
			youTag.SetActive (true);
		}
		else {
			paulTag.SetActive (true);
			bossTag.SetActive (false);
			youTag.SetActive (false);
		}
	}

	public void ScrollPagerWithText(string text, CutsceneController callback)
	{
		if (hidden) {
			ShowWindows ();
		}
		cutsceneCallback = callback;
		skipTextStage = false;
		currentText = text;
		if(!pager.activeSelf) pager.SetActive (true);
		StartCoroutine ("TextScroll", text);
		displaying = true;
	}

	private void DisplayPagerWithText(string text)
	{
		skipTextStage = true;
		pagerText.text = text;
	}

	public void DisablePager()
	{
		if(pager.activeSelf) pager.SetActive (false);
		pagerText.text = "";
		displaying = false;
	}

	public void EndDisplay()
	{
		cutsceneCallback.NextPagerPrompt ();
	}

	public void HideWindow()
	{
		pager.SetActive (false);
		paulTag.SetActive (false);
		bossTag.SetActive (false);
		youTag.SetActive (false);
		speaker.SetActive (false);
		hidden = true;
	}

	public void ShowWindows()
	{
		pager.SetActive (true);
		speaker.SetActive (true);
		hidden = false;
	}

	IEnumerator TextScroll(string text)
	{
		for (int i = 0; i < text.Length + 1; i++) {
			pagerText.text = text.Substring (0, i);
				yield return null;
		}
		skipTextStage = true;
	}
}
