using UnityEngine;
using System.Collections;

public class WarpCutscene : CutsceneController {

	private string boss = "Boss";
	private string brad = "Brad";

	void Start()
	{
		cutscenedialogue = new string[]
		{
			changeSpeaker + brad,
			"Oh...",
			"That doesn't look right.",
			"Okay, I messed this room up.",
			"Just avoid the... spacey-timey holes. No disintegrating, a'ight?",
			"And don't rat me out!"
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
