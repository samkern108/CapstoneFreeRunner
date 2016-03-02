using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour {

	AudioSource AS;
	private LineRenderer LR;
	private LineRenderer WhiteLR;
	public ParticleSystem PS;
	public bool endAtDirectionHandle = false;

	Transform DirectionHandle;
	bool active;
	float maxLazerLength = 1000;
	Color color = Color.red;
	private bool disabled = false;
	private float dist;
	private float maxDist = 40;
	private float z;
	
	void Start () {
		AS = GetComponent<AudioSource> ();
		DirectionHandle = FindDirectionHandle(this.transform, "DirectionHandle");
		active = true;
		LR = GetComponent<LineRenderer> ();
		LR.useWorldSpace = true;
		if (WhiteLR == null) {
			LR.material = new Material (Shader.Find ("Particles/Additive"));
		}
		LR.SetColors(color, color);
		z = transform.position.z;

		if(endAtDirectionHandle) {
			maxLazerLength = Vector2.Distance (transform.position, DirectionHandle.position);
		}
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
		AS.volume = dist.Map (0, 30, .5f, 0);
	}

	void Raycast() {
		if (active){
			Vector3 dir = (DirectionHandle.position - transform.position).normalized;		
			Ray2D ray = new Ray2D(transform.position, dir);

			RaycastHit2D[] hitList = Physics2D.RaycastAll (ray.origin, ray.direction, maxLazerLength);

			//draw laser
			UpdateLinePosition (0, ray.origin);

			foreach(RaycastHit2D hit in hitList) {
				//if player enters laser
				if (hit.collider.gameObject.tag == "Player"){
					hit.collider.gameObject.SendMessage("PlayerHit", 2);
					return;
				}

				if(hit == true && !hit.collider.CompareTag("Background")){
					PS.transform.position = new Vector3 (hit.point.x, hit.point.y, z);
					if (!PS.isPlaying)
						PS.Play ();
					UpdateLinePosition (1, hit.point);
					return;
				}
			}
		//if the ray hit nothing
		if(PS.isPlaying)
			PS.Stop();
			//if the ray hit nothing
			Vector3 point = ray.GetPoint(maxLazerLength);
			UpdateLinePosition (1, point);
		}
	}

	private void UpdateLinePosition(int pos, Vector2 point)
	{
		LR.SetPosition(pos, new Vector3(point.x, point.y, z));
		if (WhiteLR != null) {
			WhiteLR.SetPosition (pos, new Vector3 (point.x, point.y, z + 1));
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
