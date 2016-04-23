using UnityEngine;
using System.Collections;

public class SidewaysCutscene : CutsceneController {

	private string boss = "Boss";
	private string brad = "Brad";

	void Start()
	{
		cutscenedialogue = new string[]
		{
			changeSpeaker + brad,
			"...",
			"Huh... sideways.",
			"You know what, uh...",
			"Yeah, that's fine.",
			"Don't worry about it."
		};
	}

	public override void StartCutscene()
	{
		base.StartCutscene ();
		this.NextCutsceneLine ();
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.tag == "Player") {
			this.StartCutscene ();
		}
	}

	public override void EndCutscene()
	{
		CameraController.self.followingPlayer = true;
		base.EndCutscene ();
	}
}
