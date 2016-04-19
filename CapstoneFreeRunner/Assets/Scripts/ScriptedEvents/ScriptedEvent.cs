using UnityEngine;
using System.Collections;

public class ScriptedEvent : MonoBehaviour {

	GameObject callback;

	public virtual void TriggerEventWithCallback(GameObject cb)
	{
		callback = cb;
	}
		
	public virtual void EndEvent ()
	{
		gameObject.SetActive (false);
	}

	public virtual void ReturnToCaller()
	{
		callback.SendMessage ("EndScriptedEvent");
	}
}
