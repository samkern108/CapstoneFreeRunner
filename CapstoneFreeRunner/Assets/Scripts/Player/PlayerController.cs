using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
	public Transform playerStartPosition;

	public static PlayerState state;

	public struct PlayerState
	{
		//## POSITION ##//
		public Vector3 position;

		//## RAYCASTING ##//
		public bool onWallBack;
		public bool onWallFront;
		public bool onCeiling;
		public bool onGround;

		//## WARPING ##//
		public bool drained;

		//## JUMPING AND FALLING ##//
		public bool isFalling;
		public bool jumping ;
		public bool leanOffWall;
		public bool boostButton;

		public bool facingRight;

		public bool onWall()
		{
			return onWallBack || onWallFront;
		}
	}

	//## INPUT ##//
	private static bool playerInputEnabled = true;

	public static void PlayerInputEnabled(bool enabled)
	{
		playerInputEnabled = enabled;
		if(!playerInputEnabled) {
			currentSpeedVector = new Vector3();
			currentJumpSpeed = new Vector3();
		}
	}

	//## ANIMATION ##//
	private Animator animator;
	private int animState = 0;
	
	//## RUNNING ##//
	private static Vector3 currentSpeedVector;
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
	float warpDelay = 1f;
	
	//## FALLING ##//
	private float terminalVelocity = -.8f;
	private float gravityFactor = .02f;	
	private bool isFallingOffCeiling = false;
	
	//##  JUMPING  ##//
	private bool jumpButtonDown = false;
	private float initialJumpSpeed = .45f;
	private float boostJumpSpeed = .6f;
	private static Vector3 currentJumpSpeed;

	//## CURRENT INPUT VALUES ##//
	private float hAxis = 0;
	private float vAxis = 0;


	void Start()
	{
		animator = this.GetComponent<Animator>();
		state = new PlayerState ();
		state.facingRight = true;
		Reset ();
	}


	//## UPDATE ##//
	void FixedUpdate()
	{
		if (playerInputEnabled) {
			if (warpVector.x != 0 || warpVector.y != 0) {
				transform.position += warpVector;
				warpVector = new Vector3 ();
			} else {
				ApplyJump ();
				ApplyGravity ();
				transform.position += currentSpeedVector;
			}
			Animate ();
			state.position = transform.position;
		}
	}

	void Update () 
	{
		if (playerInputEnabled) 
		{
			vAxis = InputWrapper.GetVerticalAxis ();
			hAxis = InputWrapper.GetHorizontalAxis ();
			state.boostButton = InputWrapper.GetBoost ();
			jumpButtonDown = InputWrapper.GetJump ();

			Linecasts();
			WarpController ();

			if ((state.onWallFront || state.onWallBack) && !state.jumping) 
			{	
				ClimbWalls ();
				JumpOffWalls (jumpButtonDown);
				
				if (state.onGround || state.onCeiling) {
					HorizontalMovement ();
				}
			} 
			else if (!state.onCeiling) 
			{
				isFallingOffCeiling = false;
				HorizontalMovement ();
				Jump (jumpButtonDown);
			}
			
			if (state.onCeiling) 
			{
				HorizontalMovement ();
				JumpOffCeiling (jumpButtonDown);
			}
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

	private RaycastHit2D[] hit;
	private float zMin;
	private float zMax;

	//## LINECASTING ##//
	private void Linecasts()
	{
		/*  Maybe I can get LinecastNonAlloc to work someday.
		Debug.Log (Physics2D.LinecastNonAlloc (transform.position, groundChecker.position, hit, 1 << LayerMask.NameToLayer ("Ground")));
		state.onGround = (Physics2D.LinecastNonAlloc (transform.position, groundChecker.position, hit, 1 << LayerMask.NameToLayer ("Ground"), -Mathf.Infinity, Mathf.Infinity) > 0);
		state.onWallFront = (Physics2D.LinecastNonAlloc (transform.position, wallCheckerTop.position, hit, 1 << LayerMask.NameToLayer ("Wall"), -Mathf.Infinity, Mathf.Infinity) > 0) || (Physics2D.LinecastNonAlloc (transform.position, wallCheckerBottom.position, hit, 1 << LayerMask.NameToLayer ("Wall"), -Mathf.Infinity, Mathf.Infinity) > 0);
		state.onCeiling = (Physics2D.LinecastNonAlloc (transform.position, ceilingChecker.position, hit, 1 << LayerMask.NameToLayer ("Ground"), -Mathf.Infinity, Mathf.Infinity) > 0);
		state.onWallBack = (Physics2D.LinecastNonAlloc (transform.position, wallCheckerTopBack.position, hit, 1 << LayerMask.NameToLayer ("Wall"), -Mathf.Infinity, Mathf.Infinity) > 0) || (Physics2D.LinecastNonAlloc (transform.position, wallCheckerBottomBack.position, hit, 1 << LayerMask.NameToLayer ("Wall"), -Mathf.Infinity, Mathf.Infinity) > 0);
		 */

		if (transform.parent.name.Equals("Level")) {
			zMin = 15;
			zMax = Mathf.Infinity;
		} else {
			float z = transform.parent.position.z;
			if(z > 15) {
				zMin = 6;
				zMax = 14;
			}
			else {
				zMin = 15;
				zMax = Mathf.Infinity;
			}
		}

		state.onGround = Physics2D.Linecast (transform.position, groundChecker.position, 1 << LayerMask.NameToLayer ("Ground"), zMin, zMax);
		state.onWallFront = Physics2D.Linecast (transform.position, wallCheckerTop.position, 1 << LayerMask.NameToLayer ("Wall"), zMin, zMax) || Physics2D.Linecast (transform.position, wallCheckerBottom.position, 1 << LayerMask.NameToLayer ("Wall"), zMin, zMax);
		state.onCeiling = Physics2D.Linecast (transform.position, ceilingChecker.position, 1 << LayerMask.NameToLayer ("Ground"), zMin, zMax);
		state.onWallBack = Physics2D.Linecast (transform.position, wallCheckerTopBack.position, 1 << LayerMask.NameToLayer ("Wall"), zMin, zMax) || Physics2D.Linecast (transform.position, wallCheckerBottomBack.position, 1 << LayerMask.NameToLayer ("Wall"), zMin, zMax);

		if(state.onWallBack) 
			FlipPlayer();
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
		} else if (state.onWall ()) {
			newState = 3;
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
	private void Recharge()
	{
		state.drained = false;
	}
	
	void OnCollisionEnter2D (Collision2D collision) {
		float z = collision.collider.transform.position.z;
		if (z < zMin || z > zMax) {
			Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
		}
	}

	private Vector3 warpVector;

	//As of right now, the player must hold the arrow key in the direction they want to warp
	//This is to avoid ceiling/wall or floor/wall warp conflicts.
	//I will add a few more "if" statements later to clear these conflicts.

	//The code is also a little ugly and redundant.  I'll fix it later. (TM)
	private void WarpController() 
	{
		if (!state.drained && InputWrapper.GetWarp()) {

			if(state.onGround) {

				RaycastHit2D hit = Physics2D.Linecast (transform.position, groundChecker.position, 1 << LayerMask.NameToLayer ("Ground"));
				Bounds hitBounds = hit.collider.bounds;
				Vector3 size = hitBounds.size;
			
				warpVector = new Vector3(0, -size.y*2, 0);
			}
			else if(state.onCeiling) {

				RaycastHit2D hit = Physics2D.Linecast (transform.position, ceilingChecker.position, 1 << LayerMask.NameToLayer ("Ground"));
				Bounds hitBounds = hit.collider.bounds;
				Vector3 size = hitBounds.size;

				warpVector = new Vector3(0, size.y*2, 0);
			}
			else if(state.onWall() && state.facingRight) {

				RaycastHit2D hit = Physics2D.Linecast (transform.position, wallCheckerTop.position, 1 << LayerMask.NameToLayer ("Wall"));
				if(!hit) {
					hit = Physics2D.Linecast (transform.position, wallCheckerBottom.position, 1 << LayerMask.NameToLayer ("Wall"));
				}
				Bounds hitBounds = hit.collider.bounds;
				Vector3 size = hitBounds.size;

				warpVector = new Vector3(size.x*2,0,0);
			}
			else if(state.onWall() && !state.facingRight) {

				RaycastHit2D hit = Physics2D.Linecast (transform.position, wallCheckerTop.position, 1 << LayerMask.NameToLayer ("Wall"));
				if(!hit) {
					hit = Physics2D.Linecast (transform.position, wallCheckerBottom.position, 1 << LayerMask.NameToLayer ("Wall"));
				}

				Bounds hitBounds = hit.collider.bounds;
				Vector3 size = hitBounds.size;

				warpVector = new Vector3(-size.x*2,0,0);
			}
		}
	}

	public void Reset()
	{
		PlayerInputEnabled (true);
		vAxis = 0;
		hAxis = 0;
		currentJumpSpeed = new Vector3 ();
		currentSpeedVector = new Vector3 ();
		transform.position = playerStartPosition.position;
	}
}