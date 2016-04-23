using UnityEngine;
using System.Collections;

public class CameraZoomEvent : ScriptedEvent {

	public float size = 4f;
	public float time = 10f;
	public Transform driftPosition;

	public override void TriggerEventWithCallback(GameObject cb)
	{
		base.TriggerEventWithCallback (cb);

		CameraController.self.ZoomCamera (size, time);
		if (driftPosition != null) {
			CameraController.self.DriftToPosition (driftPosition.position, time);
		}
			
		EndEvent ();
	}

	public override void EndEvent()
	{
		base.ReturnToCaller ();
	}
}
