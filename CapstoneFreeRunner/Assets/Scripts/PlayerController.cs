﻿using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	private Rigidbody2D rb2d;

	[HideInInspector] public bool facingRight = true;

	[HideInInspector] public bool playerInputEnabled = true;

	private float boostMod = 1.5f;
	//private bool boost = false;

	private float moveForce = 30;
	private float maxSpeed = 5;

	[HideInInspector] public bool jumping = false;
	private float jumpForce = 800;
	private float fallDelayInterval = 0.1f;
	private float fallTime;
	private bool fallTimeRunning = false;

	private float climbingForce = 50;
	private float maxClimbingSpeed = 2;
	//private float climbingSpeed = 0.05f;

	// used to remove jump forces on first contact with wall
	private bool canTriggeronWallTopHit = true; 

	public Transform groundChecker;
	private bool onGround = false;

	public Transform wallCheckerTop;
	private bool onWallTop = false;

	public Transform wallCheckerBottom;
	private bool onWallBottom = false;

	public Transform ceilingChecker;
	private bool onCeiling = false;

	void Awake(){
		rb2d = GetComponent<Rigidbody2D> ();
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

		CollisionCheck();

		if (playerInputEnabled){
			MoveController();
		}
	}

	void FixedUpdate () {

		//cancle out gravity
		if (onWallTop || onCeiling){
			//rb2d.gravityScale = 0;
			Vector3 t = Physics.gravity * rb2d.mass;
			rb2d.AddForce (-t);
		}
	}

	void CollisionCheck(){

		// TODO Ray casts should NOT go diagonally from the center of the player

		// is the player on the ground
		onGround = Physics2D.Linecast (transform.position, groundChecker.position, 1 << LayerMask.NameToLayer ("Ground"));
		
		// is the player on a wall
		onWallTop = Physics2D.Linecast (transform.position, wallCheckerTop.position, 1 << LayerMask.NameToLayer ("Ground"));
		
		// is the player on a wall
		onWallBottom = Physics2D.Linecast (transform.position, wallCheckerBottom.position, 1 << LayerMask.NameToLayer ("Ground"));
		
		// is the player on a ceiling
		onCeiling = Physics2D.Linecast (transform.position, ceilingChecker.position, 1 << LayerMask.NameToLayer ("Ground"));
	}

	void MoveController () {

		JumpController();

		if (onWallTop){

			ClimbController();

		}

		if (!onWallTop){

			WalkController();

		}

		if (onWallTop || onWallBottom || onCeiling){
			//wait for jump before being pushed off wall
			if (Input.GetAxis ("Horizontal") != 0){
				if (!fallTimeRunning){
					fallTime = Time.time + fallDelayInterval;
					fallTimeRunning = true;
				}
			} else {
				fallTimeRunning = false;
			}
			if (Time.time > fallTime){
				WalkController();
			}
		}
	}

	void WalkController(){

		//rb2d.gravityScale = 2;
		rb2d.drag = 0;
		canTriggeronWallTopHit = true;

		float horizontalInput = Input.GetAxis ("Horizontal");

		float force = BoostController(moveForce);
		float max = BoostController(maxSpeed);

		// if player has not reached maxSpeed
		if (horizontalInput * rb2d.velocity.x < max) {
			//add force to player
			rb2d.AddForce(Vector2.right * horizontalInput * force);
		}
		
		// if the player has exceeded maxSpeed
		if (Mathf.Abs (rb2d.velocity.x) > max) {
			//set the player's velocity to the maxSpeed in the x axis
			rb2d.velocity = new Vector2 (Mathf.Sign (rb2d.velocity.x) * max, rb2d.velocity.y);
		}
		
		// should the player be flipped
		if (horizontalInput > 0 && !facingRight) {
			FlipPlayer ();
		} else if (horizontalInput < 0 && facingRight) {
			FlipPlayer ();
		}
	}

	void ClimbController(){

		float verticalInput = Input.GetAxis ("Vertical");

		rb2d.drag = 5;

		float force = BoostController(climbingForce);
		float max = BoostController(maxClimbingSpeed);
		
		//remove jump velocity on first contact
		if (canTriggeronWallTopHit){
			rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
			canTriggeronWallTopHit = false;
		}

		// if player has not reached maxSpeed
		if (verticalInput * rb2d.velocity.y < max) {
			//add force to player
			rb2d.AddForce(Vector2.up * verticalInput * force);
		}
		
		// if the player has exceeded maxSpeed
		if (Mathf.Abs (rb2d.velocity.y) > max) {
			//set the player's velocity to the maxSpeed in the x axis
			rb2d.velocity = new Vector2 (rb2d.velocity.x, Mathf.Sign (rb2d.velocity.y) * max);
		}
	}

	void JumpController(){

		if (Input.GetButtonDown ("Jump") && (onGround || onWallTop || onWallBottom) && !onCeiling) {
			// jump
			rb2d.AddForce(new Vector2(0f, jumpForce));
			onWallTop = false;
			onWallBottom = false;
			//onGround = false;
		} else if (Input.GetButtonUp ("Jump") && rb2d.velocity.y > 0) {
			// abort jump
			Debug.Log ("Abort");
			rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
		}
	}

	float BoostController(float val) {
		float boostInput = Input.GetAxis ("Fire1");
		if (boostInput <= 0 ){
			return val * boostMod;
		}else{
			return val;
		}
	}

	void FlipPlayer(){
		facingRight = !facingRight;
		// flip the players scale across the x axis
		Vector3 scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;
	}
}
