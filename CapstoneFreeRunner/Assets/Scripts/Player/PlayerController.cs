using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
	public Transform playerStartPosition;
	public static GameObject player;

	public static PlayerState state;
	public class PlayerState
	{
		//## POSITION ##//
		public Vector3 position;
		public Vector3 startPosition;

		//## RAYCASTING ##//
		public bool onWallBack;
		public bool onWallFront;
		public bool onCeiling;
		public bool onGround;
		public bool fallingOffCeiling;
		public bool onCeilingCorner;
		public bool onFloorCorner;

		//## WARPING ##//
		public bool drained;

		//## JUMPING AND FALLING ##//
		public bool jumping;
		public bool leanOffWall;
		public bool sprintButton;

		public bool facingRight;

		public bool colliding()
		{
			return onGround || onCeiling || onWallFront;
		}

		public bool inAir()
		{
			return !(onGround || (onCeiling && !fallingOffCeiling) || (onWallFront));//&& !fallingOffWall));
		}
	}

	private float playerWidth = 1f;
	private float playerHeight = 1.2f;

	//## CURRENT INPUT VALUES ##//
	private float hAxis = 0;
	private float vAxis = 0;
	private float hWarp = 0;
	private float vWarp = 0;

	//## UPDATE ##//

	void Update () 
	{
		if (playerInputEnabled) 
		{
			//1: Update Input Values
			vAxis = InputWrapper.GetVerticalAxis ();
			hAxis = InputWrapper.GetHorizontalAxis ();

			vWarp = InputWrapper.GetWarpVertical ();
			hWarp = InputWrapper.GetWarpHorizontal ();

			state.sprintButton = InputWrapper.GetSprint ();
			jumpButtonDown = InputWrapper.GetJump ();

			//2: Conduct Linecasts
			Linecasts();

			//3: Handle Warping
			if (!state.drained && (vWarp != 0 || hWarp != 0)) {
				HandleWarp ();
				if (warpVector.x != 0 || warpVector.y != 0) {
					transform.position += warpVector;
					warpVector = new Vector3 ();
					warping = false;
					state.position = transform.position;
					Animate ();
				} 
				//Time.timeScale = 0;
				return;
			}

			//4: Handle Regular Movement
			if(boostTimer > 0) {
				HandleBoost();
			}
			else {
				if (state.onWallFront) {	
					HandleOnWallFront ();
					if (jumpButtonDown) {
						JumpOffWalls ();
					}
				} else if (state.onWallBack) {
					HandleOnWallBack ();
				}

				if (state.onGround) {
					MoveOnGround ();
					if (jumpButtonDown) {
						Jump ();
					}
				} else if (state.onCeiling) {
					MoveOnCeiling ();
					if (jumpButtonDown) {
						JumpOffCeiling ();
					}
				} else if (state.inAir ()) {
					MoveInAir ();
					if (jumpButtonDown) {
						BoostJump ();
					}
				}
			}

			//5: Apply Movement Vectors to Transform
			if(!boosting) {
				ApplyJump ();
				ApplyGravity ();
				if (!canBoost && state.colliding()) {
					canBoost = true;
				}
			}
			transform.position += currentSpeedVector * 50 * Time.deltaTime;
			state.position = transform.position;

			//6: Animation
			Animate ();
		}
	}

	//## RUNNING ##//
	private static Vector3 currentSpeedVector;
	private float sprintSpeed = .4f;
	private float runSpeed = .2f;

	private void MoveInAir()
	{	
		//if ((hAxis > 0 && !state.facingRight) || (hAxis < 0 && state.facingRight))
		//	FlipPlayer ();

		currentSpeedVector.x = hAxis * runSpeed;
	}

	private void MoveOnGround()
	{
		if (state.onGround && !state.onWallFront)
			currentSpeedVector.y = 0;

		if ((hAxis > 0 && !state.facingRight) || (hAxis < 0 && state.facingRight))
			FlipPlayer ();

		if (state.onWallFront) {
			currentSpeedVector.x = 0;
			return;
		}

		currentSpeedVector.x = hAxis * (state.sprintButton ? sprintSpeed : runSpeed);
	}

	private void MoveOnCeiling()
	{
		if ((hAxis > 0 && !state.facingRight) || (hAxis < 0 && state.facingRight))
			FlipPlayer ();

		if (state.onWallFront) {
			currentSpeedVector.x = 0;
			return;
		}

		currentSpeedVector.x = hAxis * (state.sprintButton ? sprintSpeed : runSpeed);
	}

	private void HandleOnWallBack()
	{
		if ((currentSpeedVector.x < 0 && state.facingRight) || (currentSpeedVector.x > 0 && state.facingRight)) {
			currentSpeedVector.x = 0;
			FlipPlayer ();
		}
	}

	private void HandleOnWallFront ()
	{
		if ((hAxis > 0 && !state.facingRight) || (hAxis < 0 && state.facingRight)) {
			state.leanOffWall = true;
		}
		else {
			state.leanOffWall = false;
			currentSpeedVector.x = 0;
		}

		float verticalSpeed = vAxis * (state.sprintButton ? sprintSpeed : runSpeed);

		if(state.onGround && verticalSpeed < 0 || state.onCeiling && verticalSpeed > 0)
			verticalSpeed = 0;

		currentSpeedVector.y = verticalSpeed;
	}

	//##  JUMPING  ##//
	private bool jumpButtonDown = false;
	private float jumpSpeed = .45f;
	private static Vector3 currentJumpSpeed;

	//## FALLING ##//
	private float terminalVelocity = -.8f;
	private float gravityFactor = .02f;	

	private void JumpOffCeiling()
	{
		state.fallingOffCeiling = true;
		canBoost = true;
	}

	private void JumpOffWalls()
	{
		if(state.leanOffWall) {
			if(!state.jumping) {
				currentJumpSpeed.y = jumpSpeed * vAxis; //## This should allow us to jump sideways, up, or in a downward arc.
				currentSpeedVector.x = Mathf.Sign(hAxis) * runSpeed;
				state.jumping = true;
			}
		}
	}

	private void Jump()
	{
		if (state.onGround) {
			if(!state.jumping) {
				currentJumpSpeed.y = jumpSpeed;
				state.jumping = true;
			}
		}
	}

	private void ApplyGravity()
	{
		if(state.inAir ())
		{
			currentSpeedVector.y -= gravityFactor;

			if(currentSpeedVector.y < terminalVelocity) 	
				currentSpeedVector.y = terminalVelocity;

			//##POTENTIAL BUG: What happens if we're falling off the ceiling onto a wall and never stop colliding?
			if (!state.colliding()) {
				state.fallingOffCeiling = false;
			}
		}
	}

	private void ApplyJump()
	{
		if (state.jumping) {
			currentJumpSpeed.y -= gravityFactor;
			currentSpeedVector.y = currentJumpSpeed.y;

			if(state.onCeiling) {
				state.jumping = false;

				if(currentSpeedVector.y > 0)
					currentSpeedVector.y = 0;
			}
		}

		if (currentJumpSpeed.y < 0) {
			state.jumping = false;
			currentJumpSpeed.y = 0;
		}
	}

	//## BOOSTING ##

	private bool canBoost = true;
	private bool boosting = false;
	private float boostTimeMax = .30f;
	private float boostTimer = 0f;
	private float boostSpeed = .45f;

	private void BoostJump()
	{
		if (canBoost) {
			boostTimer = boostTimeMax;
			boosting = true;
			canBoost = false;
		}
	}

	private void HandleBoost()
	{
		boostTimer -= Time.deltaTime;

		if (state.colliding()) {
			boosting = false;
			boostTimer = 0f;
			currentSpeedVector = new Vector3();
			return;
		}

		if ((hAxis > 0 && !state.facingRight) || (hAxis < 0 && state.facingRight))
			FlipPlayer ();

		currentSpeedVector = boostSpeed * new Vector3 (hAxis != 0 ? 1 * Mathf.Sign (hAxis) : 0, vAxis != 0 ? 1 * Mathf.Sign (vAxis) : 0, 0).normalized;
		if (boostTimer <= 0) {
			boosting = false;
		}
	}


	private void FlipPlayer()
	{
		state.facingRight = !state.facingRight;
		Vector3 scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;
	}

	//## COLLISION CHECKING ##//
	private float zMin;
	private float zMax;

	public Transform groundChecker;
	public Transform wallCheckerTop;
	public Transform wallCheckerBottom;
	public Transform wallCheckerTopBack;
	public Transform wallCheckerBottomBack;
	public Transform ceilingChecker;
	public Transform floorCornerCheck;
	public Transform ceilingCornerCheckBack;
	public Transform ceilingCornerCheckFront;

	private void Linecasts()
	{
		//  Maybe I can get LinecastNonAlloc to work someday.

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

		state.onGround = Physics2D.Linecast (transform.position, groundChecker.position, 1 << LayerMask.NameToLayer ("Wall"), zMin, zMax);
		state.onWallFront = Physics2D.Linecast (wallCheckerBottom.position, wallCheckerTop.position, 1 << LayerMask.NameToLayer ("Wall"), zMin, zMax);
		state.onCeiling = Physics2D.Linecast (transform.position, ceilingChecker.position, 1 << LayerMask.NameToLayer ("Wall"), zMin, zMax);
		state.onWallBack = Physics2D.Linecast (wallCheckerTopBack.position, wallCheckerBottomBack.position, 1 << LayerMask.NameToLayer ("Wall"), zMin, zMax);

		//		state.onFloorCorner = !state.onWallBack && !state.onGround && Physics2D.Linecast (groundChecker.position, floorCornerCheck.position, 1 << LayerMask.NameToLayer ("Wall"), zMin, zMax);
//		state.onCeilingCorner = !state.onWallBack && !state.onCeiling && Physics2D.Linecast (ceilingChecker.position, ceilingCornerCheck.position, 1 << LayerMask.NameToLayer ("Wall"), zMin, zMax);
//		Debug.Log (state.onFloorCorner);
	}

	void OnCollisionEnter2D (Collision2D collision) {
		float z = collision.transform.position.z;
		if (z < zMin || z > zMax) {
			Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
		}
	}


	//## ANIMATION ##//
	private Animator animator;
	private int animState = 0;
	private void Animate()
	{
		int newState = animState;
		if (state.onGround) {
			if (hAxis > .2 || hAxis < -.2) {
				if (state.sprintButton) {
					newState = 2;
				} else {
					newState = 1;
				}
			} else {
				newState = 0;
			}
		} else if (state.onWallFront) {
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


	//## WARPING ##//

	private void Recharge()
	{
		state.drained = false;
	}

	private Vector3 warpVector;
	private bool warping = false;
	private float maxWarpDistance = 2f;

	private void HandleWarp() 
	{
			if(state.onWallFront) 
			{
				RaycastHit2D hit = Physics2D.Linecast (wallCheckerBottom.position, wallCheckerTop.position, 1 << LayerMask.NameToLayer ("Wall"));
				if (hit.collider != null) {
					Bounds hitBounds = hit.collider.bounds;
					Vector3 size = hitBounds.size;

					if (size.x <= maxWarpDistance) {
						if (state.facingRight && hWarp > 0) { //FIX: HWARP >
							warpVector = new Vector3 (size.x + playerWidth, 0, 0);
							FlipPlayer ();
							warping = true;
						} else if (!state.facingRight && hWarp < 0) { //FIX: HWARP <
							warpVector = new Vector3 (-(size.x + playerWidth), 0, 0);
							FlipPlayer ();
							warping = true;
						}
					}
				}
			}
			if(state.onGround && vWarp < 0)
			{
				RaycastHit2D hit = Physics2D.Linecast (transform.position, groundChecker.position, 1 << LayerMask.NameToLayer ("Wall"));
				if (hit.collider != null) {
					Bounds hitBounds = hit.collider.bounds;
					Vector3 size = hitBounds.size;

					if (size.y <= maxWarpDistance) {
						warpVector = new Vector3 (0, -size.y - playerHeight, 0);
						warping = true;
					}
				}
			}
			else if(state.onCeiling && vWarp > 0)
			{
				RaycastHit2D hit = Physics2D.Linecast (transform.position, ceilingChecker.position, 1 << LayerMask.NameToLayer ("Wall"));
				if (hit.collider != null) {
					Bounds hitBounds = hit.collider.bounds;
					Vector3 size = hitBounds.size;

					if (size.y <= maxWarpDistance) {
						warpVector = new Vector3 (0, size.y + playerHeight, 0);
						warping = true;
					}
				}
			}
	}


	//## GAME CONTROL ##//
	void Start()
	{
		animator = this.GetComponent<Animator>();
		state = new PlayerState ();
		state.facingRight = true;
		Reset ();
		state.startPosition = playerStartPosition.position;
		player = this.gameObject;
	}

	public void Reset()
	{
		PlayerInputEnabled (true);
		currentJumpSpeed = new Vector3 ();
		currentSpeedVector = new Vector3 ();
		transform.position = state.startPosition;
		state.position = transform.position;
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
}