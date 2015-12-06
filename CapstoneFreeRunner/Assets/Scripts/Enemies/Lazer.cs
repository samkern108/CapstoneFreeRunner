using UnityEngine;
using System.Collections;

public class Lazer : MonoBehaviour {

	LineRenderer LR;
	Transform DirectionHandle;
	bool active;
	float maxLazerLength = 10000;
	Color startColor = Color.red;
	Color endColor = Color.red;
	// Use this for initialization
	void Start () {
		LR = gameObject.GetComponent<LineRenderer>();
		DirectionHandle = transform.Find("DirectionHandle");
		active = true;

		LR.material = new Material(Shader.Find("Particles/Additive"));
		LR.SetColors(startColor, endColor);
	}
	
	// Update is called once per frame
	void Update () {
		if (active){
			Vector3 dir = (DirectionHandle.position - transform.position).normalized;		
			Ray2D ray = new Ray2D(transform.position, dir);
			RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, maxLazerLength);

			//draw lazer
			LR.SetPosition(0, ray.origin);
			if(hit == true){
				LR.SetPosition(1,hit.point);
			}else{
				LR.SetPosition(1,ray.GetPoint(maxLazerLength));
			}

			//if player enters lazer
			if (hit.collider.gameObject.tag == "Player"){
				hit.collider.gameObject.SendMessage("KillPlayer");
			}
		}
	}
}
