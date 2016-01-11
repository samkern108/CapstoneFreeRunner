using UnityEngine;
using System.Collections;

public class LinearObjectTrack : MonoBehaviour {

	Transform obj;
	Transform pointA;
	Transform pointB;
	public float speed = 1;

	void Start () {
		obj = transform.GetChild(2);
		pointA = transform.Find("PointA");
		pointB = transform.Find("PointB");
		obj.position = pointA.position;
	}

	void Update () {
		obj.position = Vector3.Lerp (pointA.position, pointB.position, (Mathf.Sin(speed * Time.time) + 1.0f) / 2.0f);
	}

	public void Reset()
	{
		obj.position = pointA.position;
	}
}
