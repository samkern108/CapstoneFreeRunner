using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	private float offsetX = 0;
	private float offsetY = 0;
	private float targetX = 0;
	private float targetY = 0;
	private float speed;
	private float size;

	void Start () {
		size = this.GetComponent <Camera>().orthographicSize;
		speed = size * 3;
	}

	void LateUpdate () 
	{
		MoveCamera ();
	}

	void Update () {
		bool facingRight = PlayerController.self.state.facingRight;

		if (PlayerController.self.state.onGround) {
			targetY = (float)(size / 3);
		} else if (PlayerController.self.state.onCeiling) {
			targetY = (float)(-size / 3);
		} else {
			targetY = 0;
		}

		if (PlayerController.self.state.onWallFront) {
			targetX = (facingRight ? -1 : 1) * (PlayerController.self.state.leanOffWall ? (float)(size/2) : (float)(size/4));
		} else {
			targetX = 0;
		}
	}

	void MoveCamera() {
		float moveX = (targetX - offsetX) / speed;
		float moveY = (targetY - offsetY) / speed;

		offsetX += moveX;
		offsetY += moveY;

		transform.position = PlayerController.self.transform.position;
		transform.position += new Vector3 (offsetX, offsetY, -9); //The -9 ensures that the camera is "in front of" the hero
	}
}
