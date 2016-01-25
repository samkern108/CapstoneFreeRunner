﻿using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public static CameraController self;

	private float offsetX = 0;
	private float offsetY = 0;
	private float targetX = 0;
	private float targetY = 0;

	private float shakeOffsetX = 0;
	private float shakeOffsetY = 0;

	private float speed;
	private float regularSize;

	void Reset() {
		if (GetComponent <Camera> ().orthographicSize != regularSize) {
			StartCoroutine ("RestoreSize",20);
		}
	}

	void Start () {
		regularSize = GetComponent <Camera>().orthographicSize;
		speed = regularSize * 3;
		self = this;
	}

	void LateUpdate () 
	{
		MoveCamera ();
	}

	float targetSize;
	float dt;

	/**
	 * Zooms the camera by %1 percentage% over %2 time%, then restore over %3 time%
	 * Example: to double size, percentage should be 2
	 * time and rTime should be greater than one.
	 **/
	public void ZoomCamera(float percentage, float time, float rTime)
	{
		targetSize = regularSize * percentage;
		dt = (targetSize - regularSize)/time;
		Debug.Log (regularSize + " " + percentage + " " + targetSize + "  " + dt);
		StartCoroutine ("ZoomCoroutine", rTime);
	}

	public void RestoreSize(float restoreTime)
	{
		StartCoroutine ("RestoreCoroutine", restoreTime);
	}

	IEnumerator ZoomCoroutine(float restoreTime)
	{
		bool zooming = targetSize > regularSize;
		float tempSize = regularSize;
		while((zooming && tempSize < targetSize) || (!zooming && tempSize > targetSize)) {
			tempSize += dt;
			GetComponent <Camera> ().orthographicSize = tempSize;
			yield return null;
		}
		if (restoreTime != -1) {
			StartCoroutine ("RestoreCoroutine", restoreTime);
		}
		Debug.Log ("DONE");
	}

	IEnumerator RestoreCoroutine(float time)
	{
		if (time == 0) {
			GetComponent <Camera> ().orthographicSize = regularSize;
			yield break;
		}
		float dt = (regularSize - targetSize)/time;
		while(targetSize < regularSize) {
			targetSize += dt;
			GetComponent <Camera> ().orthographicSize = targetSize;
			yield return null;
		}
	}

	public void ShakeCamera(float amount)
	{
		shakeOffsetX = Random.Range(-1, 1) * Mathf.PerlinNoise (Time.time, Time.time) * amount;
		shakeOffsetY = Random.Range(-1, 1) * Mathf.PerlinNoise (Time.time, Time.time) * amount;
	}

	void MoveCamera() 
	{
		transform.position = new Vector3(PlayerController.state.position.x + shakeOffsetX, PlayerController.state.position.y + shakeOffsetY, -9);
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
