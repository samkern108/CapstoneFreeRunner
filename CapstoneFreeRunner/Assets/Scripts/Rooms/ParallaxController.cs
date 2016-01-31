using UnityEngine;
using System.Collections;

public class ParallaxController : MonoBehaviour {

	public Transform backgroundCenter;

	void Start () {
		Parallax[] p = GetComponentsInChildren <Parallax>();
		float percentage = 20 / p.Length;
		for (int i = 1 ; i < p.Length + 1; i++) {
			p[i - 1].Init (backgroundCenter.position, (percentage * i)/1000, (percentage * i-Mathf.Floor(p.Length/2))/3000, (p.Length - i));
		}
	}

	void Update () {
		transform.position = new Vector3(PlayerController.PlayerPosition.x, PlayerController.PlayerPosition.y, 20);
	}
}
