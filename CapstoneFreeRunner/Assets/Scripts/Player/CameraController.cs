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

	void MoveCamera() 
	{
		transform.position = new Vector3(PlayerController.state.position.x, PlayerController.state.position.y, -9);
	}

	/*void Update () {
		bool facingRight = PlayerController.state.facingRight;

		if (PlayerController.state.onGround) {
			targetY = (float)(size / 3);
		} else if (PlayerController.state.onCeiling) {
			targetY = (float)(-size / 3);
		} else {
			targetY = 0;
		}

		if (PlayerController.state.onWallFront) {
			targetX = (facingRight ? -1 : 1) * (PlayerController.state.leanOffWall ? (float)(size/2) : (float)(size/4));
		} else {
			targetX = 0;
		}
	}

	void MoveCamera() {
		float moveX = (targetX - offsetX) / speed;
		float moveY = (targetY - offsetY) / speed;

		offsetX += moveX;
		offsetY += moveY;

		transform.position = PlayerController.state.position;
		transform.position += new Vector3 (offsetX, offsetY, -9); //The -9 ensures that the camera is "in front of" the hero
	}*/
}
