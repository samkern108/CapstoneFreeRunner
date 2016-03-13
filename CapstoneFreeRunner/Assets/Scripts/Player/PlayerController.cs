using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
	public Transform playerStartPosition;
	public Transform raycastParent;

	public GameObject boostParticle;
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
	private bool sprintButtonDown, jumpButtonDown, jumpButtonUp;

	private Vector3 intermediatePosition;

	//## UPDATE ##//
	void Update () 
	{
		if (PlayerInputEnabled) 
		{
			intermediatePosition = transform.position;

			//2: Update Input Values
			vAxis = InputWrapper.GetVerticalAxis ();
			hAxis = InputWrapper.GetHorizontalAxis ();
			vWarp = InputWrapper.GetWarpVertical ();
			hWarp = InputWrapper.GetWarpHorizontal ();
			sprintButtonDown = InputWrapper.GetSprint ();
			jumpButtonDown = InputWrapper.GetJump ();
			jumpButtonUp = InputWrapper.GetAbortJump ();

			//3: Handle Warping
			if ((vWarp != 0 || hWarp != 0) && !state.drained) {// && !state.InAir()) {
				if (HandleWarp ()) return;
			}

			//4: Handle Regular Movement
			if(state.boosting) {
				Animate(HandleBoost());
			}
			else {
				if (state.OnCorner()) {
					Animate(MoveOnCorner ());
					transform.position = intermediatePosition;
					Linecasts (transform.position);
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
			intermediatePosition = transform.position + state.currentSpeedVector * Time.deltaTime;
			raycastParent.position = intermediatePosition;
			Linecasts (raycastParent.position);
			transform.position = intermediatePosition;
			raycastParent.position = transform.position;
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
	Vector2 checkerX, checkerY, cornerChecker;

	short[] cornerMoveData = new short[4];

	private AnimationState MoveOnCorner()
	{
		AnimationState anim = AnimationState.JUMP;
		state.currentSpeedVector = new Vector3 ();

		if (state.falling) return anim;

		switch (state.collidingCorner) {
		case Corner.bottomBack:
			if (jumpButtonDown) {
				Jump();
			} else if (vAxis < -.3f) {
				cornerMoveData = new short[4]{ 1, 1, -1, -1 };
				FlipPlayer ();
			} else if (state.ValueInDirection (hAxis, .3f, false)) {
				cornerMoveData = new short[4]{ 1, -1, 1, 1 };
			} 
			cornerChecker = floorCornerCheckBack.position;
			anim = AnimationState.FLOOR_CORNER;
			break;
		case Corner.bottomFront:
			if (jumpButtonDown) {
				JumpOffWalls ();
			} 
			else if (vAxis < -.3f) { //(???)
				cornerMoveData = new short[4]{ 1, -1, -1, -1 };
			} else if (state.ValueInDirection (hAxis, .3f, true)) {
				cornerMoveData = new short[4]{ 1, 1, 1, 1 };
			}
			cornerChecker = floorCornerCheckFront.position;
			anim = AnimationState.WALL_UP_CORNER;
			break;
		case Corner.topBack:
			if (jumpButtonDown) {
				JumpOffCeiling ();
			} else if (vAxis > .3f) {
				FlipPlayer ();
				cornerMoveData = new short[4]{ 1, 1, 1, -1 };
			} else if (state.ValueInDirection (hAxis, .3f, false)) {
				cornerMoveData = new short[4]{ 1, -1, -1, 1 };
			}

			cornerChecker = ceilingCornerCheckBack.position;
			anim = AnimationState.CEILING_CORNER;
			break;
		case Corner.topFront:
			if (jumpButtonDown) {
				JumpOffWalls ();
			}
			else if (vAxis > .3f) {
				cornerMoveData = new short[4]{ 1, -1, 1, -1 };
			} else if (state.ValueInDirection (hAxis, .3f, true)) {
				cornerMoveData = new short[4]{ 1, 1, -1, 1 };
			}
			cornerChecker = ceilingCornerCheckFront.position;
			anim = AnimationState.WALL_DOWN_CORNER;
			break;
		}

		if(cornerMoveData[0] == 1) {
			RaycastHit2D hit = Physics2D.Linecast (transform.position, cornerChecker, 1 << LayerMask.NameToLayer ("Wall"));

			float moveX = transform.position.x;
			float moveY = transform.position.y;

			//0: move 				(1 : yes, -1 : no)
			//1: pos X 				(1 : +, -1 : -)
			//2: pos Y 				(1 : +, -1 : -)
			//3: primary direction 	(1 : x, -1 : y)

			switch(cornerMoveData[3])
			{
			case 1: //moving primarily in x
				if (hit.collider != null)
					moveX = hit.collider.transform.position.x + (state.FacingRight(false) * cornerMoveData[1]) * (hit.collider.bounds.size.x / 2);
				Debug.Log (hit.collider.name + "  " + hit.collider.transform.position.x + " " + hit.collider.bounds.size.x);
				break;
			case -1: //moving primarily in y
				if(hit.collider != null)
					moveY = hit.collider.transform.position.y + cornerMoveData[2] * (hit.collider.bounds.size.y/2);
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
		state.currentSpeedVector.x = hAxis * (sprintButtonDown ? sprintSpeed : runSpeed);
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
		state.falling = true;
		return AnimationState.JUMP;
	}

	private AnimationState JumpOffWalls()
	{
		if (InputWrapper.isGamepadConnected) {
			state.currentSpeedVector.y = wallJumpArc * vAxis;
			state.currentSpeedVector.x = state.FacingRight(false) * (sprintButtonDown ? jumpSpeedSprint : jumpSpeed);
		} else {
			if (vAxis > .1f) {
				state.currentSpeedVector.y = wallJumpArc;
			} else if (vAxis < -.1f) {
				state.currentSpeedVector.y = 0;
			} else {
				state.currentSpeedVector.y = lowJumpArc;
			}
			state.currentSpeedVector.x = state.FacingRight(false) * (sprintButtonDown ? jumpSpeedSprint : jumpSpeed);
		}
		return AnimationState.JUMP;
	}

	private AnimationState Jump()
	{
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
			state.currentSpeedVector = new Vector3 ();

			if (hAxis == 0 && vAxis == 0) {
				boostDirection = new Vector2 (0, 1);
			} else {
				boostDirection = new Vector2 (hAxis, vAxis).normalized;
			}
			PlayerAudioManager.self.PlayBoostRelease ();
			return AnimationState.BOOST;
		}
		return AnimationState.NONE;
	}

	private AnimationState HandleBoost()
	{
		boostTimer -= Time.deltaTime;

		if (state.Colliding()) {
			state.boosting = false;
			boostParticle.SetActive(false);
			state.currentSpeedVector = new Vector3();
			return AnimationState.NONE;
		}

		if (hAxis != 0 && vAxis != 0) {
			boostDirection = new Vector2 (hAxis, vAxis).normalized;
		}

		if (state.ValueInDirection (hAxis, 0, false))
			FlipPlayer ();

		if (!boostParticle.activeSelf) {
			boostParticle.SetActive (true);
		}

		state.currentSpeedVector = boostSpeed * boostDirection;
		if (boostTimer <= 0) {
			state.boosting = false;
			boostParticle.SetActive(false);
		}
		return AnimationState.BOOST;
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

	// Maybe I can get LinecastNonAlloc to work someday.
	private void Linecasts(Vector3 middle)
	{
		zMin = 15;
		zMax = 40;

		state.onWallBack = Physics2D.Linecast (wallCheckerBottomBack.position, wallCheckerBottom.position, 1 << LayerMask.NameToLayer ("Wall"), zMin, zMax);

		hit = Physics2D.Linecast (wallCheckerBottom.position, wallCheckerTop.position, 1 << LayerMask.NameToLayer ("Wall"), zMin, zMax);
		if (hit.collider != null) {

			//anticipatedPos.x = hit.transform.position.x + state.FacingRight(false) * (hit.collider.bounds.size.x/2 + playerWidth/2);

			state.onWallFront = true;
		} else  
			state.onWallFront = false;

		hit = Physics2D.Linecast (middle, groundChecker.position, 1 << LayerMask.NameToLayer ("Wall"), zMin, zMax);

		if (hit.collider != null) {
			//transform.position += new Vector3 (0,(Vector3.Distance(transform.position, groundChecker.position) - hit.distance),0);
			//transform.position = new Vector3(transform.position.x, hit.collider.transform.position.y + hit.collider.bounds.size.y/2, transform.position.z);

			//anticipatedPos.y = ;

			state.collidingCorner = Corner.noCorner;
			state.onGround = true;
			state.onCeiling = false;
			return;
		} else  
			state.onGround = false;

		hit = Physics2D.Linecast (middle, ceilingChecker.position, 1 << LayerMask.NameToLayer ("Wall"), zMin, zMax);

		if (hit.collider != null) {
			//transform.position -= new Vector3 (0,(Vector3.Distance(transform.position, ceilingChecker.position) - hit.distance),0);
			//transform.position = new Vector3(transform.position.x, , transform.position.z);

			state.collidingCorner = Corner.noCorner;
			state.onCeiling = true;
			return;
		} else  
			state.onCeiling = false;

		if (!state.onCeiling && !state.onWallBack && !state.onWallFront && !state.onGround) {
			cornerHit = Physics2D.Linecast (middle, floorCornerCheckBack.position, 1 << LayerMask.NameToLayer ("Wall"), zMin, zMax);
			state.collidingCorner = Corner.bottomBack;

			//This code is so ugly it makes my brain spasm
			if (cornerHit.collider == null) {
				cornerHit = Physics2D.Linecast (middle, floorCornerCheckFront.position, 1 << LayerMask.NameToLayer ("Wall"), zMin, zMax);
				state.collidingCorner = Corner.bottomFront;
			}
			if (cornerHit.collider == null) {
				cornerHit = Physics2D.Linecast (middle, ceilingCornerCheckBack.position, 1 << LayerMask.NameToLayer ("Wall"), zMin, zMax);
				state.collidingCorner = Corner.topBack;
			}
			if (cornerHit.collider == null) {
				cornerHit = Physics2D.Linecast (middle, ceilingCornerCheckFront.position, 1 << LayerMask.NameToLayer ("Wall"), zMin, zMax);
				state.collidingCorner = Corner.topFront;
			}
			if (cornerHit.collider != null) {
				return;
			}
		}
		state.collidingCorner = Corner.noCorner;
		return;
	}

	void OnCollisionEnter2D (Collision2D collision) {
		float z = collision.transform.position.z;
		if (z < zMin || z > zMax) {
			Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
		}
	}


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
	private float maxWarpDistance = 2f;

    private void WarpEffect() {
        /*
        switch (angle) {
            case 0:
            warpParticleEmitter1.transform.localPosition = new Vector3(0.5f, 0, 0);
            warpParticleEmitter2.transform.localPosition = new Vector3(0.5f, 0, 0);
            break;
            case 90:
            warpParticleEmitter1.transform.localPosition = new Vector3(0, -0.5f, 0);
            warpParticleEmitter2.transform.localPosition = new Vector3(0, -0.5f, 0);
            break;
            case 180:
            warpParticleEmitter1.transform.localPosition = new Vector3(0.5f, 0, 0);
            warpParticleEmitter2.transform.localPosition = new Vector3(0.5f, 0, 0);
            break;
            case 270:
            warpParticleEmitter1.transform.localPosition = new Vector3(0, 0.5f, 0);
            warpParticleEmitter2.transform.localPosition = new Vector3(0, 0.5f, 0);
            break;

        }
        */
        //warpParticleEmitter1.transform.rotation = Quaternion.Euler(new Vector3(angle, 90, 0));
        //warpParticleEmitter2.transform.rotation = Quaternion.Euler(new Vector3(angle, 90, 0));
        warpParticleEmitter1.GetComponent<ParticleSystem>().Emit(25);
        warpParticleEmitter2.GetComponent<ParticleSystem>().Emit(50);
    }

    private bool HandleWarp() 
	{
		Vector3 checker1 = new Vector3(), checker2 = new Vector3();
		bool warpVertical = true;

		if(state.onWallFront && state.ValueInDirection(hWarp, .3f, true)) 
		{
			checker1 = wallCheckerBottom.position;
			checker2 = wallCheckerTop.position;
			warpVertical = false;
		}
		else if(state.onGround && vWarp < 0)
		{
			checker1 = transform.position;
			checker2 = groundChecker.position;
		}
		else if(state.onCeiling && vWarp > 0)
		{
			checker1 = transform.position;
			checker2 = ceilingChecker.position;
		}

		if (checker1 != new Vector3 ()) {
			RaycastHit2D hit = Physics2D.Linecast (checker1, checker2, 1 << LayerMask.NameToLayer ("Wall"));

			if (hit.collider != null) {
				Bounds hitBounds = hit.collider.bounds;
				Vector3 size = hitBounds.size;

				if (!warpVertical) {
					if (size.x <= maxWarpDistance) {
                       
                        transform.position = new Vector3 (hitBounds.center.x + state.FacingRight (true) * (size.x / 2 + (playerWidth - .2f) / 2), transform.position.y, transform.position.z);
                        WarpEffect();
                        bool flip = false;//Physics2D.Linecast (transform.position, ceilingChecker.position, 1 << LayerMask.NameToLayer ("Wall")) || Physics2D.Linecast (transform.position, groundChecker.position, 1 << LayerMask.NameToLayer ("Wall"));
						FlipPlayer ();
						return true;
					}
				} else {
					if (size.y <= maxWarpDistance) {
                        
                        transform.position = new Vector3 (transform.position.x, hitBounds.center.y + Mathf.Sign (vWarp) * ((size.y / 2) + ((playerHeight - .2f) / 2)), transform.position.z);
                        WarpEffect();
                        return true;
					}
				}
			}
		}
		return false;
	}


	//## INITIALIZING THE CONTROLLER ##//
	void Start()
	{
		state = new PlayerState ();
		animator = this.GetComponent<Animator>();
		state.facingRight = true;
		state.respawnPosition = playerStartPosition.position;
		PlayerTransform = this.transform;
		Player = this.gameObject;
		playerHeight = Vector2.Distance (groundChecker.position, ceilingChecker.position);
		playerWidth = Vector2.Distance (wallCheckerTop.position, wallCheckerTopBack.position);

		Reset ();
	}

	public void Reset()
	{
		Animate (AnimationState.IDLE);
		RaycastHit2D hit = Physics2D.Raycast (state.respawnPosition, Vector2.down, 100, 1 << LayerMask.NameToLayer ("Wall"));
		transform.position = new Vector3(hit.point.x, hit.point.y + playerHeight/2, transform.position.z);

		boostParticle.SetActive (false);
		PlayerInputEnabled = true;

		state.boosting = false;
		state.currentSpeedVector = new Vector3 ();
		state.drained = false;

		Linecasts(transform.position);
	}

	//## INPUT ##//
	private static bool _playerInputEnabled = true;
	public static bool PlayerInputEnabled
	{
		get { return _playerInputEnabled; } 
		set {_playerInputEnabled = value; if (!_playerInputEnabled)
			state.currentSpeedVector = new Vector3 ();}
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