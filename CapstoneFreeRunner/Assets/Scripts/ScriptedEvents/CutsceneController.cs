using UnityEngine;
using System.Collections;
using System;

public class CutsceneController : MonoBehaviour {

	private bool triggered = false;
	private int dialogueLine = 0;

	private int eventNum = 0;

	public string[] cutscenedialogue;
	public ScriptedEvent[] events;

	public static string changeSpeaker = "<";
	public static string pause = "XX";
	public static string eventTrigger = "%%";
	public static bool cutsceneOcurring = false;

	public virtual void StartCutscene()
	{
		DayManager.self.PauseGame (true);
		triggered = true;
	}

	public virtual void EndCutscene()
	{
		Destroy (this.gameObject);
		DayManager.self.PauseGame (false);
		Pager.self.DisablePager ();
	}

	public void NextCutsceneLine()
	{
		string line = "";
		char[] lineAr;

		if (dialogueLine >= cutscenedialogue.Length) {
			EndCutscene ();
		} else {
			line = cutscenedialogue [dialogueLine];
			lineAr = line.ToCharArray ();
			if (lineAr [0] == 'X' && lineAr [1] == 'X') {
				Pager.self.HideWindow ();
				dialogueLine++;
				Invoke ("NextCutsceneLine",1f);
				return;
			}
			else if(lineAr[0] == '%' && lineAr[1] == '%'){
				dialogueLine++;
				BeginScriptedEvent (eventNum);
				eventNum++;
				return;			
			}
			else if (lineAr [0] == '<') {
				SetSpeaker (line.Substring (1, line.Length - 1));
				Pager.self.pagerText.text = "";

				Invoke("NextCutsceneLine", .5f);
			} 
			else {
				DisplayDialogue (line);
			}
		}
		dialogueLine++;
	}
		
	//Event Callback
	public void EndScriptedEvent()
	{
		NextCutsceneLine ();
	}

	private void DisplayDialogue(string text)
	{
		Pager.self.ScrollPagerWithText (text);
	}

	private void SetSpeaker(string text)
	{
		Pager.self.SetSpeaker (text);
	}

	private void BeginScriptedEvent(int i)
	{
		events[i].TriggerEventWithCallback (this.gameObject);
	}
		
	//Effects of user input
	void Update() {
		if (triggered) {
			if (InputWrapper.GetProgressCutscene()) {
				SkipText ();
			}
			/*if (InputWrapper.GetWarpButton ()) {
				EndCutscene ();
			}*/
		}
	}

	private void SkipText()
	{
		NextCutsceneLine ();
	}
}
