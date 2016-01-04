using UnityEngine;
using System.Collections;

public class RotatingObjectTrack : MonoBehaviour {

	public Transform obj;
	Transform pointA;
	Transform pointB;
	float speed = .5f;

	private Quaternion rotationA;
	private Quaternion rotationB;
	
	void Start () {
		if (obj == null) {
			obj = transform.GetChild (2);
		}
		pointA = transform.Find("PointA");
		pointB = transform.Find("PointB");
		rotationA = Quaternion.LookRotation ((pointA.position - transform.position).normalized);
		rotationB = Quaternion.LookRotation ((pointB.position - transform.position).normalized);
		rotationA.x = 0;
		rotationA.z = 0;
		rotationB.x = 0;
		rotationB.z = 0;
		obj.rotation = rotationA;
	}
	
	void Update () {
//		obj.Rotate( Vector3.Lerp (pointA.position, pointB.position, (Mathf.Sin(speed * Time.time) + 1.0f) / 2.0f) );
//		Debug.Log (Quaternion.Slerp (pointA.rotation, pointB.rotation, (Mathf.Sin(speed * Time.time) + 1.0f) / 2.0f));
		obj.rotation = Quaternion.Slerp (rotationA, rotationB, (Mathf.Sin(speed * Time.time) + 1.0f) / 2.0f);
//		obj.Rotate ();
	}
}
