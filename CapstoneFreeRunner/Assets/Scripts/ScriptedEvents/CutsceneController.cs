using UnityEngine;
using System.Collections;
using System;

public class CutsceneController : MonoBehaviour {

	public bool disabled;
	private int dialogueLine = 0;

	public string[] cutscenedialogue;
	private string speaker = "Boss";

	void OnTriggerEnter2D(Collider2D col)
	{
		if (PlayerPrefs.GetInt(OptionsMenu.Key.PlayCutscenes.ToString()) == 0 && col.tag == "Player") {
			StartCutscene ();
		}
	}

	private void StartCutscene()
	{
		if(!disabled) {
			DayManager.self.PauseGame (true);
			NextPagerPrompt ();
		}
	}

	public void EndCutscene()
	{
		DayManager.self.PauseGame (false);
		this.disabled = true;
		this.gameObject.SetActive (false);
	}

	public void NextPagerPrompt()
	{
		string line = "";
		char[] lineAr;

		if (dialogueLine >= cutscenedialogue.Length) {
			Pager.self.DisablePager ();
			EndCutscene ();
		} else {
			line = cutscenedialogue [dialogueLine];
			lineAr = line.ToCharArray ();
			if (lineAr [0] == 'X' && lineAr [1] == 'X') {
				Pager.self.HideWindow ();
				dialogueLine++;
				Invoke ("NextPagerPrompt",1f);
				return;
			}
			else if(lineAr[0] == '%' && lineAr[1] == '%'){
				dialogueLine++;
				BeginScriptedEvent (line.Substring(2));
				return;			}
			else if (lineAr [0] == '<') {
				SetSpeaker (line.Substring (1, line.Length - 2));
				Pager.self.pagerText.text = "";

				Invoke("NextPagerPrompt", .5f);
			} else {
				DisplayDialogue (line);
			}
		}
		dialogueLine++;
	}

	private void SetSpeaker(string text)
	{
		Pager.self.SetSpeaker (text);
	}

	private void DisplayDialogue(string text)
	{
		Pager.self.ScrollPagerWithText (text, this);
	}

	private void BeginScriptedEvent(string childname)
	{
		Transform t = transform.Find (childname);
		ScriptedEvent se = t.GetComponent <ScriptedEvent>();
		if (se == null) {
			Debug.Log ("ERROR: NO SCRIPTED EVENT WITH NAME " + childname);
		}
		se.TriggerEventWithCallback (this.gameObject);
	}

	public void EndScriptedEvent()
	{
		NextPagerPrompt ();
	}
}
