using UnityEngine;
using System.Collections;

public class RoomLoadEvent : ScriptedEvent {

	public GameObject room;

	void Start () {
		room.SetActive (false);
	}

	public override void TriggerEventWithCallback(GameObject cb)
	{
		base.TriggerEventWithCallback (cb);

		room.SetActive (true);

		Invoke ("EndEvent",.5f);
	}

	public override void EndEvent()
	{
		base.EndEvent ();
	}
}
