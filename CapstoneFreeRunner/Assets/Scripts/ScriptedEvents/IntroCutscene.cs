using UnityEngine;
using System.Collections;

public class IntroCutscene : CutsceneController {

	private string boss = "Boss";
	private string brad = "Brad";

	void Start()
	{
		cutscenedialogue = new string[]
		{
			eventTrigger,
			changeSpeaker + boss,
			"...",
			"Did we lose him?",
			changeSpeaker + brad,
			"Attempting to reassemble him now...",
			eventTrigger,
			"... Ah.",
			changeSpeaker + boss,
			"Damn it! That's the third time this month.",
			"If we lose another paper boy, we'll be out of business!",
			"Who is going to deliver for us now?",
			changeSpeaker + brad,
			"Not me.",
			"I... I'm just busy.",
			"Super busy.",
			eventTrigger,
			changeSpeaker + boss,
			"Intern!",
			"Your time has come.",
			"Your country needs you.",
			"As soon as we load in that next room...",
			"Get out there and deliver as many papers as you can!",
			"Ready?",
			"GO!"
		};
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (PlayerPrefs.GetInt(OptionsMenu.Key.PlayCutscenes.ToString()) == 0 && col.tag == "Player") {
			base.StartCutscene ();
		}
	}

	public override void EndCutscene()
	{
		CameraController.self.followingPlayer = true;
		base.EndCutscene ();
	}
}
