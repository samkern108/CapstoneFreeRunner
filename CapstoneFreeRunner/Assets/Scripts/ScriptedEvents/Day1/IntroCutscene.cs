using UnityEngine;
using System.Collections;

public class IntroCutscene : CutsceneController {

	private string boss = "Boss";
	private string brad = "Brad";

	void Start()
	{
		CameraController.self.followingPlayer = false;

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
			"Quit loitering! You've been promoted!",
			"Your time has come!",
			"Your country... no, your COMPANY needs you!",
			"As soon as we load in that next room...",
			"Get out there and deliver as many papers as you can!",
			"(And try not to explode like the last guy...)",
			"Ready?",
			eventTrigger,
			eventTrigger,
			"GO!",
			"...",
			"... Brad, are you going to load in the other half of the wall?",
			changeSpeaker + brad,
			"... Oh, right.",
			eventTrigger,
			"Good to go, Boss.",
			changeSpeaker + boss,
			"Now, GO!"
		};
	}

	public override void StartCutscene()
	{
		base.StartCutscene ();
		this.NextCutsceneLine ();
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (PlayerPrefs.GetInt(OptionsMenu.Key.PlayCutscenes.ToString()) == 0 && col.tag == "Player") {
			this.StartCutscene ();
		}
	}

	public override void EndCutscene()
	{
		CameraController.self.followingPlayer = true;
		base.EndCutscene ();
	}
}
