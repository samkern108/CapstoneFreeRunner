using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {

	public GameObject hurtScreen;
	public GameObject gameOverScreen;

	public static UIManager self;

	public void Start()
	{
		self = this;
	}


	public void RestartGame() {
		Debug.Log ("Note to Sam:  Build Restart Game Function :)");
	}

	public void ClearHurtScreen ()
	{
		hurtScreen.SetActive (false);
	}

	public void DisplayHurtScreen ()
	{
		hurtScreen.SetActive (true);
	}

	public void DisplayGameOverScreen ()
	{
		gameOverScreen.SetActive (true);
	}
}
