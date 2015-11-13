﻿using UnityEngine;
using System.Collections;

// THIS HAS VERY SLOWLY BUT SURELY BECOME SAM'S BABY.
// SHE WILL KEEP WORKING ON IT UNTIL IT IS A PERFECT LITTLE HONOR STUDENT.

public class PlayerController : MonoBehaviour 
{
	public static PlayerController self;

	//## INPUT ##//
	[HideInInspector] public bool playerInputEnabled = true;

	//## ANIMATION ##//
	private Animator animator;

	//## RUNNING ##//
	private bool facingRight = true;
	private Vector3 currentSpeedVector;
	private float boostSpeed = .4f;
	private float maxSpeed = .2f;
	
	//## COLLISION CHECKING ##//
	public Transform groundChecker;
	public Transform wallCheckerTop;
	public Transform wallCheckerBottom;
	public Transform wallCheckerTopBack;
	public Transform wallCheckerBottomBack;
	public Transform ceilingChecker;

	//## RAYCASTING ##//
	private bool onWallBack = false;
	private bool onWallFront = false;
	private bool onCeiling = false;
	private bool onGround = false;

	//## FALLING ##//
	private float terminalVelocity = -.8f;
	private bool isFalling = false;
	private float gravityFactor = .02f;	
	
	//##  JUMPING  ##//
	[HideInInspector] public bool jumping = false;
	private float jumpOffWallSpeedMod = .7f;
	private float initialJumpSpeed = .45f;
	private float boostJumpSpeed = .6f;
	private float currentJumpSpeed;
	private bool leanOffWall = false;
	private float xJumpMod = .1f;
	private float boostXJumpMod = .2f;

	//## CURRENT INPUT VALUES ##//
	private float hAxis = 0;
	private float vAxis = 0;
	private bool boostButton = false;

	void Start(){
		animator = this.GetComponent<Animator>();
		self = this;
	}

	void FixedUpdate()
	{
		vAxis = InputWrapper.GetVerticalAxis ();
		hAxis = InputWrapper.GetHorizontalAxis ();
		boostButton = InputWrapper.GetBoost();
		
		if (!onGround && !onCeiling && !onWallFront) {
			isFalling = true;
		}
		if(isFalling)
		{
			currentSpeedVector.y -= gravityFactor;
			if(currentSpeedVector.y < terminalVelocity) {
				currentSpeedVector.y = terminalVelocity;
			}

			if (!onCeiling && !onWallFront || onGround) {
				isFalling = false;
			} 
		}
		if (onGround && !onWallFront) {
			currentSpeedVector.y = 0;
		}
	}

	void Update () 
	{
		if (playerInputEnabled){
			JumpController();

			if(onGround) {
				HorizontalMovement();
			}
			if (onWallFront || onWallBack){
				ClimbWalls();
			}
			else if(!onGround){
				HorizontalMovement();
			}

			if (onCeiling) {
				ClimbCeiling();
			}
		}
		transform.position += currentSpeedVector;
		CollisionCheck();
		Animate();
	}

	private int animState = 0;

	void Animate()
	{
		int newState = animState;
		if (onGround) {
			if (hAxis != 0) {
				if (boostButton) {
					newState = 2;
				} else {
					newState = 1;
				}
			} else {
				newState = 0;
			}
		} else if (onWallFront) {
			newState = 3;
		} else if (onWallBack) {
		}
		else if (onCeiling) {
			newState = 4;
		} else {
			newState = 5;
		}
		if (animState != newState) {
			animator.SetInteger ("State", newState);
			animState = newState;
		}
	}

	private void HorizontalMovement()
	{
		if ((hAxis > 0 && !facingRight) || (hAxis < 0 && facingRight)) {
			FlipPlayer ();
		} 
		else if (onWallFront || onWallBack) {
			currentSpeedVector.x = 0;
			return;
		}
		
		if (boostButton) {
			currentSpeedVector.x = hAxis * boostSpeed;
		}
		else {
			currentSpeedVector.x = hAxis * maxSpeed;
		}
	}

