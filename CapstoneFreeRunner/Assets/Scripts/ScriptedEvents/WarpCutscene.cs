using UnityEngine;
using System.Collections;

public class WarpCutscene : CutsceneController {

	private string boss = "Boss";
	private string brad = "Brad";

	void Start()
	{
		cutscenedialogue = new string[]
		{
			changeSpeaker + boss,
			"Uh oh... Don't warp into that negative space!",
			"We can bring you back,",
			"But it probably won't feel great."
		};
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.tag == "Player") {
			Debug.Log (base.isActiveAndEnabled);
			base.StartCutscene ();
		}
	}

	public override void EndCutscene()
	{
		CameraController.self.followingPlayer = true;
		base.EndCutscene ();
	}
}
