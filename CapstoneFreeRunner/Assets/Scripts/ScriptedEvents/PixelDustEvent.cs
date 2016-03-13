using UnityEngine;
using System.Collections;

public class PixelDustEvent : ScriptedEvent {

	ParticleSystem ps;
	SpriteRenderer sr;

	public void Start()
	{
		ps = GetComponent <ParticleSystem>();
		sr = GetComponent<SpriteRenderer> ();
		sr.enabled = false;
		ps.Stop ();
	}

	public override void TriggerEventWithCallback(GameObject cb)
	{
		base.TriggerEventWithCallback (cb);

		ps.Play ();
		Color c = sr.color;
		c.a = 0;
		sr.color = c;
		sr.enabled = true;

		StartCoroutine ("AdjustOpacity");

		Invoke ("EndEvent",2f);
	}

	IEnumerator AdjustOpacity()
	{
		Color c = sr.color;
		do {
			c.a += Time.deltaTime/2;
			sr.color = c;
			yield return null;
		} while(c.a < 1);

		c.a = 1;
		sr.color = c;
	}

	public override void EndEvent()
	{
		ps.Stop ();
		base.EndEvent ();
	}
}
