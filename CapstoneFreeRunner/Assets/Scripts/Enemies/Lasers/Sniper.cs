using UnityEngine;
using System.Collections;

public class Sniper : MonoBehaviour {

	LineRenderer LR;
	GameObject player;
	AudioSource AS_Charge;
	AudioSource AS_PowerDown;

	float sightLength = 22;

	Color trackingColor = Color.white;
	Color fireColor = Color.red;

	private bool spotted = false;
	private float fireRate = 3;
	private float fireTimeDiff;
	private float fireAt = 0;
	private bool scheduledToFire = false;
	private bool fired = false;
	private bool dormant = true;

	void Start () {
		LR = GetComponent <LineRenderer>();
		AS_Charge = GetComponents <AudioSource>()[0];
		AS_PowerDown = GetComponents <AudioSource>()[1];
		LR.material = new Material(Shader.Find("Particles/Additive"));
		player = GameObject.Find("hero");
		Reset ();
	}

	public void Reset()
	{
		fired = false;
		scheduledToFire = false;
		fireAt = 0;
		LR.SetColors(trackingColor,trackingColor);
	}

	void Update () {
		if (!fired) {
			RayCast ();

			if (spotted && !scheduledToFire) {
				fireAt = Time.time + fireRate;
				AS_PowerDown.Stop ();
				AS_Charge.Play ();
				dormant = false;
				scheduledToFire = true;
			} else if (!spotted && !dormant) {
				dormant = true;
				AS_Charge.Stop();
				AS_PowerDown.Play ();
				Reset ();
			}

			if (scheduledToFire) {
				fireTimeDiff = fireAt - Time.time;
				ChargeLerp ();

				if (fireTimeDiff <= 0) {
					fired = true;
					player.SendMessage ("PlayerHit", 2);
				}
			}
		}
	}

	void RayCast(){

		Vector2 dir = (PlayerController.PlayerPosition - transform.position).normalized;		
		Ray2D ray = new Ray2D(transform.position, dir);
		RaycastHit2D[] hitList = Physics2D.RaycastAll(ray.origin, ray.direction, sightLength);
		
		//draw sight
		LR.SetPosition(0, ray.origin);
		
		foreach(RaycastHit2D hit in hitList) {
			//if player enters laser
			if (hit.collider.tag == "Player"){
				spotted = true;
				LR.enabled = true;
			} else {
				spotted = false;
			}
			
			if (hit && !hit.collider.isTrigger) {
				LR.SetPosition(1,hit.point);
				return;
			}
		}
		//if the ray hit nothing
		spotted = false;
		LR.enabled = false;
		LR.SetPosition(1,ray.GetPoint(sightLength));

	}

	private void ChargeLerp() {
		float size = (float)(Mathf.Lerp (.2f, .05f, fireTimeDiff/fireRate));
		LR.SetWidth (size, size);

		Color currentColor = Color.Lerp (fireColor, trackingColor, fireTimeDiff/fireRate);
		LR.SetColors (currentColor,currentColor);
		CameraController.self.ShakeCamera ((fireRate - fireTimeDiff).Map(0, fireRate, 0, 1.2f));
	}
}
