using UnityEngine;
using System.Collections;

public class LinearObjectTrack : MonoBehaviour {

	Transform obj;
	Transform pointA;
	Transform pointB;
	float speed = 1;
	// Use this for initialization
	void Start () {
		obj = transform.GetChild(2);
		pointA = transform.Find("PointA");
		pointB = transform.Find("PointB");
	}
	
	// Update is called once per frame
	void Update () {
		obj.position = Vector3.Lerp (pointA.position, pointB.position, (Mathf.Sin(speed * Time.time) + 1.0f) / 2.0f);
	}
}
