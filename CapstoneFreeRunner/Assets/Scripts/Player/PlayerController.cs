using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
	public Transform playerStartPosition;
	public GameObject boostArrow;

	private Animator animator;

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
		public bool leanOffWall;

		public bool facingRight;

		/**
		 * true: returns 1 if facing right, -1 if facing left
		 * false: returns 1 if facing left, -1 if facing right
		 **/
		public int FacingRight(bool right) {
			return (right == facingRight) ? 1 : -1;
		}

		/**
		 * Returns true if the player is moving in the direction they are facing (true)
		 * or moving away from the direction they're facing (false)
		 **/
		public bool ValueInDirection(float value, float comparison, bool right) {
			return ((facingRight == right) ? 1 : -1) * value > comparison;
		}

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
	private bool sprintButtonDown;

	//## UPDATE ##//
	void Update () 
	{
		if (PlayerInputEnabled) 
		{
			//1: Update Input Values
			vAxis = InputWrapper.GetVerticalAxis ();
			hAxis = InputWrapper.GetHorizontalAxis ();
			vWarp = InputWrapper.GetWarpVertical ();
			hWarp = InputWrapper.GetWarpHorizontal ();
			sprintButtonDown = InputWrapper.GetSprint ();
			jumpButtonDown = InputWrapper.GetJump ();
			jumpButtonUp = InputWrapper.GetAbortJump ();

			//2: Conduct Linecasts
			Linecasts();

			//3: Handle Warping
			if ((vWarp != 0 || hWarp != 0) && !state.drained) {
				HandleWarp ();
				if (warpVector.x != 0 || warpVector.y != 0) {
					transform.position += warpVector;
					warpVector = new Vector3 ();
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
				}
				else if (state.onWallFront) {
					Animate(HandleOnWallFront ());
					if (jumpButtonDown) {
						Animate(JumpOffWalls ());
					}
				} else if (state.onWallBack) {
					Animate(HandleOnWallBack ());
				}

				if (state.onGround) {
					Animate(MoveOnGround ());
					if (jumpButtonDown) {
						Jump ();
					}
				} else if (state.onCeiling && !state.fallingOffCeiling) {
					Animate(MoveOnCeiling ());
					if (jumpButtonDown) {
						Animate(JumpOffCeiling ());
					}
				} else if (state.InAir ()) {
					MoveInAir ();
					Animate(ApplyGravity ());
					if (jumpButtonDown) {
						BoostJump ();
					}
				 }
			}

			//5: Apply Movement Vectors to Transform
			if(!boosting) {
				if (!canBoost && state.Colliding()) {
					canBoost = true;
				}
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
	//1: reverse X 			(1 : *, -1 : *)
	//2: reverse Y 			(1 : *, -1 : *)
	//3: primary direction 	(1 : x, -1 : y)
	short[] cornerMoveData = new short[4];

	private void MoveOnCorner()
	{
		currentSpeedVector = new Vector3 ();

		switch (state.collidingCorner) {
		case Corner.bottomBack:
			if (vAxis < .3f) {
				cornerMoveData = new short[4]{1, 1, -1, -1};
				FlipPlayer ();
				cornerChecker = floorCornerCheckBack.position;
				checkerX = groundChecker.position;
				checkerY = wallCheckerBottomBack.position;
			} else if (state.ValueInDirection(hAxis, .3f, false)) {
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
			} else if (state.ValueInDirection(hAxis, .3f, true)) {
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
			} else if (state.ValueInDirection(hAxis, .3f, false)) {
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
			} else if (state.ValueInDirection(hAxis, .3f, true)) {
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
			transform.position += new Vector3 (cornerMoveData[1] * distanceX * state.FacingRight(true), cornerMoveData[2] * distanceY, 0);
			cornerMoveData [0] = -1;
		}
	}

	private void MoveInAir()
	{
		currentSpeedVector.x = hAxis * runSpeed;
	}

	private AnimationState MoveOnGround()
	{
		if (!state.onWallFront)
			currentSpeedVector.y = 0;
		else if (state.ValueInDirection(hAxis, 0f, true)) {
			currentSpeedVector.x = 0;
			return AnimationState.IDLE;
		}

		if (state.ValueInDirection(hAxis, 0f, false))  
			FlipPlayer ();

		currentSpeedVector.x = hAxis * (sprintButtonDown ? sprintSpeed : runSpeed);

		if (currentSpeedVector.x == 0) {
			return AnimationState.IDLE;
		} else {
			return (sprintButtonDown ? AnimationState.RUN : AnimationState.WALK);
		}
	}

	private AnimationState MoveOnCeiling()
	{
		if (state.ValueInDirection(hAxis, 0f, false))
			FlipPlayer ();

		if (state.onWallFront) {
			currentSpeedVector.x = 0;
			return AnimationState.IDLE_CEILING;
		}

		currentSpeedVector.x = hAxis * (sprintButtonDown ? sprintSpeed : runSpeed);
		return AnimationState.CLIMB_CEILING;
	}

	private AnimationState HandleOnWallBack()
	{
		if (state.ValueInDirection(currentSpeedVector.x, 0, false)) {
			currentSpeedVector.x = 0;
			FlipPlayer ();
		}
		return AnimationState.NONE;
	}

	private AnimationState HandleOnWallFront ()
	{
		if (state.fallingOffWall) {
			if (state.ValueInDirection(hAxis, 0, true) || state.onGround || state.onCeiling) {
				state.fallingOffWall = false;
			} 
			else {
				return AnimationState.NONE;
			}
		}

		float verticalSpeed = 0;
		if (state.ValueInDirection(hAxis, .3f, false)) {
			 state.leanOffWall = true;
		} else {
			state.leanOffWall = false;
			currentSpeedVector.x = 0;
			if(!(state.onGround && vAxis < 0) && !(state.onCeiling && vAxis > 0))
				verticalSpeed = vAxis * (sprintButtonDown ? sprintSpeed : runSpeed);
		}

		currentSpeedVector.y = verticalSpeed;
		return (verticalSpeed != 0) ? AnimationState.CLIMB_WALL : AnimationState.IDLE_WALL;
	}

	//## JUMPING ##//
	private bool jumpButtonDown = false;
	private bool jumpButtonUp = false;
	private float jumpSpeed = .45f;
	private float jumpArc = .25f;

	//## FALLING ##//
	private float terminalVelocity = -.8f, gravityFactor = .02f;

	private AnimationState JumpOffCeiling()
	{
		state.fallingOffCeiling = true;
		return AnimationState.JUMP;
	}

	private AnimationState JumpOffWalls()
	{
		if (state.leanOffWall) {
			currentSpeedVector.y = jumpArc;
			currentSpeedVector.x = Mathf.Sign(hAxis) * jumpSpeed;
			return AnimationState.JUMP;
		} else {
			//transform.position += new Vector3 ((state.facingRight ? -1 : 1), 0, 0);
			//state.fallingOffWall = true;
		}
		return AnimationState.NONE;
	}

	private AnimationState Jump()
	{
		if (state.onGround) {
			currentSpeedVector.y = jumpSpeed;
			return AnimationState.JUMP;
		}
		return AnimationState.NONE;
	}

	private AnimationState ApplyGravity()
	{
		currentSpeedVector.y -= gravityFactor;

		if (currentSpeedVector.y > 0 && jumpButtonUp) {
			currentSpeedVector.y /= 2;
		}
		if (currentSpeedVector.y < terminalVelocity)
			currentSpeedVector.y = terminalVelocity;

		if (state.fallingOffCeiling) {
			state.fallingOffCeiling = false;
		} 
		else if (state.fallingOffWall) {
			state.fallingOffWall = false;
		}
		return AnimationState.JUMP;
	}

	//## BOOSTING ##

	private bool canBoost = true, boosting = false, chargingBoost = false;
	private float boostTimeMax = .25f, boostTimer = 0f, boostSpeed = .45f;
	private Vector2 boostDirection;

	private void BoostJump()
	{
		if (canBoost) {
			shakeAmount = .1f;
			boostTimer = boostTimeMax;
			boosting = true;
			canBoost = false;
			chargingBoost = true;
			CameraController.self.ZoomCamera (.7f, 40);
			currentSpeedVector = new Vector3 ();
			Time.timeScale = .2f;
			PlayerAudioManager.self.PlayBoostCharge ();
			boostArrow.SetActive (true);
			boostArrow.transform.rotation = Quaternion.Slerp(boostArrow.transform.rotation, Quaternion.LookRotation (Vector3.forward, new Vector3 (hAxis != 0 ?  state.FacingRight(true) * Mathf.Sign(hAxis) * 1 : 0, vAxis != 0 ? Mathf.Sign(vAxis) * 1 : 0, 0)), 1);
		}
	}

	private float shakeAmount;

	private void HandleBoost()
	{
		if (chargingBoost && jumpButtonUp) {
			if (hAxis == 0 && vAxis == 0) {
				boostDirection = new Vector2 (0, 1);
			} else {
				boostDirection = new Vector2 (hAxis, vAxis).normalized;
			}
			CameraController.self.RestoreSize (5);
			chargingBoost = false;
			Time.timeScale = 1f;
			PlayerAudioManager.self.PlayBoostRelease ();
			boostArrow.SetActive (false);
			shakeAmount = 2f;
		} else if (chargingBoost) {
			CameraController.self.ShakeCamera (shakeAmount);
			shakeAmount += .01f;
			boostArrow.transform.rotation = Quaternion.Slerp(boostArrow.transform.rotation, Quaternion.LookRotation (Vector3.forward, new Vector3 (hAxis != 0 ?  state.FacingRight(true) * Mathf.Sign(hAxis) * 1 : 0, vAxis != 0 ? Mathf.Sign(vAxis) * 1 : 0, 0)), 1);
			return;
		}

		boostTimer -= Time.deltaTime;
		if (shakeAmount > 0) {
			shakeAmount -= .05f;
			CameraController.self.ShakeCamera (shakeAmount);
		}

		if (state.Colliding()) {
			boosting = false;
			boostTimer = 0f;
			currentSpeedVector = new Vector3();
			return;
		}

		if (state.ValueInDirection(hAxis, 0, false))
			FlipPlayer ();

		currentSpeedVector = boostSpeed * boostDirection;
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
	private RaycastHit2D hit;

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

		state.onWallBack = Physics2D.Linecast (wallCheckerBottomBack.position, wallCheckerBottom.position, 1 << LayerMask.NameToLayer ("Wall"), zMin, zMax);
		state.onWallFront = Physics2D.Linecast (wallCheckerBottom.position, wallCheckerTop.position, 1 << LayerMask.NameToLayer ("Wall"), zMin, zMax);
			
		hit = Physics2D.Linecast (transform.position, groundChecker.position, 1 << LayerMask.NameToLayer ("Wall"), zMin, zMax);

		if (hit.collider != null) {
			transform.position += new Vector3 (0,Vector3.Distance(transform.position, groundChecker.position) - hit.distance,0);
			state.collidingCorner = Corner.noCorner;
			state.onGround = true;
			return;
		} else {
			state.onGround = false;
		}

		hit = Physics2D.Linecast (transform.position, ceilingChecker.position, 1 << LayerMask.NameToLayer ("Wall"), zMin, zMax);

		if (hit.collider != null) {
		//	transform.position += new Vector3 (0,Vector3.Distance(transform.position, groundChecker.position) - hit.distance,0);
			state.collidingCorner = Corner.noCorner;
			state.onCeiling = true;
			return;
		} else {
			state.onCeiling = false;
		}
			
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
	private enum AnimationState {IDLE, WALK, RUN, CLIMB_WALL, CLIMB_CEILING, JUMP, IDLE_WALL, IDLE_CEILING, NONE};
	private AnimationState animationState;
	private void Animate(AnimationState state)
	{
		if (state != AnimationState.NONE && animationState != state) {
			animator.SetInteger ("State", (int)state);
			animationState = state;
		} 
	}

	//## WARPING ##//
	private Vector3 warpVector;
	private float maxWarpDistance = 2f;

	private void HandleWarp() 
	{
		if(state.onWallFront && state.ValueInDirection(hWarp, .3f, true)) 
		{
			RaycastHit2D hit = Physics2D.Linecast (wallCheckerBottom.position, wallCheckerTop.position, 1 << LayerMask.NameToLayer ("Wall"));
			if (hit.collider != null) {
				Bounds hitBounds = hit.collider.bounds;
				Vector3 size = hitBounds.size;

				if (size.x <= maxWarpDistance) {
					warpVector = new Vector3 (state.FacingRight(true) * (size.x + playerWidth), 0, 0);
					if(!state.onGround) FlipPlayer ();
				}
			}
		}
		else if(state.onGround && vWarp < 0)
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


	//## INITIALIZING THE CONTROLLER ##//
	void Start()
	{
		animator = this.GetComponent<Animator>();
		state = new PlayerState ();
		state.facingRight = true;
		Reset ();
		state.respawnPosition = playerStartPosition.position;
		PlayerTransform = this.transform;
		Player = this.gameObject;
	}

	public void Reset()
	{
		PlayerInputEnabled = true;
		currentSpeedVector = new Vector3 ();
		transform.position = state.respawnPosition;
	}

	//## INPUT ##//
	private static bool _playerInputEnabled = true;
	public static bool PlayerInputEnabled
	{
		get { return _playerInputEnabled; } 
		set {_playerInputEnabled = value; if (!_playerInputEnabled)
				currentSpeedVector = new Vector3 ();}
	}
		
	//## STATIC REFERENCES TO PLAYER ##//
	private static Transform _playerTransform;
	public static Transform PlayerTransform
	{
		set { if(_playerTransform == null) _playerTransform = value; }
	}
	public static Vector3 PlayerPosition {
		get { return _playerTransform.position; }
	}

	private static GameObject _player;
	public static GameObject Player
	{
		set { if(_player == null) _player = value; }
		get { return _player; }
	}
} 