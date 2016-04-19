using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public static CameraController self;
	private Camera heroCamera;
	public bool followingPlayer = false;

	private float offsetX = 0;
	private float offsetY = 0;
	private float targetX = 0;
	private float targetY = 0;

	private float shakeOffsetX = 0;
	private float shakeOffsetY = 0;

	private float regularSize;

	void Awake () {
		heroCamera = GetComponent <Camera>();
		regularSize = heroCamera.orthographicSize;

		self = this;
	}

	void LateUpdate () 
	{
		if(followingPlayer) MoveCamera ();
	}

	private Vector3 targetPos;

	public void DriftToPosition(Vector3 pos, float time)
	{
		targetPos = pos;
		targetPos.z = heroCamera.transform.position.z;

		StartCoroutine ("Drift", time);
	}

	IEnumerator Drift(float time)
	{
		int i = 0;
		while(heroCamera.transform.position != targetPos) {
			i++;
			heroCamera.transform.position = Vector3.Lerp (heroCamera.transform.position, targetPos, i/time);
			yield return null;
		}
	}

	public void SetCameraSize(float size)
	{
		heroCamera.orthographicSize = size;
	}

	float targetSize;
	float dt;

	/**
	 * Zooms the camera to %1 size% over %2 time%.
	 * if time is 0, the zoom will be immediate
	 **/
	public void ZoomCamera(float size, float time)
	{
		if (time <= 0) {
			SetCameraSize (size);
		} else {
			targetSize = size;
			dt = (targetSize - regularSize) / time;
			StartCoroutine ("ZoomCoroutine");
		}
	}

	public void RestoreSize(float restoreTime)
	{
		StopCoroutine("ZoomCoroutine");
		targetSize = heroCamera.orthographicSize;
		StartCoroutine ("RestoreCoroutine", restoreTime);
	}

	IEnumerator ZoomCoroutine()
	{
		bool zooming = targetSize > regularSize;
		float tempSize = regularSize;
		while((zooming && tempSize < targetSize) || (!zooming && tempSize > targetSize)) {
			tempSize += dt;
			heroCamera.orthographicSize = tempSize;
			yield return null;
		}
	}

	IEnumerator RestoreCoroutine(float time)
	{
		if (time == 0) {
			heroCamera.orthographicSize = regularSize;
			yield break;
		}
		float dt = (regularSize - targetSize)/time;
		while(targetSize < regularSize) {
			targetSize += dt;
			heroCamera.orthographicSize = targetSize;
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
		transform.position = new Vector3(PlayerController.PlayerPosition.x + shakeOffsetX, PlayerController.PlayerPosition.y + shakeOffsetY, -9);
	}

	/*
	    private float speed = regularSize * 3;
	    void Update () {
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

	void Reset() {
		if (heroCamera.orthographicSize != regularSize) {
			StartCoroutine ("RestoreSize",20);
		}
	}
}
