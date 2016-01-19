using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour {

	AudioSource AS;
	LineRenderer LR;
	public ParticleSystem PS;

	Transform DirectionHandle;
	bool active;
	float maxLazerLength = 1000;
	Color startColor = Color.red;
	Color endColor = Color.red;
	private bool disabled = false;
	private float dist;
	private float maxDist = 40;
	
	void Start () {
		AS = GetComponent<AudioSource> ();
		LR = GetComponent<LineRenderer>();
		DirectionHandle = FindDirectionHandle(this.transform, "DirectionHandle");
		active = true;
		LR.material = new Material(Shader.Find("Particles/Additive"));
		LR.SetColors(startColor, endColor);
	}

	void Update () {
		dist = Vector2.Distance (transform.position, PlayerController.state.position);
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
			
			//draw laser
			LR.SetPosition(0, ray.origin);

			foreach(RaycastHit2D hit in hitList) {
				//if player enters laser
				if (hit.collider.gameObject.tag == "Player"){
					hit.collider.gameObject.SendMessage("PlayerHit", 2);
					return;
				}

				if(hit == true && !hit.collider.CompareTag("Background")){
					PS.transform.position = new Vector3 (hit.point.x, hit.point.y, -1);
					if (!PS.isPlaying)
						PS.Play ();
					LR.SetPosition(1, hit.point);
					return;
				}
			}
		//if the ray hit nothing
		if(PS.isPlaying)
			PS.Stop();
			//if the ray hit nothing
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
