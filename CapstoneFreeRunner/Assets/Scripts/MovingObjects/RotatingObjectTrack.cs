using UnityEngine;
using System.Collections;

public class RotatingObjectTrack : MonoBehaviour {

	Transform pointA;
	Transform pointB;
	Transform obj;
	float speed = .5f;

	private Quaternion rotationA;
	private Quaternion rotationB;
	
	void Start () {
		obj = transform.GetChild (2);
		pointA = transform.Find("PointA");
		pointB = transform.Find("PointB");

		rotationA = Quaternion.LookRotation (pointA.position - transform.position, Vector3.forward);
		rotationB = Quaternion.LookRotation (pointB.position - transform.position, Vector3.forward);

		rotationA.x = 0;
		rotationA.y = 0;
		rotationB.x = 0;
		rotationB.y = 0;
		Reset ();
	}
	
	void Update () 
	{
		obj.rotation = Quaternion.Slerp (rotationA, rotationB, (Mathf.Sin(speed * Time.time) + 1.0f) / 2.0f);
	}

	void Reset()
	{
		obj.rotation = rotationA;
	}
}
