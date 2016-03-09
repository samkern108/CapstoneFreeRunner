using UnityEngine;
using System.Collections;

public class ScriptedEvent : MonoBehaviour {

	GameObject callback;

	public void TriggerEventWithCallback(GameObject cb)
	{
		callback = cb;
		ReturnToCallback ();
	}

	public void ReturnToCallback()
	{
		callback.SendMessage ("EndScriptedEvent");
		this.gameObject.SetActive (false);
	}
}
