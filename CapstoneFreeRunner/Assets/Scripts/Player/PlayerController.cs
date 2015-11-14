using UnityEngine;
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
	private int animState = 0;
	
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

	//## WARPING ##//
	public Transform wallWarpCheck;
	float nextWarpTime = 0;
	float warpDelay = 0.5f;

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
	private bool leanOffWall = false;
	private float initialJumpSpeed = .45f;
	private float boostJumpSpeed = .6f;
	private Vector3 currentJumpSpeed;

	//## CURRENT INPUT VALUES ##//
	private float hAxis = 0;
	private float vAxis = 0;
	private bool boostButton = false;


	void Start(){
		animator = this.GetComponent<Animator>();
		self = this;
	}


	//## UPDATE ##//

	void FixedUpdate()
	{
		if (playerInputEnabled) {
			vAxis = InputWrapper.GetVerticalAxis ();
			hAxis = InputWrapper.GetHorizontalAxis ();
			boostButton = InputWrapper.GetBoost ();
		}

		ApplyGravity ();
	}

	void Update () 
	{
		if (playerInputEnabled) {

			WarpController();

			if ((onWallFront || onWallBack) && !jumping) {

				if(onWallBack) 
					FlipPlayer();

				ClimbWalls ();
				JumpOffWalls();

				if(onGround || onCeiling)
					HorizontalMovement ();
			}
			else if(!onCeiling) {
				HorizontalMovement ();
				Jump();
			}

			if(onCeiling) {
				HorizontalMovement ();
				JumpOffCeiling();
			}

			Boost();
			ApplyJump();

			Animate ();
			transform.position += currentSpeedVector;
			Linecasts();
		}
	}
	
	private void HorizontalMovement()
	{
		if (onGround && !onWallFront)
			currentSpeedVector.y = 0;

		if ((hAxis > 0 && !facingRight) || (hAxis < 0 && facingRight))
			FlipPlayer ();

		if (onWallFront || (onWallBack && !onGround) && !onCeiling) {
			currentSpeedVector.x = 0;
			return;
		}
		
		currentSpeedVector.x = hAxis * (boostButton ? boostSpeed : maxSpeed);
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

		float verticalSpeed = vAxis * (boostButton ? boostSpeed : maxSpeed);

		if(onGround && verticalSpeed < 0 || onCeiling && verticalSpeed > 0)
			verticalSpeed = 0;

		currentSpeedVector.y = verticalSpeed;
	}

	private void JumpOffCeiling()
	{
		bool jumpButtonDown = InputWrapper.GetJump ();
		
		if (jumpButtonDown) {
			isFalling = true;
		} 
		else {
			jumping = false;

			if(currentSpeedVector.y > 0)
				currentSpeedVector.y = 0;
		}
	}

	private void JumpOffWalls()
	{
		bool jumpButtonDown = InputWrapper.GetJump ();
		
		if(jumpButtonDown && onWallFront)
		{
			if(leanOffWall) {
				if(!jumping) {
					if(InputWrapper.GetBoost()) {
						currentJumpSpeed.y = boostJumpSpeed;
						currentSpeedVector.x = Mathf.Sign(hAxis) * boostSpeed;
					}
					else {
						currentJumpSpeed.y = initialJumpSpeed;
						currentSpeedVector.x = Mathf.Sign(hAxis) * maxSpeed;
					}
					jumping = true;
				}
			}
			else
				isFalling = true;
		}
	}


	private void Jump()
	{
		bool jumpButtonDown = InputWrapper.GetJump ();
		
		if (jumpButtonDown && onGround) {
			if(!jumping) {
				currentJumpSpeed.y = boostButton ? boostJumpSpeed : initialJumpSpeed;
				jumping = true;
			}
		}
	}

	private void Boost()
	{
//		bool jumpButtonDown = InputWrapper.GetJump ();
	}

	private void ApplyGravity()
	{
		if (!onGround && !onCeiling && !onWallFront) {
			isFalling = true;
		}
		if(isFalling)
		{
			currentSpeedVector.y -= gravityFactor;

			if(currentSpeedVector.y < terminalVelocity) 	
				currentSpeedVector.y = terminalVelocity;
			
			if (!onCeiling && !onWallFront || onGround)
				isFalling = false;
		}
	}
	
	private void ApplyJump()
	{
		bool jumpButtonUp = InputWrapper.GetAbortJump ();

		if(jumpButtonUp && currentJumpSpeed.y > 0) {
			currentJumpSpeed.y -= gravityFactor;
			currentSpeedVector.y = currentJumpSpeed.y;
		}

		if (jumping) {
			currentJumpSpeed.y -= gravityFactor;
			currentSpeedVector.y = currentJumpSpeed.y;
		}

		if (currentJumpSpeed.y < 0) {
			jumping = false;
			isFalling = true;
			currentJumpSpeed.y = 0;
		}
	}



	private void FlipPlayer()
	{
		facingRight = !facingRight;
		Vector3 scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;
	}




	//## LINECASTING ##//

	private void Linecasts()
	{
		onGround = Physics2D.Linecast (transform.position, groundChecker.position, 1 << LayerMask.NameToLayer ("Ground"));
		onWallFront = Physics2D.Linecast (transform.position, wallCheckerTop.position, 1 << LayerMask.NameToLayer ("Wall")) || Physics2D.Linecast (transform.position, wallCheckerBottom.position, 1 << LayerMask.NameToLayer ("Wall"));
		onCeiling = Physics2D.Linecast (transform.position, ceilingChecker.position, 1 << LayerMask.NameToLayer ("Ground"));
		onWallBack = Physics2D.Linecast (transform.position, wallCheckerTopBack.position, 1 << LayerMask.NameToLayer ("Wall")) || Physics2D.Linecast (transform.position, wallCheckerBottomBack.position, 1 << LayerMask.NameToLayer ("Wall"));
	}







	//## ANIMATION ##//

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


	//## WARP ##

	private void WarpController()
	{
		float warpInput = Input.GetAxis ("Fire2");
		if (warpInput > 0.5f && Time.time > nextWarpTime){
			RaycastHit2D castRight = Physics2D.Linecast (transform.position, wallWarpCheck.position, 1 << LayerMask.NameToLayer ("Wall"));
			Vector3 warpVector = new Vector3(0,0,0);
			if (onWallFront){
				if (castRight){
					float warpAmount = castRight.transform.localScale.x;
					Debug.Log(warpAmount);
					if (facingRight){
						warpVector.x = (warpAmount * 5.65f) + 1;
					}else{
						warpVector.x = (warpAmount* -5.65f) - 1;
					}
					nextWarpTime = Time.time + warpDelay;
					FlipPlayer();
					transform.position += warpVector;
				}
			}else{
				
			}
		}
	}
}
