using UnityEngine;
using System.Collections;

public class Day3Cutscene : CutsceneController {

	private string boss = "Boss";
	private string brad = "Brad";

	void Start()
	{
		cutscenedialogue = new string[]
		{
			eventTrigger,
			changeSpeaker + boss,
			"You're alive!",
			"(ehem)",
			"Excellent work.",
			"Ready for the next batch?",
			pause,
			"What are you talking about? You just had a break!",
			"Get back out there!"
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
