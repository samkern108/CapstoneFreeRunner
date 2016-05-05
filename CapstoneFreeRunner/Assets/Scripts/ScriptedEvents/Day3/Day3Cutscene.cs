using UnityEngine;
using System.Collections;

public class Day3Cutscene : CutsceneController {

	private string boss = "Boss";
	private string brad = "Brad";

	void Start()
	{
		cutscenedialogue = new string[]
		{
			changeSpeaker + boss,
			"Well, that's all we have for this demo! Thanks for playing!",
			"Remember to leave us feedback on the index cards!",
			"Press ESC to return to the menu!"
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
