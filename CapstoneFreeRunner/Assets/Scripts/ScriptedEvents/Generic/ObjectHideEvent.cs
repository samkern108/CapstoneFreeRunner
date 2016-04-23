using UnityEngine;
using System.Collections;

public class ObjectHideEvent : ScriptedEvent {

	public GameObject[] objectsToHide;

	public override void TriggerEventWithCallback(GameObject cb)
	{
		base.TriggerEventWithCallback (cb);

		for (int i = 0; i < objectsToHide.Length; i++) {
			objectsToHide [i].SetActive (false);
		}
	}

	public override void EndEvent ()
	{
		ReturnToCaller ();
	}
}
