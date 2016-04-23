using UnityEngine;
using System.Collections;

public class ObjectLoadEvent : ScriptedEvent {

	public GameObject[] objectsToLoad;

	void Start () {
		for (int i = 0; i < objectsToLoad.Length; i++) {
			objectsToLoad [i].SetActive (false);
		}
	}

	public override void TriggerEventWithCallback(GameObject cb)
	{
		base.TriggerEventWithCallback (cb);

		for (int i = 0; i < objectsToLoad.Length; i++) {
			objectsToLoad [i].SetActive (true);
		}
	}

	public override void EndEvent ()
	{
		ReturnToCaller ();
	}
}
