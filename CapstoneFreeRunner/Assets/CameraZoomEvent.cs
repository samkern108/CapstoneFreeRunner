using UnityEngine;
using System.Collections;

public class CameraZoomEvent : ScriptedEvent {

	void Start()
	{
		CameraController.self.SetCameraSize (4.5f);
	}

	public override void TriggerEventWithCallback(GameObject cb)
	{
		base.TriggerEventWithCallback (cb);

		CameraController.self.RestoreSize (20f);
		CameraController.self.DriftToPlayer ();


		Invoke ("EndEvent",2f);
	}

	public override void EndEvent()
	{
		base.EndEvent ();
	}
}
