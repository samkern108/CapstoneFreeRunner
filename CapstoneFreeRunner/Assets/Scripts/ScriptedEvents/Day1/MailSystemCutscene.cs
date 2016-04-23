using UnityEngine;
using System.Collections;

public class MailSystemCutscene : CutsceneController {

	private string boss = "Boss";
	private string brad = "Brad";

	void Start()
	{
		cutscenedialogue = new string[]
		{
			changeSpeaker + boss,
			"Wow.",
			"Just marvel at the efficiency and rationale of the modern mail system.",
			"Aren't you glad to live in this time?"
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
			Debug.Log (base.isActiveAndEnabled);
			this.StartCutscene ();
		}
	}

	public override void EndCutscene()
	{
		CameraController.self.followingPlayer = true;
		base.EndCutscene ();
	}
}
