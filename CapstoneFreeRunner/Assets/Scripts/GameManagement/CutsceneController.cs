using UnityEngine;
using System.Collections;
using System;

public class CutsceneController : MonoBehaviour {

	public bool disabled;
	private int dialogueLine = 0;

	public string[] cutscenedialogue;
	private string speaker = "Boss";

	ScriptedEvent[] scriptedEvents;

	void Start()
	{
		scriptedEvents = this.GetComponentsInChildren <ScriptedEvent> ();
		foreach(ScriptedEvent e in scriptedEvents) {
			e.gameObject.SetActive (false);
		}
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.tag == "Player") {
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

	private void EndCutscene()
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
				BeginScriptedEvent (Int32.Parse(line.Substring(2)));
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

	private void BeginScriptedEvent(int id)
	{
		if (id < scriptedEvents.Length) {
			scriptedEvents [id].gameObject.SetActive (true);
			scriptedEvents [id].TriggerEventWithCallback (this.gameObject);
		}
	}

	public void EndScriptedEvent()
	{
		NextPagerPrompt ();
	}
}
