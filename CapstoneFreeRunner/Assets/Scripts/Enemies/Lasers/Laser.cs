using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour {

	public AudioSource AS;
	public LineRenderer LR;
	public ParticleSystem PS;
	public Transform DirectionHandle;

	public bool endAtDirectionHandle = false;

	//not used right now
	private LineRenderer WhiteLR;

	float maxLaserLength = 1000;

	private bool disabled = false;
	private float dist;
	private float maxDist = 40;
	private float z;

	private Vector2 previoushit;
	
	void Start () {
		LR.useWorldSpace = true;
		z = transform.position.z;

		if(endAtDirectionHandle) {
			maxLaserLength = Vector2.Distance (transform.position, DirectionHandle.position);
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

		Raycast ();
		AudioUpdate ();
	}

	private void AudioUpdate()
	{
		AS.volume = dist.Map (0, 30, .5f, 0);
	}

	Ray2D ray;
	RaycastHit2D[] hitList;

	void Raycast() {
		Vector3 dir = (DirectionHandle.position - transform.position).normalized;		
		ray = new Ray2D(transform.position, dir);

		hitList = Physics2D.RaycastAll (ray.origin, ray.direction, maxLaserLength);

		//draw laser
		UpdateLinePosition (0, ray.origin);

		foreach(RaycastHit2D hit in hitList) {
			//if player enters laser
			if (hit.collider.gameObject.tag == "Player") {
				PlayerHealth.self.PlayerHit (2);
			}
			else if(!hit.collider.CompareTag("Background")){
				if (hit.point != previoushit) {
					previoushit = hit.point;
				
					PS.transform.position = new Vector3 (hit.point.x, hit.point.y, z);
					UpdateLinePosition (1, hit.point);
					Vector3 hitNormal = hit.normal;
					float rot_z = Mathf.Atan2 (hitNormal.y, hitNormal.x) * Mathf.Rad2Deg;
					PS.transform.rotation = Quaternion.Euler (0f, 0f, rot_z - 90);
				
					if (!PS.isPlaying)
						PS.Play ();
				}
				return;
			}
		}
	//if the ray hit nothing
	if(PS.isPlaying)
		PS.Stop();
		//if the ray hit nothing
		Vector3 point = ray.GetPoint(maxLaserLength);
		UpdateLinePosition (1, point);
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
