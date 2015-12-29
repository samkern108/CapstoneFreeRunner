using UnityEngine;
using System.Collections;

public class Lazer : MonoBehaviour {

	LineRenderer LR;
	Transform DirectionHandle;
	bool active;
	float maxLazerLength = 1000;
	Color startColor = Color.red;
	Color endColor = Color.red;

	// Use this for initialization
	void Start () {
		LR = gameObject.GetComponent<LineRenderer>();
		DirectionHandle = FindDirectionHandle(this.transform, "DirectionHandle");
		active = true;
		LR.material = new Material(Shader.Find("Particles/Additive"));
		LR.SetColors(startColor, endColor);
	}
	
	// Update is called once per frame
	void Update () {
		Raycast ();
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
				if (hit.collider.gameObject.tag == "Player"){
					hit.collider.gameObject.SendMessage("PlayerHit", 2);
					return;
				}

				if(hit == true && !hit.collider.CompareTag("Background")){
					LR.SetPosition(1,hit.point);
					return;
				}
			}
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
