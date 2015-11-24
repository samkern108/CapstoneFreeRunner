using UnityEngine;
using System.Collections;

// THIS HAS VERY SLOWLY BUT SURELY BECOME SAM'S BABY.
// SHE WILL KEEP WORKING ON IT UNTIL IT IS A PERFECT LITTLE HONOR STUDENT.

public class PlayerController : MonoBehaviour 
{
	public static PlayerController self;

	public PlayerState state;

	public struct PlayerState
	{
		//## RAYCASTING ##//
		public bool onWallBack;
		public bool onWallFront;
		public bool onCeiling;
		public bool onGround;

		//## JUMPING AND FALLING ##//
		public bool isFalling;
		public bool jumping ;
		public bool leanOffWall;
		public bool boostButton;

		public bool facingRight;
	}

	//## INPUT ##//
	[HideInInspector] public bool playerInputEnabled = true;

	//## ANIMATION ##//
	private Animator animator;
	private int animState = 0;
	
	//## RUNNING ##//
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
	private bool warpEnabled = false;
	public Transform wallWarpCheck;
	float nextWarpTime = 0;
	float warpDelay = 0.5f;
	
	//## FALLING ##//
	private float terminalVelocity = -.8f;
	private float gravityFactor = .02f;	
	private bool isFallingOffCeiling = false;
	
	//##  JUMPING  ##//
	private float initialJumpSpeed = .45f;
	private float boostJumpSpeed = .6f;
	private Vector3 currentJumpSpeed;

	//## CURRENT INPUT VALUES ##//
	private float hAxis = 0;
	private float vAxis = 0;


	void Start(){
		animator = this.GetComponent<Animator>();
		self = this;
		state = new PlayerState ();
		state.facingRight = true;
	}


	//## UPDATE ##//

	void FixedUpdate()
	{
		if (playerInputEnabled) {
			vAxis = InputWrapper.GetVerticalAxis ();
			hAxis = InputWrapper.GetHorizontalAxis ();
			state.boostButton = InputWrapper.GetBoost ();
		}

		ApplyGravity ();
	}

	void Update () 
	{
		if (playerInputEnabled) {

			Linecasts();
			if(state.onWallBack) 
				FlipPlayer();

			WarpController();

			bool jumpButtonDown = InputWrapper.GetJump();

			if ((state.onWallFront || state.onWallBack) && !state.jumping) {

				ClimbWalls ();
				JumpOffWalls(jumpButtonDown);

				if(state.onGround || state.onCeiling)
					HorizontalMovement ();
			}
			else if(!state.onCeiling) {
				isFallingOffCeiling = false;
				HorizontalMovement ();
				Jump(jumpButtonDown);
			}

			if(state.onCeiling) {
				HorizontalMovement ();
				JumpOffCeiling(jumpButtonDown);
			}
		
			ApplyJump();

			Animate ();
			transform.position += currentSpeedVector;
		}
	}
	
	private void HorizontalMovement()
	{
		if (state.onGround && !state.onWallFront)
			currentSpeedVector.y = 0;

		if ((hAxis > 0 && !state.facingRight) || (hAxis < 0 && state.facingRight))
			FlipPlayer ();

		if (state.onWallFront || (state.onWallBack && !state.onGround) && !state.onCeiling) {
			currentSpeedVector.x = 0;
			return;
		}
		
		currentSpeedVector.x = hAxis * (state.boostButton ? boostSpeed : maxSpeed);
	}

	private void ClimbWalls()
	{
		if ((hAxis > 0 && !state.facingRight) || (hAxis < 0 && state.facingRight)) {
			state.leanOffWall = true;
		}
		else {
			state.leanOffWall = false;
			currentSpeedVector.x = 0;
		}

		float verticalSpeed = vAxis * (state.boostButton ? boostSpeed : maxSpeed);

		if(state.onGround && verticalSpeed < 0 || state.onCeiling && verticalSpeed > 0)
			verticalSpeed = 0;

		currentSpeedVector.y = verticalSpeed;
	}

	private void JumpOffCeiling(bool jumpButtonDown)
	{
		if (jumpButtonDown) {
			isFallingOffCeiling = true;
			state.isFalling = true;
		} 
		else {
			state.jumping = false;

			if(currentSpeedVector.y > 0)
				currentSpeedVector.y = 0;
		}
	}

