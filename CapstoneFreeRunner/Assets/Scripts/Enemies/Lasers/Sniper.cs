using UnityEngine;
using System.Collections;

public class Sniper : MonoBehaviour {

	LineRenderer LR;
	
	GameObject player;

	float sightLength = 22;

	Color trackingColor = Color.white;
	Color fireColor = Color.red;

	bool spotted = false;
	float fireRate = 2;
	float fireAt = 0;
	bool scheduledToFire = false;

	void Start () {
		LR = gameObject.GetComponent<LineRenderer>();
		LR.material = new Material(Shader.Find("Particles/Additive"));
		LR.SetColors(trackingColor, trackingColor);
		player = GameObject.Find("hero");
	}
	
	void Update () {
		RayCast();

		if (spotted && !scheduledToFire){
			fireAt = Time.time + fireRate;
			scheduledToFire = true;
		}else if(!spotted){
			fireAt = 0;
			scheduledToFire = false;
		}
		//Debug.Log ("Time: "+Time.time);
		//Debug.Log (fireAt);
		if (scheduledToFire && fireAt < Time.time){
			Fire();
			scheduledToFire = false;
		}

	}

	void RayCast(){

		Vector3 dir = (player.transform.position - transform.position).normalized;		
		Ray2D ray = new Ray2D(transform.position, dir);
		RaycastHit2D[] hitList = Physics2D.RaycastAll(ray.origin, ray.direction, sightLength);
		
		//draw sight
		LR.SetPosition(0, ray.origin);
		
		foreach(RaycastHit2D hit in hitList) {
			//if player enters laser
			if (hit.collider.gameObject.tag == "Player"){
				spotted = true;
				LR.enabled = true;
				//return;
			}else{
				spotted = false;
				LR.enabled = false;
			}
			
			if(hit == true && !hit.collider.CompareTag("Background")){
				LR.SetPosition(1,hit.point);
				return;
			}
		}
		//if the ray hit nothing
		spotted = false;
		LR.enabled = false;
		LR.SetPosition(1,ray.GetPoint(sightLength));

	}

	void Fire(){
		Debug.Log ("Fire");
		LR.SetColors(fireColor, fireColor);
		player.SendMessage("PlayerHit", 2);
		Invoke("RevertColor", 1);
	}

	void RevertColor(){
		LR.SetColors(trackingColor,trackingColor);
	}
}
