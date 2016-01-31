using UnityEngine;
using System.Collections;

public class IonLaser : MonoBehaviour {

	AudioSource AS;
	LineRenderer LR;
	public ParticleSystem PS;

	Transform DirectionHandle;
	bool active;
	float maxLazerLength = 1000;

	private bool disabled = false;
	private float dist;
	private float maxDist = 40;
	private bool playerInLaser = false;

	Color startColor = Color.cyan;
	Color endColor = Color.cyan;

	void Start () {
		AS = GetComponent<AudioSource> ();
		LR = GetComponent<LineRenderer>();
		DirectionHandle = FindDirectionHandle(this.transform, "DirectionHandle");
		active = true;
		LR.material = new Material(Shader.Find("Particles/Additive"));
		LR.SetColors(startColor, endColor);
	}

	void Update () {
		dist = Vector2.Distance (transform.position, PlayerController.PlayerPosition);
		if (dist < maxDist) {
			if (disabled) {
				AS.enabled = true;
				LR.enabled = true;
				PS.Play ();
				disabled = false;
			}
			Raycast ();
			AudioUpdate ();
		} else {
			if (!disabled) {
				AS.enabled = false;
				LR.enabled = false;
				PS.Stop ();
				disabled = true;
			}
		}
	}

	private void AudioUpdate()
	{
		AS.volume = dist.Map (0, 30, 1f, 0);
	}

	void Raycast() {
		if (active){
			Vector3 dir = (DirectionHandle.position - transform.position).normalized;		
			Ray2D ray = new Ray2D(transform.position, dir);
			RaycastHit2D[] hitList = Physics2D.RaycastAll(ray.origin, ray.direction, maxLazerLength);

			//draw lazer
			LR.SetPosition(0, ray.origin);

			foreach(RaycastHit2D hit in hitList) {
				//if player enters lazer
				if (hit.collider.gameObject.tag == "Player") {
					if (!playerInLaser) {
						playerInLaser = true;
						hit.collider.gameObject.SendMessage ("PlayerDrainEnter");
					}
					return;
				} else {
					if(playerInLaser){
						playerInLaser = false;
						PlayerController.Player.SendMessage ("PlayerDrainLeave",2);
					}
				}

				if(hit == true && !hit.collider.CompareTag("Background")){
					if (!PS.isPlaying)
						PS.Play ();
					PS.transform.position = hit.point;
					LR.SetPosition(1,hit.point);
					return;
				}
			}
			//if the ray hit nothing
			if(PS.isPlaying)
				PS.Stop();
			LR.SetPosition(1,ray.GetPoint(maxLazerLength));
		}
	}

	Transform FindDirectionHandle(Transform parent, string name){
		Transform result = parent.Find(name);
		if(result != null){
			return result;
		}else{
			foreach(Transform child in parent)
			{
				result = FindDirectionHandle(child,name);
				if (result != null){
					return result;
				}
			}
			return null;
		}
	}
}