	private void JumpOffWalls(bool jumpButtonDown)
	{
		if(jumpButtonDown && state.onWallFront)
		{
			if(state.leanOffWall) {
				if(!state.jumping) {
					if(InputWrapper.GetBoost()) {
						currentJumpSpeed.y = boostJumpSpeed;
						currentSpeedVector.x = Mathf.Sign(hAxis) * boostSpeed;
					}
					else {
						currentJumpSpeed.y = initialJumpSpeed;
						currentSpeedVector.x = Mathf.Sign(hAxis) * maxSpeed;
					}
					state.jumping = true;
				}
			}
			else
				state.isFalling = true;
		}
	}


	private void Jump(bool jumpButtonDown)
	{
		if (jumpButtonDown && state.onGround) {
			if(!state.jumping) {
				currentJumpSpeed.y = state.boostButton ? boostJumpSpeed : initialJumpSpeed;
				state.jumping = true;
			}
		}
	}

	private void ApplyGravity()
	{
		if (!state.onGround && (!state.onCeiling && !isFallingOffCeiling) && !state.onWallFront) {
			state.isFalling = true;
		}
		if(state.isFalling)
		{
			currentSpeedVector.y -= gravityFactor;

			if(currentSpeedVector.y < terminalVelocity) 	
				currentSpeedVector.y = terminalVelocity;
			
			if (!state.onCeiling && !state.onWallFront || state.onGround)
				state.isFalling = false;
		}
	}
	
	private void ApplyJump()
	{
		if (state.jumping) {
			currentJumpSpeed.y -= gravityFactor;
			currentSpeedVector.y = currentJumpSpeed.y;
		}

		if (currentJumpSpeed.y < 0) {
			state.jumping = false;
			state.isFalling = true;
			currentJumpSpeed.y = 0;
		}
	}



	private void FlipPlayer()
	{
		state.facingRight = !state.facingRight;
		Vector3 scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;
	}




	//## LINECASTING ##//

	private void Linecasts()
	{
		state.onGround = Physics2D.Linecast (transform.position, groundChecker.position, 1 << LayerMask.NameToLayer ("Ground"));
		state.onWallFront = Physics2D.Linecast (transform.position, wallCheckerTop.position, 1 << LayerMask.NameToLayer ("Wall")) || Physics2D.Linecast (transform.position, wallCheckerBottom.position, 1 << LayerMask.NameToLayer ("Wall"));
		state.onCeiling = Physics2D.Linecast (transform.position, ceilingChecker.position, 1 << LayerMask.NameToLayer ("Ground"));
		state.onWallBack = Physics2D.Linecast (transform.position, wallCheckerTopBack.position, 1 << LayerMask.NameToLayer ("Wall")) || Physics2D.Linecast (transform.position, wallCheckerBottomBack.position, 1 << LayerMask.NameToLayer ("Wall"));
	}







	//## ANIMATION ##//

	void Animate()
	{
		int newState = animState;
		if (state.onGround) {
			if (hAxis != 0) {
				if (state.boostButton) {
					newState = 2;
				} else {
					newState = 1;
				}
			} else {
				newState = 0;
			}
		} else if (state.onWallFront) {
			newState = 3;
		} else if (state.onWallBack) {
		}
		else if (state.onCeiling) {
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
		if (warpEnabled) {
			float warpInput = Input.GetAxis ("Fire2");
			if (warpInput > 0.5f && Time.time > nextWarpTime) {
				RaycastHit2D castRight = Physics2D.Linecast (transform.position, wallWarpCheck.position, 1 << LayerMask.NameToLayer ("Wall"));
				Vector3 warpVector = new Vector3 (0, 0, 0);
				if (state.onWallFront) {
					if (castRight) {
						float warpAmount = castRight.transform.localScale.x;
						if (state.facingRight) {
							warpVector.x = (warpAmount * 5.65f) + 1;
						} else {
							warpVector.x = (warpAmount * -5.65f) - 1;
						}
						nextWarpTime = Time.time + warpDelay;
						FlipPlayer ();
						transform.position += warpVector;
					}
				}
			}
		}
	}
}
