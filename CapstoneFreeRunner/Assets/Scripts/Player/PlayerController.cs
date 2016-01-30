using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
	public Transform playerStartPosition;
	public static GameObject player;
	public static Transform staticTransform;

	public enum Corner {topFront, topBack, bottomFront, bottomBack, noCorner};
	public static PlayerState state;
	public class PlayerState
	{
		//## POSITION ##//
		public Vector3 respawnPosition;

		//## RAYCASTING ##//
		public bool onWallBack;
		public bool onWallFront;
		public bool onCeiling;
		public bool onGround;

		public Corner collidingCorner;

		public bool fallingOffCeiling;
		public bool fallingOffWall;

		//## WARPING ##//
		public bool drained;

		//## JUMPING AND FALLING ##//
		public bool jumping;
		public bool leanOffWall;
		public bool sprintButton;

		public bool facingRight;

		public bool OnCorner()
		{
			return (collidingCorner != Corner.noCorner);
		}

		public bool Colliding()
		{
			return onGround || onCeiling || onWallFront || OnCorner();
		}

		public bool InAir()
		{
			return !Colliding () || fallingOffCeiling || fallingOffWall;
		}
	}

	private float playerWidth = 1f, playerHeight = 1.2f;

	//## CURRENT INPUT VALUES ##//
	private float hAxis = 0, vAxis = 0, hWarp = 0, vWarp = 0;

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
					//animator.SetTrigger();
					//animator.ResetTrigger ();
				} 
				return;
			}

			//4: Handle Regular Movement
			if(boostTimer > 0) {
				HandleBoost();
			}
			else {
				if (state.OnCorner()) {
					MoveOnCorner ();
					transform.position += currentSpeedVector * 50 * Time.deltaTime;
					//animator.SetTrigger();
					//animator.ResetTrigger ();
					return;
				}
				else if (state.onWallFront) {
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
				} else if (state.onCeiling && !state.fallingOffCeiling) {
					MoveOnCeiling ();
					if (jumpButtonDown && vAxis < 0) {
						JumpOffCeiling ();
					}
				} else if (state.InAir ()) {
					MoveInAir ();
					ApplyGravity ();
					if (jumpButtonDown) {
						BoostJump ();
					}
				 }
			}

			 //5: Animation
			Animate ();

			 //6: Apply Movement Vectors to Transform
			if(!boosting) {
				if (!canBoost && state.Colliding()) {
					canBoost = true;
				}
			}
			if (state.jumping && state.Colliding ()) {
				state.jumping = false;
			}
			transform.position += currentSpeedVector * 50 * Time.deltaTime;
		}
	}

	//## RUNNING ##//
	private static Vector3 currentSpeedVector;
	private float runSpeed = .2f, sprintSpeed = .4f;



	float x, y;
	Vector2 checkerX, checkerY, cornerChecker;

	//0: move 				(1 : yes, -1 : no)
	//0: reverse X 			(1 : *, -1 : *)
	//1: reverse Y 			(1 : *, -1 : *)
	//2: primary direction 	(1 : x, -1 : y)
	short[] cornerMoveData = new short[4];

	private void MoveOnCorner()
	{
		currentSpeedVector = new Vector3 ();

		//Debug.Log (state.collidingCorner);

		switch (state.collidingCorner) {
		case Corner.bottomBack:
			if (vAxis < .3f) {
				cornerMoveData = new short[4]{1, 1, -1, -1};
				FlipPlayer ();
				cornerChecker = floorCornerCheckBack.position;
				checkerX = groundChecker.position;
				checkerY = wallCheckerBottomBack.position;
			} else if ((hAxis > 3f && !state.facingRight) || (hAxis < -.3f && state.facingRight)) {
				cornerMoveData = new short[4]{1, -1, 1, 1};
				cornerChecker = floorCornerCheckBack.position;
				checkerX = groundChecker.position;
				checkerY = wallCheckerBottomBack.position;
			}
			break;
		case Corner.bottomFront:
			if (vAxis < .3f) {
				cornerMoveData = new short[4]{1, -1, -1, -1};
				cornerChecker = floorCornerCheckFront.position;
				checkerX = groundChecker.position;
				checkerY = wallCheckerBottom.position;
			} else if ((hAxis < -.3f && !state.facingRight) || (hAxis > .3f && state.facingRight)) {
				cornerMoveData = new short[4]{1, 1, 1, 1};
				cornerChecker = floorCornerCheckFront.position;
				checkerX = groundChecker.position;
				checkerY = wallCheckerBottom.position;
			}
			break;
		case Corner.topBack:
			if (vAxis > .3f) {
				FlipPlayer ();
				cornerMoveData = new short[4]{1, 1, 1, -1};
				cornerChecker = ceilingCornerCheckBack.position;
				checkerX = ceilingChecker.position;
				checkerY = wallCheckerTopBack.position;
			} else if ((hAxis > .3f && !state.facingRight) || (hAxis < -.3f && state.facingRight)) {
				cornerMoveData = new short[4]{1, -1, -1, 1};
				cornerChecker = ceilingCornerCheckBack.position;
				checkerX = ceilingChecker.position;
				checkerY = wallCheckerTopBack.position;
			}
			break;
		case Corner.topFront:
			if (vAxis > .3f) {
				cornerMoveData = new short[4]{ 1, -1, 1, -1 };
				cornerChecker = ceilingCornerCheckFront.position;
				checkerX = ceilingChecker.position;
				checkerY = wallCheckerTop.position;
			} else if ((hAxis < -.3f && !state.facingRight) || (hAxis > .3f && state.facingRight)) {
				cornerMoveData = new short[4]{ 1, 1, -1, 1 };
				cornerChecker = ceilingCornerCheckFront.position;
				checkerX = ceilingChecker.position;
				checkerY = wallCheckerTop.position;
			}
			break;
		}

		if(cornerMoveData[0] == 1) {
			float distanceX = Physics2D.Linecast (checkerX, cornerChecker, 1 << LayerMask.NameToLayer ("Wall")).distance;
			float distanceY = Physics2D.Linecast (checkerY, cornerChecker, 1 << LayerMask.NameToLayer ("Wall")).distance;

			switch(cornerMoveData[3])
			{
			case 1: //moving primarily in x
				distanceX = Vector2.Distance (checkerX, cornerChecker) - distanceX;
				distanceY = 0;
				break;
			case -1: //moving primarily in y
				distanceY = Vector2.Distance (checkerY, cornerChecker) - distanceY;
				//distanceX = 0;
				break;
			}

			Debug.Log (cornerMoveData[0] + " " + cornerMoveData[1] + " " + cornerMoveData[2] + " " + cornerMoveData[3] + "  " + distanceX + "  " + distanceY);

			transform.position += new Vector3 (cornerMoveData[1] * distanceX * (state.facingRight ? 1 : -1), cornerMoveData[2] * distanceY, 0);
			cornerMoveData [0] = -1;
		}
	}

	private void MoveInAir()
	{
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
		if (state.fallingOffWall) {
			if ((hAxis > 0 && state.facingRight) || (hAxis < 0 && !state.facingRight) || state.onGround || state.onCeiling) {
				state.fallingOffWall = false;
			} 
			else {
				return;
			}
		}

		float verticalSpeed = 0;
		if ((hAxis > .5f && !state.facingRight) || (hAxis < -.5f && state.facingRight)) {
			 state.leanOffWall = true;
		} else {
			state.leanOffWall = false;
			currentSpeedVector.x = 0;
			if(!(state.onGround && vAxis < 0) && !(state.onCeiling && vAxis > 0))
				verticalSpeed = vAxis * (state.sprintButton ? sprintSpeed : runSpeed);
		}

		currentSpeedVector.y = verticalSpeed;
	}

	//## JUMPING ##//
	private bool jumpButtonDown = false;
	private float jumpSpeed = .45f;
	private float jumpArc = .35f;

	//## FALLING ##//
	private float terminalVelocity = -.8f, gravityFactor = .02f;

	private void JumpOffCeiling()
	{
		state.fallingOffCeiling = true;
		canBoost = true;
	}

	private void JumpOffWalls()
	{
		if (state.leanOffWall) {
			if (!state.jumping) {
				currentSpeedVector.y = (Mathf.Abs(vAxis) > .8f ? Mathf.Sign(vAxis)*jumpArc : 0);
				currentSpeedVector.x = Mathf.Sign(hAxis) * jumpSpeed;
				state.jumping = true;
			}
		} else {
			//transform.position += new Vector3 ((state.facingRight ? -1 : 1), 0, 0);
			//state.fallingOffWall = true;
		}
	}

	private void Jump()
	{
		if (state.onGround && !state.jumping) {
			currentSpeedVector.y = jumpSpeed;
			state.jumping = true;
		}
	}

	private void ApplyGravity()
	{
		currentSpeedVector.y -= gravityFactor;

		if (currentSpeedVector.y < terminalVelocity)
			currentSpeedVector.y = terminalVelocity;

		if (state.fallingOffCeiling && !state.onCeiling) {
			state.fallingOffCeiling = false;
		} 
		else if (state.fallingOffWall && state.onCeiling || state.onGround) {
			state.fallingOffWall = false;
		}
	}

	//## BOOSTING ##

	private bool canBoost = true, boosting = false;
	private float boostTimeMax = .30f, boostTimer = 0f, boostSpeed = .45f;

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

		if (state.Colliding()) {
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
	private float zMin, zMax;

	public Transform groundChecker;
	public Transform wallCheckerTop;
	public Transform wallCheckerBottom;
	public Transform wallCheckerTopBack;
	public Transform wallCheckerBottomBack;
	public Transform ceilingChecker;
	public Transform floorCornerCheckFront;
	public Transform floorCornerCheckBack;
	public Transform ceilingCornerCheckBack;
	public Transform ceilingCornerCheckFront;

	private RaycastHit2D cornerHit;

	private void Linecasts()
	{
		// Maybe I can get LinecastNonAlloc to work someday.

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
		state.onCeiling = Physics2D.Linecast (transform.position, ceilingChecker.position, 1 << LayerMask.NameToLayer ("Wall"), zMin, zMax);

		state.onWallBack = Physics2D.Linecast (wallCheckerBottomBack.position, wallCheckerBottom.position, 1 << LayerMask.NameToLayer ("Wall"), zMin, zMax);
		state.onWallFront = Physics2D.Linecast (wallCheckerBottom.position, wallCheckerTop.position, 1 << LayerMask.NameToLayer ("Wall"), zMin, zMax);

		if (!state.onCeiling && !state.onWallBack && !state.onWallFront && !state.onGround) {
			cornerHit = Physics2D.Linecast (transform.position, floorCornerCheckBack.position, 1 << LayerMask.NameToLayer ("Wall"), zMin, zMax);
			state.collidingCorner = Corner.bottomBack;

			//This code is so ugly it makes my brain spasm
			if (cornerHit.collider == null) {
				cornerHit = Physics2D.Linecast (transform.position, floorCornerCheckFront.position, 1 << LayerMask.NameToLayer ("Wall"), zMin, zMax);
				state.collidingCorner = Corner.bottomFront;
			}
			if (cornerHit.collider == null) {
				cornerHit = Physics2D.Linecast (transform.position, ceilingCornerCheckBack.position, 1 << LayerMask.NameToLayer ("Wall"), zMin, zMax);
				state.collidingCorner = Corner.topBack;
			}
			if (cornerHit.collider == null) {
				cornerHit = Physics2D.Linecast (transform.position, ceilingCornerCheckFront.position, 1 << LayerMask.NameToLayer ("Wall"), zMin, zMax);
				state.collidingCorner = Corner.topFront;
			}
			if (cornerHit.collider != null) {
				return;
			}
		}
		state.collidingCorner = Corner.noCorner;
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
		if (state.OnCorner()) {
			animState = 10;
		}
		else if (state.onGround) {
			if (currentSpeedVector.x != 0) {
				if (state.sprintButton) {
					newState = 2;
				} else {
					newState = 1;
				}
			} else {
				newState = 0;
			}
		} 
		else if (state.onWallFront) {
			if (currentSpeedVector.y != 0) {
				newState = 3;
			} else {
				newState = 6;
			}
		}
		else if (state.onCeiling) {
			if (currentSpeedVector.x != 0) {
				newState = 4;
			} else {
				newState = 7;
			}
		} 
		else {
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
					if (state.facingRight && hWarp > 0) { 
						warpVector = new Vector3 (size.x + playerWidth, 0, 0);
						FlipPlayer ();
					} else if (!state.facingRight && hWarp < 0) { 
						warpVector = new Vector3 (-(size.x + playerWidth), 0, 0);
						FlipPlayer ();
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
		state.respawnPosition = playerStartPosition.position;
		player = this.gameObject;
		staticTransform = this.transform;
	}

	public void Reset()
	{
		PlayerInputEnabled (true);
		currentSpeedVector = new Vector3 ();
		transform.position = state.respawnPosition;
	}

	//## INPUT ##//
	private static bool playerInputEnabled = true;

	public static void PlayerInputEnabled(bool enabled)
	{
		playerInputEnabled = enabled;
		if(!playerInputEnabled) {
			currentSpeedVector = new Vector3();
		}
	}

	public static Vector3 PlayerPosition()
	{
		return staticTransform.position;
	}
} 