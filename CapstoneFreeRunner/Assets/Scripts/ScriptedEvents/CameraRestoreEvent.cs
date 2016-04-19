using UnityEngine;
using System.Collections;

public class CameraRestoreEvent : ScriptedEvent {

	public float time = 20f;
	public Transform driftPosition;

	public override void TriggerEventWithCallback(GameObject cb)
	{
		base.TriggerEventWithCallback (cb);

		CameraController.self.RestoreSize (time);
		if (driftPosition != null) {
			CameraController.self.DriftToPosition (driftPosition.position, 200);
		}

		Invoke ("EndEvent",2f);
	}

	public override void EndEvent()
	{
		base.ReturnToCaller ();
	}
}
