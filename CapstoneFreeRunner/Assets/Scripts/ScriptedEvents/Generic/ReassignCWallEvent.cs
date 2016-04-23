using UnityEngine;
using System.Collections;

public class ReassignCWallEvent : ScriptedEvent {

	public Wall cwall;
	public Wall reassignment;

	public override void TriggerEventWithCallback(GameObject cb)
	{
		base.TriggerEventWithCallback (cb);
		cwall.companion = reassignment.gameObject;
		reassignment.companion = cwall.gameObject;
	}

	public virtual void EndEvent ()
	{
		gameObject.SetActive (false);
		base.ReturnToCaller ();
	}
}
