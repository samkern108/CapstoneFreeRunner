using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
	public static float playerZLayer;

	public GameObject sprite;

	public Transform playerStartPosition;
	public Transform raycastParent;

	public ParticleSystem boostParticle;
    public GameObject warpParticleEmitter1;
    public GameObject warpParticleEmitter2;

    private Animator animator;

	public enum Corner {topFront, topBack, bottomFront, bottomBack, noCorner};
	public static PlayerState state;
	public class PlayerState
	{
		//## POSITION ##//
		public Vector3 respawnPosition;
		public Vector3 currentSpeedVector;

		//## RAYCASTING ##//
		public bool onWallBack;
		public bool onWallFront;
		public bool onCeiling;
		public bool onGround;

		public Corner collidingCorner;

		public bool falling;
		public bool boosting;
		public bool sprintJump = false;

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
			return !Colliding () || falling;
		}
	}

	private float playerWidth, playerHeight;

	//## CURRENT INPUT VALUES ##//
	private float hAxis = 0, vAxis = 0, hWarp = 0, vWarp = 0;
	private bool sprintButtonDown, jumpButtonDown, jumpButtonUp, warpButtonDown;

	private Vector3 intermediatePosition;

	//## UPDATE ##//
	void Update () 
	{
		if (PlayerInputEnabled) 
		{
			Linecasts ();

			intermediatePosition = transform.position;

			//2: Update Input Values
			vAxis = InputWrapper.GetVerticalAxis ();
			hAxis = InputWrapper.GetHorizontalAxis ();
			vWarp = InputWrapper.GetWarpVertical ();
			hWarp = InputWrapper.GetWarpHorizontal ();
			warpButtonDown = InputWrapper.GetWarpButton ();
			sprintButtonDown = InputWrapper.GetSprint ();
			jumpButtonDown = InputWrapper.GetJump ();
			jumpButtonUp = InputWrapper.GetAbortJump ();

			//3: Handle Warping
			if (warpButtonDown && !state.drained && !state.InAir()) {
				if (HandleWarp ()) return;
			}
				
			if (state.sprintJump && !state.InAir ()) {
				state.sprintJump = false;
			}

			//4: Handle Regular Movement
			if(state.boosting) {
				Animate(HandleBoost());
			}
			else {
				if (state.OnCorner()) {
					Animate(MoveOnCorner ());
					transform.position = intermediatePosition;
//					Linecasts ();
					//return;
				}
				else if (state.onWallFront) {
					if (jumpButtonDown) {
						Animate (JumpOffWalls ()); 
						//return; //if we have the return statement in here, the player does not move in air after jumping.
					} else {
						Animate(HandleOnWallFront ());
					}
				} else if (state.onWallBack) {
					Animate(HandleOnWallBack ());
				}

				if (state.onGround) {
					if (jumpButtonDown) {
						Jump ();
					} else {
						Animate(MoveOnGround ());
					}
				} else if (state.onCeiling) {



					if (jumpButtonDown) {
						Animate (JumpOffCeiling ());
					} else {
						Animate(MoveOnCeiling ());
					}
				}
				if (state.InAir ()) {
					MoveInAir ();
					Animate(ApplyGravity ());
					if (jumpButtonDown && !(state.falling && state.Colliding())) {
						Animate(BoostJump ());
					}
				}
			}
				
			if(!canBoost && state.Colliding()) {
					canBoost = true;
			}

			//5: Apply Movement Vectors to Transform

			if(state.currentSpeedVector.magnitude != 0) 
				transform.position = CheckNewPosition (transform.position + state.currentSpeedVector * Time.deltaTime, transform.position);
		}
	}

	//## RUNNING ##//
	private float runSpeed = 12f, sprintSpeed = 24f;

	//## JUMPING ##//
	private float jumpSpeed = 50f;
	private float jumpArc = 40f;
	private float wallJumpArc = 40f;
	private float lowJumpArc = 30f;
	private float jumpSpeedSprint = 60f;

	//## FALLING ##//
	private float terminalVelocity = -28f, gravityFactor = 170f;

	//## BOOSTING ##//
	private float boostTimeMax = .2f, boostTimer = 0f, boostSpeed = 35f;

	float x, y;
	Vector2 cornerChecker;

	short[] cornerMoveData = new short[4];

	private AnimationState MoveOnCorner()
	{
		AnimationState anim = AnimationState.JUMP;
		state.currentSpeedVector = Vector3.zero;

		if (state.falling) return anim;

		switch (state.collidingCorner) {
		case Corner.bottomBack:
			if (jumpButtonDown) {
				Jump();
			} 
			//move down a cliff while looking at it
			else if (vAxis < -.15f) { 
				FlipPlayer ();
				cornerMoveData = new short[4]{ 1, 1, 1, -1 };
				cornerChecker = floorCornerCheckFront.position;
			} 
			//move backwards while looking down a cliff
			else if (state.ValueInDirection (hAxis, .15f, false)) { 
				cornerMoveData = new short[4]{ 1, 1, 1, 1 }; //x : -1
				cornerChecker = floorCornerCheckBack.position;
			} 
			anim = AnimationState.FLOOR_CORNER;
			break;
		case Corner.bottomFront:
			if (jumpButtonDown) {
				JumpOffWalls ();
			} 
			//move down a wall while clinging to the top of it
			else if (vAxis < -.15f) {
				cornerMoveData = new short[4]{ 1, -1, 1, -1 };
			} 
			//move up and over the corner of a cliff
			else if (state.ValueInDirection (hAxis, .15f, true)) {
				cornerMoveData = new short[4]{ 1, -1, 1, 1 }; //x : 1
			}
			cornerChecker = floorCornerCheckFront.position;
			anim = AnimationState.WALL_UP_CORNER;
			break;
		case Corner.topBack:
			if (jumpButtonDown) {
				JumpOffCeiling ();
			} 
			//move up onto a wall when you're climbing on the ceiling
			else if (vAxis > .15f) {
				FlipPlayer ();
				cornerMoveData = new short[4]{ 1, 1, -1, -1 };
				cornerChecker = ceilingCornerCheckFront.position;
			} 
			//move backwards on the ceiling
			else if (state.ValueInDirection (hAxis, .15f, false)) {
				cornerMoveData = new short[4]{ 1, 1, -1, 1 }; //x: -1
				cornerChecker = ceilingCornerCheckBack.position;
			}
			anim = AnimationState.CEILING_CORNER;
			break;
		case Corner.topFront:
			if (jumpButtonDown) {
				JumpOffWalls ();
			}
			//move up on the wall while clinging to a low corner
			else if (vAxis > .15f) {
				cornerMoveData = new short[4]{ 1, -1, -1, -1 };
			} 
			//move under wall while clinging to it.
			else if (state.ValueInDirection (hAxis, .15f, true)) {
				cornerMoveData = new short[4]{ 1, -1, -1, 1 }; // x: 1
			}
			cornerChecker = ceilingCornerCheckFront.position;
			anim = AnimationState.WALL_DOWN_CORNER;
			break;
		}

		if(cornerMoveData[0] == 1) {
			RaycastHit2D hit = Physics2D.Linecast (transform.position, cornerChecker, layerMask);

			float moveX = transform.position.x;
			float moveY = transform.position.y;

			//0: move				(1 : yes, -1 : no)
			//1: X					(1 : right, -1 : left)	which side of the collider should we move the player to?
			//2: Y					(1 : top, -1 : bottom)
			//3: primary direction	(1 : x, -1 : y)

			switch(cornerMoveData[3])
			{
			case 1: //moving primarily in x
				if (hit.collider != null)
					moveX = hit.collider.transform.position.x + (state.FacingRight(true) * cornerMoveData[1]) * (hit.collider.bounds.size.x / 2 - playerWidth/8);
				else {
					Debug.Log ("Collider x is null");
				}
				break;
			case -1: //moving primarily in y
				if (hit.collider != null) {
					moveY = hit.collider.transform.position.y + cornerMoveData [2] * (hit.collider.bounds.size.y / 2);
				} else {
					Debug.Log ("Collider y is null for corner check: " + state.collidingCorner);
				}
				break;
			}
			intermediatePosition.x = moveX;
			intermediatePosition.y = moveY;			
			cornerMoveData [0] = -1;
		}
		return anim;
	}

	private void MoveInAir()
	{
		if (state.ValueInDirection(hAxis, 0f, false))  
			FlipPlayer ();
		
		state.currentSpeedVector.x = hAxis * (state.sprintJump ? sprintSpeed : runSpeed);
	}

	private AnimationState MoveOnGround()
	{
		 state.falling = false;

		if (!state.onWallFront)
			state.currentSpeedVector.y = 0;
		else if (state.ValueInDirection(hAxis, 0f, true)) {
			state.currentSpeedVector.x = 0;
			return AnimationState.IDLE;
		}

		if (state.ValueInDirection(hAxis, 0f, false))  
			FlipPlayer ();

		state.currentSpeedVector.x = hAxis * (sprintButtonDown ? sprintSpeed : runSpeed);

		if (Mathf.Abs(state.currentSpeedVector.x) < .05f) {
			return AnimationState.IDLE;
		} else {
			return (sprintButtonDown ? AnimationState.RUN : AnimationState.WALK);
		}
	}

	private AnimationState MoveOnCeiling()
	{
		if (state.currentSpeedVector.y > 0)
			state.currentSpeedVector.y = 0;
		
		if (state.ValueInDirection(hAxis, 0f, false))
			FlipPlayer ();

		if (state.onWallFront) {
			state.currentSpeedVector.x = 0;
			return AnimationState.IN_CORNER;
		}

		state.currentSpeedVector.x = hAxis * (sprintButtonDown ? (sprintSpeed - .1f) : runSpeed);

		return Mathf.Abs(state.currentSpeedVector.x) < .05f ? AnimationState.IDLE_CEILING : AnimationState.CLIMB_CEILING;
	}

	private AnimationState HandleOnWallBack()
	{
		if(!state.onGround && !state.onCeiling) {
			FlipPlayer ();
		}
		//if the player's back is to the wall and they try to move backwards "through" it, we stop their motion and flip them.
		else if (state.ValueInDirection (state.currentSpeedVector.x, 0, false)) {
			state.currentSpeedVector.x = 0;
			FlipPlayer ();
		}
		return AnimationState.NONE;
	}

	private AnimationState HandleOnWallFront ()
	{
		if (state.falling) {
			if (state.ValueInDirection(hAxis, .2f, true)) {
				state.falling = false;
			} 
			else return AnimationState.NONE;
		}

		float verticalSpeed = 0;

		if (state.ValueInDirection(hAxis, .3f, false)) {
			state.leanOffWall = true;
		} else {
			state.leanOffWall = false;
			state.currentSpeedVector.x = 0;
			if (!(state.onGround && vAxis < 0) && !(state.onCeiling && vAxis > 0)) {
				verticalSpeed = vAxis * (sprintButtonDown ? sprintSpeed : (runSpeed - .1f));
			}
		}

		state.currentSpeedVector.y = verticalSpeed;
		return (Mathf.Abs(verticalSpeed) >= .05f) ? AnimationState.CLIMB_WALL : AnimationState.IDLE_WALL;
	}

	private AnimationState JumpOffCeiling()
	{
		if (sprintButtonDown)
			state.sprintJump = true;
		state.falling = true;
		return AnimationState.JUMP;
	}

	private AnimationState JumpOffWalls()
	{
		if (InputWrapper.isGamepadConnected) {
            state.currentSpeedVector.y = wallJumpArc; //* vAxis;
		} else {
			if (vAxis > .1f)
				state.currentSpeedVector.y = wallJumpArc;
			else if (vAxis < -.1f)
				state.currentSpeedVector.y = 0;
			else
				state.currentSpeedVector.y = lowJumpArc;
		}
		if (sprintButtonDown)
			state.sprintJump = true;

		state.currentSpeedVector.x = state.FacingRight(false) * (sprintButtonDown ? jumpSpeedSprint : jumpSpeed);

		return AnimationState.JUMP;
	}

	private AnimationState Jump()
	{
		if (sprintButtonDown)
			state.sprintJump = true;
		state.currentSpeedVector.y = jumpArc;
		return AnimationState.JUMP;
	}

	private AnimationState ApplyGravity()
	{
		state.currentSpeedVector.y -= gravityFactor * Time.deltaTime;

		if (state.currentSpeedVector.y < terminalVelocity)
			state.currentSpeedVector.y = terminalVelocity;

		return AnimationState.JUMP;
	}

	//## BOOSTING ##

	private bool canBoost = true;
	private Vector2 boostDirection;
	private float shakeAmount;

	private AnimationState BoostJump()
	{
		if (canBoost) {
			state.falling = false;
			boostTimer = boostTimeMax;
			state.boosting = true;
			canBoost = false;
			state.currentSpeedVector = Vector3.zero;

			if (hAxis == 0 && vAxis == 0)
				boostDirection = Vector2.up;
			else 
				boostDirection = new Vector2 (hAxis, vAxis).normalized;

			PlayerAudioManager.self.PlayBoostRelease ();

			if (!boostParticle.gameObject.activeSelf)
				boostParticle.gameObject.SetActive (true);
			
			return AnimationState.BOOST;
		}
		return AnimationState.NONE;
	}

	private AnimationState HandleBoost()
	{
		boostTimer -= Time.deltaTime;

		if (state.Colliding()) {
			StopBoosting ();
			state.currentSpeedVector = Vector3.zero;
			return AnimationState.NONE;
		}

		if (hAxis != 0 && vAxis != 0)
			boostDirection = new Vector2 (hAxis, vAxis).normalized;

		if (state.ValueInDirection (hAxis, 0, false))
			FlipPlayer ();

		state.currentSpeedVector = boostSpeed * boostDirection;

		//Vector3 lookAt = state.currentSpeedVector.normalized;

		//float rot_z = Mathf.Atan2(lookAt.y, lookAt.x) * Mathf.Rad2Deg;
		//rot_z += ((Mathf.Sign (transform.localScale.x) == 1) ? 0 : 90);
		//rot_z += ((Mathf.Sign (transform.localScale.x) == 1) ? 0 : 180);
		//sprite.transform.rotation = Quaternion.Euler (0f, 0f, rot_z);

		//boostParticle.transform.rotation = Quaternion.Euler(0f, 0f, rot_z);
		//boostParticle.startRotation = rot_z;// + ((Mathf.Sign(transform.localScale.x) == 1) ? 0 : 180);//Quaternion.Euler(0f, 0f, rot_z + ((Mathf.Sign(boostParticle.transform.localScale.x) == 1) ? 0 : 180) );

		if (boostTimer <= 0) {
			StopBoosting ();
		}
		return AnimationState.BOOST;
	}

	private void StopBoosting()
	{
		state.boosting = false;
		boostParticle.gameObject.SetActive(false);
		//sprite.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
	}

	private void FlipPlayer()
	{
		state.facingRight = !state.facingRight;
		Vector3 scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;

		scale = boostParticle.transform.localScale;
		scale.x *= -1;
		boostParticle.transform.localScale = scale;
	}

	//## COLLISION CHECKING ##//

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

	private Vector3 CheckNewPosition(Vector3 newPos, Vector3 oldPos)
	{
		Vector2 dir = (newPos - oldPos).normalized;
		Vector2 playerDimensions = new Vector2 (dir.x * (playerWidth/2), dir.y * (playerHeight/2));
		float distance = Vector2.Distance (newPos, oldPos) + playerDimensions.magnitude;

		RaycastHit2D raycast = Physics2D.Raycast (oldPos, dir, distance + playerDimensions.magnitude, layerMask);

		if (raycast.collider == null || raycast.distance > distance) {
			return newPos;
		} else {
			//Debug.DrawRay(oldPos, dir * 100, (raycast.collider == null) ? Color.white : Color.red, 5f, false);

			//Debug.Log (raycast.distance + "  " + distance);
			//Debug.Log (newPos + "  " + oldPos + "  " + (newPos - oldPos).magnitude + "  " + dir);
			//Debug.Log (playerDimensions + "  " + playerDimensions.magnitude);*/

			return raycast.point + new Vector2(-playerDimensions.x, -playerDimensions.y);
		}
	}

	// Maybe I can get LinecastNonAlloc to work someday.
	private void Linecasts()
	{
		state.onWallBack = Physics2D.Linecast (wallCheckerBottomBack.position, wallCheckerBottom.position, layerMask);

		hit = Physics2D.Linecast (wallCheckerBottom.position, wallCheckerTop.position, layerMask);
		if (hit.collider != null) {
			state.onWallFront = true;
		} else  
			state.onWallFront = false;

		hit = Physics2D.Linecast (transform.position, groundChecker.position, layerMask);

		if (hit.collider != null) 
		{
			state.collidingCorner = Corner.noCorner;
			state.onGround = true;
			state.onCeiling = false;
			return;
		} 
		else  state.onGround = false;

		hit = Physics2D.Linecast (transform.position, ceilingChecker.position, layerMask);

		if (hit.collider != null) 
		{
			state.collidingCorner = Corner.noCorner;
			state.onCeiling = true;
			return;
		} 
		else  state.onCeiling = false;

		if (!state.onWallBack && !state.onWallFront) {

			if(Physics2D.Linecast (transform.position, floorCornerCheckBack.position, layerMask)) {
				state.collidingCorner = Corner.bottomBack;	return;
			}
			else if(Physics2D.Linecast (transform.position, floorCornerCheckFront.position, layerMask)) {
				state.collidingCorner = Corner.bottomFront;	return;
			}
			else if(Physics2D.Linecast (transform.position, ceilingCornerCheckBack.position, layerMask)) {
				state.collidingCorner = Corner.topBack;		return;
			}
			else if(Physics2D.Linecast (transform.position, ceilingCornerCheckFront.position, layerMask)) {
				state.collidingCorner = Corner.topFront;	return;
			}
		}
		state.collidingCorner = Corner.noCorner;
		return;
	}

	/*void OnCollisionEnter2D (Collision2D collision) {
		float z = collision.transform.position.z;
		if (z < zMin || z > zMax) {
			Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
		}
	}*/


	//## ANIMATION ##//
	private enum AnimationState {IDLE, WALK, RUN, CLIMB_WALL, CLIMB_CEILING, JUMP, IDLE_WALL, IDLE_CEILING, IN_CORNER, FLOOR_CORNER, CEILING_CORNER, WALL_DOWN_CORNER, WALL_UP_CORNER, BOOST, NONE};
	private AnimationState animationState;
	private void Animate(AnimationState state)
	{
		if (state != AnimationState.NONE && animationState != state) {
			animator.SetInteger ("State", (int)state);
			animationState = state;
		} 
	}

	//## WARPING ##//

    private void WarpEffect() 
	{
        warpParticleEmitter1.GetComponent<ParticleSystem>().Emit(25);
        warpParticleEmitter2.GetComponent<ParticleSystem>().Emit(50);
    }

    private bool HandleWarp() 
	{
		Vector3 checker1 = new Vector3(), checker2 = new Vector3();
		bool warpVertical = true;
		int onGround = 0;
	
		if(state.onWallFront) 
		{
			checker1 = wallCheckerBottom.position;
			checker2 = wallCheckerTop.position;
			warpVertical = false;
		}
		if(state.onGround)
		{
			if (warpVertical || (!warpVertical && (Mathf.Abs (hAxis) < Mathf.Abs (vAxis)))) {
				checker1 = transform.position;
				checker2 = groundChecker.position;
				onGround = -1;	
				warpVertical = true;
			}
		}
		if(state.onCeiling)
		{
			if (warpVertical || (!warpVertical && (Mathf.Abs (hAxis) < Mathf.Abs (vAxis)))) {
				checker1 = transform.position;
				checker2 = ceilingChecker.position;
				onGround = 1;
				warpVertical = true;
			}
		}

		if (checker1 != Vector3.zero) {
			RaycastHit2D hit = Physics2D.Linecast (checker1, checker2, layerMask);
			if (hit.collider.tag != "Warp") {
				return false;
			}

			if (hit.collider != null) {
				Wall wall = hit.collider.GetComponent <Wall> ();

				if (!warpVertical) {
					//if the player is moving quickly toward a ceiling or floor, don't warp them through the wall.
					bool verticalCheck = Physics2D.Raycast(transform.position, Vector2.up * Mathf.Sign(hAxis), Mathf.Abs(hAxis), layerMask);
					if(verticalCheck) {
						return false;
					}

					Vector3 newPos = wall.WarpHorizontal (transform.position, state.FacingRight (true), playerWidth, playerZLayer);

					if(transform.position != newPos) {
						transform.position = newPos;
						FlipPlayer ();
						WarpEffect();
						return true;
					}
				} else {
					//if the player is moving quickly toward a vertical wall, don't warp them through the floor.
					bool horizontalCheck = Physics2D.Raycast(transform.position, Vector2.right * state.FacingRight(true), Mathf.Abs(hAxis), layerMask);
					if(horizontalCheck) {
						return false;
					}

					Vector3 newPos = wall.WarpVertical (transform.position, onGround, playerHeight, playerZLayer);

					if(transform.position != newPos) {
						transform.position = newPos;
						WarpEffect();
						return true;
					}
				}
			}
		}
		return false;
	}


	//## INITIALIZING THE CONTROLLER ##//
	void Awake()
	{
		state = new PlayerState ();
		animator = this.GetComponentInChildren<Animator>();
		state.facingRight = false;
		state.respawnPosition = playerStartPosition.position;
		PlayerTransform = this.transform;
		Player = this.gameObject;
		playerHeight = Vector2.Distance (groundChecker.position, ceilingChecker.position);
		playerWidth = Vector2.Distance (wallCheckerTop.position, wallCheckerTopBack.position);
		playerZLayer = transform.position.z;
		layerMask = 1 << LayerMask.NameToLayer ("Wall");

		Reset ();
	}

	int layerMask;

	public void Reset()
	{
		state.currentSpeedVector = Vector3.zero;
		state.boosting = false;

		RaycastHit2D hit = Physics2D.Raycast (state.respawnPosition, Vector2.down, 100, layerMask);
		transform.position = new Vector3(hit.point.x, hit.point.y + playerHeight/2, playerZLayer);

		PlayerInputEnabled = true;

		Animate (AnimationState.IDLE);
		boostParticle.gameObject.SetActive (false);
		state.drained = false;
	}

	//## INPUT ##//
	private static bool _playerInputEnabled = true;
	public static bool PlayerInputEnabled
	{
		get { return _playerInputEnabled; } 
		set {_playerInputEnabled = value; if (!_playerInputEnabled)
			state.currentSpeedVector = Vector3.zero;}
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