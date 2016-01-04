using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {

	public GameObject hurtScreen;
	public GameObject disableScreen;
	public GameObject gameOverScreen;

	public static UIManager self;

	public void Start()
	{
		self = this;
	}

	public void ClearHurtScreen ()
	{
		hurtScreen.SetActive (false);
	}

	public void DisplayHurtScreen ()
	{
		hurtScreen.SetActive (true);
	}

	public void ClearDisableScreen ()
	{
		disableScreen.SetActive (false);
	}
	
	public void DisplayDisableScreen ()
	{
		disableScreen.SetActive (true);
	}

	public void DisplayGameOverScreen ()
	{
		gameOverScreen.SetActive (true);
	}
}