	private void ClimbWalls()
	{
		if ((hAxis > 0 && !facingRight) || (hAxis < 0 && facingRight)) {
			leanOffWall = true;
		}
		else {
			leanOffWall = false;
			currentSpeedVector.x = 0;
		}

		float verticalSpeed = 0;

		verticalSpeed = vAxis * (boostButton ? boostSpeed : maxSpeed);

		if(onGround && verticalSpeed < 0 || onCeiling && verticalSpeed > 0) {
			verticalSpeed = 0;
		}

		currentSpeedVector.y = verticalSpeed;
	}

	private void ClimbCeiling()
	{
		if ((hAxis > 0 && !facingRight) || (hAxis < 0 && facingRight)) {
			FlipPlayer ();
		} 
		else if (onWallFront) {
			currentSpeedVector.x = 0;
			return;
		}

		currentSpeedVector.x = hAxis * (boostButton ? boostSpeed : maxSpeed);
	}

	private void JumpController()
	{
		bool jumpButtonDown = InputWrapper.GetJump ();
		bool jumpButtonUp = InputWrapper.GetAbortJump ();

		if (jumpButtonDown && onCeiling) {
			isFalling = true;
			return;
		} else if (onCeiling) {
			jumping = false;
			currentJumpSpeed = 0;
			return;
		}

		if(jumpButtonDown && onWallFront)
		{
			if(leanOffWall) {
				if(!jumping) {
					if(InputWrapper.GetBoost()) {
						currentJumpSpeed = boostJumpSpeed;
						currentSpeedVector.x = Mathf.Sign(hAxis) * boostSpeed;
					}
					else {
						currentJumpSpeed = initialJumpSpeed;
						currentSpeedVector.x = Mathf.Sign(hAxis) * maxSpeed;
					}
					jumping = true;
				}
			}
			else {
				isFalling = true;
			}
		}

		if (jumpButtonDown && onGround) {
			if(!jumping) {
				if(InputWrapper.GetBoost()) {
					currentJumpSpeed = boostJumpSpeed;
				}
				else {
					currentJumpSpeed = initialJumpSpeed;
				}
				jumping = true;
			}
		}
		else if(jumpButtonUp && currentJumpSpeed > 0) {
			currentJumpSpeed -= gravityFactor;
			if(currentJumpSpeed < 0) {
				jumping = false;
				currentJumpSpeed = 0;
			}
		}

		if (jumping) {
			currentJumpSpeed -= gravityFactor;
			if (currentJumpSpeed < 0) {
				jumping = false;
				currentJumpSpeed = 0;
			}
			currentSpeedVector.y = currentJumpSpeed;
		}
	}

	private void FlipPlayer()
	{
		facingRight = !facingRight;
		Vector3 scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;
	}

	private void CollisionCheck()
	{
		onGround = Physics2D.Linecast (transform.position, groundChecker.position, 1 << LayerMask.NameToLayer ("Ground"));
		onWallFront = Physics2D.Linecast (transform.position, wallCheckerTop.position, 1 << LayerMask.NameToLayer ("Wall")) || Physics2D.Linecast (transform.position, wallCheckerBottom.position, 1 << LayerMask.NameToLayer ("Wall"));
		onCeiling = Physics2D.Linecast (transform.position, ceilingChecker.position, 1 << LayerMask.NameToLayer ("Ground"));
		onWallBack = Physics2D.Linecast (transform.position, wallCheckerTopBack.position, 1 << LayerMask.NameToLayer ("Wall")) || Physics2D.Linecast (transform.position, wallCheckerBottomBack.position, 1 << LayerMask.NameToLayer ("Wall"));
	}







	//when we warp, we want to hide the player, move them one pixel at a time 
	//very quickly until they are colliding with the opposite wall.  :)



	//## WARP ##

	private bool canWarpRight = false;
	private bool canWarpLeft = false;
	private bool canWarpTop = false;
	private bool canWarpBottom = false;
	
	private float warpAmount = 2;
	

	private void WarpController()
	{
		float warpInput = Input.GetAxis ("Fire2");

		if (warpInput > 0){

		}
	}
}
