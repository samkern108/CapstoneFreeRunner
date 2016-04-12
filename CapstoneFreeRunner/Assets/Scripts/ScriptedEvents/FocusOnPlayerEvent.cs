using UnityEngine;
using System.Collections;

public class FocusOnPlayerEvent : ScriptedEvent {

	public override void TriggerEventWithCallback(GameObject cb)
	{
		base.TriggerEventWithCallback (cb);

		CameraController.self.DriftToPlayer ();

		Invoke ("EndEvent",.5f);
	}

	public override void EndEvent()
	{
		base.EndEvent ();
	}
}
