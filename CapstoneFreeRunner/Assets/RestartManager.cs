using UnityEngine;
using System.Collections;

public class RestartManager : MonoBehaviour {

	public static RestartManager self;

	void Start()
	{
		self = this;
	}

	public void RestartLevel()
	{
		//Sends the "Reset" message to all objects in the scene.
		//Every game object should implement a Reset function to reset its position, health,
		// etc for when the player dies or resets the level.
		BroadcastMessage ("Reset");
	}
}
