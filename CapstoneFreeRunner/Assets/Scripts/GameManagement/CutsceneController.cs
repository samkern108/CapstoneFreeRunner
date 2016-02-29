using UnityEngine;
using System.Collections;

public class CutsceneController : MonoBehaviour {

	public bool disabled;
	private int dialogueLine = 0;

	public string[] cutscenedialogue;

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
			DisplayDialogue ();
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
		dialogueLine++;
		if (dialogueLine >= cutscenedialogue.Length) {
			Pager.self.DisablePager ();
			EndCutscene ();
		} else {
			DisplayDialogue ();
		}
	}

	private void DisplayDialogue()
	{
		Pager.self.ScrollPagerWithText (cutscenedialogue[dialogueLine], this);
	}
}
