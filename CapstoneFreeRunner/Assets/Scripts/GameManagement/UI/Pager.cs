using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Pager : MonoBehaviour {

	public GameObject pager;
	public Text pagerText;
	public static Pager self;
	public CutsceneController cutsceneCallback;
	private bool skipTextStage;
	private string currentText;
	private bool displaying = false;

	void Start()
	{
		self = this;
	}

	void Update()
	{
		if(displaying && InputWrapper.GetJump()) {
			SkipText();
		}
	}

	private void SkipText()
	{
		if (!skipTextStage) {
			DisplayPagerWithText (currentText);
		} else {
			EndDisplay ();
		}
	}

	public void ScrollPagerWithText(string text, CutsceneController callback)
	{
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

	IEnumerator TextScroll(string text)
	{
		for (int i = 0; i < text.Length + 1; i++) {
			pagerText.text = text.Substring (0, i);
				yield return null;
		}
		skipTextStage = true;
	}
}
