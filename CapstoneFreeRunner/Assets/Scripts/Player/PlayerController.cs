using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
	public Transform playerStartPosition;

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
		public bool fallingOffWall;
		public bool onCeilingCornerBack;
		public bool onCeilingCornerFront;
		public bool onFloorCorner;

		//## WARPING ##//
		public bool drained;

		//## JUMPING AND FALLING ##//
		public bool jumping ;
		public bool leanOffWall;
		public bool sprintButton;

		public bool facingRight;

		public bool colliding()
		{
			return onGround || onCeiling || onWallFront;
		}

		public bool inAir()
		{
			return !(onGround || (onCeiling && !fallingOffCeiling) || onCeilingCornerFront || onCeilingCornerBack || (onWallFront && !fallingOffWall));
		}
	}

	private bool lastOnSide = false; // esle on ceiling

	private float playerWidth = .6f;
	private float playerHeight = 1f;

	//## CURRENT INPUT VALUES ##//
	private float hAxis = 0;
	private float vAxis = 0;

	private float warpHAxis = 0;
	private float warpVAxis = 0;

	//## UPDATE ##//
	void FixedUpdate()
	{
		if (playerInputEnabled) {
			if (warpVector.x != 0 || warpVector.y != 0) {
				warping = true;
				currentSpeedVector = new Vector2(0,0);
				transform.position += warpVector;
				warpVector = new Vector3 ();
			} else {
				warping = false;
				if(!boosting) {
					ApplyJump ();
					ApplyGravity ();
					if (!canBoost && state.colliding()) {
						canBoost = true;
					}
				}
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

			warpVAxis = InputWrapper.GetWarpVerticalAxis();
			warpHAxis = InputWrapper.GetWarpHorizontalAxis();

			state.sprintButton = InputWrapper.GetSprint ();
			jumpButtonDown = InputWrapper.GetJump ();

			Linecasts();

			if(boostTimer > 0) {
				HandleBoost();
				return;
			}

			WarpController (hAxis, vAxis, warpHAxis, warpVAxis);

			if (!warping){
				if (state.onWallFront) 
				{	
					HandleOnWallFront();
					if(jumpButtonDown) {
						JumpOffWalls ();
					}
					lastOnSide = true;
				}
				else if(state.onWallBack) {
					HandleOnWallBack();
					lastOnSide = true;
				}
				
				if(state.onGround) {
					MoveOnGround();
					if(jumpButtonDown) {
						Jump ();
					}
				}
				else if(state.onCeiling) {
					MoveOnCeiling();
					if(jumpButtonDown && vAxis < -axisActivationPoint) {
						JumpOffCeiling();
					}
					lastOnSide = false;
				}
				else if(state.onCeilingCornerBack || state.onCeilingCornerFront){
					if(lastOnSide){
						MoveOnCornerSide();
					}else{
						MoveOnCornerBottom();
					}
					if(jumpButtonDown && vAxis < -axisActivationPoint) {
						JumpOffCeiling();
					}
				}
				else if(state.inAir ())
				{
					MoveInAir();
					if(jumpButtonDown) {
						BoostJump();
					}
					return;
				}
			}
		}
	}

	//## RUNNING ##//
	private static Vector3 currentSpeedVector;
	private float sprintSpeed = .4f;
	private float runSpeed = .2f;
	private float airSpeed = .3f;

	private float axisActivationPoint = 0.15f; 
	private float wallJumpAxisActivationPoint = 0.3f;
	
	private void MoveInAir()
	{	
		if ((hAxis > 0 && !state.facingRight) || (hAxis < 0 && state.facingRight)){
			FlipPlayer ();
		}

		currentSpeedVector.x = hAxis * airSpeed;
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

		//if (state.onWallFront || state.onCeilingCornerBack) {
		//	currentSpeedVector.x = 0;
		//	return;
		//}
		
		currentSpeedVector.x = hAxis * (state.sprintButton ? sprintSpeed : runSpeed);
	}

	private void MoveOnCornerBottom(){
		if ((hAxis > 0 && !state.facingRight) || (hAxis < 0 && state.facingRight))
			FlipPlayer ();
		
		if (state.onCeilingCornerBack) {
			currentSpeedVector.x = 0;
			if (vAxis > 0){
				MoveOverCorner();
			}
		}else{
			currentSpeedVector.x = hAxis * (state.sprintButton ? sprintSpeed : runSpeed);
		}
	}
	private void MoveOnCornerSide(){
		if(state.onCeilingCornerFront){
			if (state.facingRight && hAxis > 0){
				MoveUnderCorner();
			}else if (hAxis < 0) {
				MoveUnderCorner();
			}
		}
		if (vAxis > 0){
			currentSpeedVector.y = vAxis * runSpeed;
		}else{
			currentSpeedVector.y = 0;
		}
	}

	private void MoveOverCorner(){
		float xOverCornerTransform = playerWidth / 2;
		float yOverCornerTransform = playerHeight / 2;

		transform.position += new Vector3(state.facingRight ? xOverCornerTransform : -xOverCornerTransform, yOverCornerTransform, 0);
		FlipPlayer();
		lastOnSide = false;
	}

	private void MoveUnderCorner(){
		float xUnderCornerTransform = playerWidth * 0.83f;
		float yUnderCornerTransform = playerHeight * 0.2f;

		transform.position += new Vector3(state.facingRight ? xUnderCornerTransform : -xUnderCornerTransform, -yUnderCornerTransform, 0);
		FlipPlayer();
		lastOnSide = true;
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
		if ((hAxis > wallJumpAxisActivationPoint && !state.facingRight) || (hAxis < -wallJumpAxisActivationPoint && state.facingRight)) {
			state.leanOffWall = true;
		}
		else {
			state.leanOffWall = false;
			currentSpeedVector.x = 0; // TODO figure out if this is nessesary
		}

		float verticalSpeed = vAxis * runSpeed; //(state.sprintButton ? sprintSpeed : runSpeed);

		if(state.onGround && verticalSpeed < 0 || state.onCeiling && verticalSpeed > 0)
			verticalSpeed = 0;

		//if(state.onCeilingCorner && state.
		if (state.leanOffWall == true){
			verticalSpeed = 0;
		}

		currentSpeedVector.y = verticalSpeed;
	}

	//##  JUMPING  ##//
	private bool jumpButtonDown = false;
	private float jumpSpeed = .45f;
	private static Vector3 currentJumpSpeed;

	private float wallJumpYVelocity = 0.5f;
	private float wallJumpXVelocity = 0.5f; 

	//## FALLING ##//
	private float terminalVelocity = -.8f;
	private float gravityFactor = .02f;	

	private void JumpOffCeiling()
	{
		state.fallingOffCeiling = true;
		canBoost = false;
	}

	private float jumpHeightVAxisScale = 3f;
	private float jumpHeightHAxisScale = 3f;

	private void JumpOffWalls()
	{
		if(state.leanOffWall) {
			if(!state.jumping) {
				currentJumpSpeed.y = jumpSpeed * (vAxis) + (Mathf.Abs(hAxis))/jumpHeightHAxisScale; //## This should allow us to jump sideways, up, or in a downward arc.
				currentSpeedVector.x = hAxis * jumpSpeed ;
				state.jumping = true;
			}
			//FlipPlayer();
		}
		else {
			//state.fallingOffWall = true;
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
				state.fallingOffWall = false;
			}
			if (state.onGround) {
				state.fallingOffWall = false;
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

		RaycastHit2D ground = Physics2D.Linecast (transform.position, groundChecker.position, 1 << LayerMask.NameToLayer ("Wall"), zMin, zMax);
		RaycastHit2D wallTop = Physics2D.Linecast (transform.position, wallCheckerTop.position, 1 << LayerMask.NameToLayer ("Wall"), zMin, zMax);
		RaycastHit2D wallBottom = Physics2D.Linecast (transform.position, wallCheckerBottom.position, 1 << LayerMask.NameToLayer ("Wall"), zMin, zMax);
		RaycastHit2D ceiling = Physics2D.Linecast (transform.position, ceilingChecker.position, 1 << LayerMask.NameToLayer ("Wall"), zMin, zMax);
		RaycastHit2D wallTopBack = Physics2D.Linecast (transform.position, wallCheckerTopBack.position, 1 << LayerMask.NameToLayer ("Wall"), zMin, zMax);
		RaycastHit2D wallBottomBack =  Physics2D.Linecast (transform.position, wallCheckerBottomBack.position, 1 << LayerMask.NameToLayer ("Wall"), zMin, zMax);
		RaycastHit2D floorCorner = Physics2D.Linecast (groundChecker.position, floorCornerCheck.position, 1 << LayerMask.NameToLayer ("Wall"), zMin, zMax);
		RaycastHit2D ceilingCornerBack = Physics2D.Linecast (ceilingChecker.position, ceilingCornerCheckBack.position, 1 << LayerMask.NameToLayer ("Wall"), zMin, zMax);
		RaycastHit2D ceilingCornerFront = Physics2D.Linecast (ceilingChecker.position, ceilingCornerCheckFront.position, 1 << LayerMask.NameToLayer ("Wall"), zMin, zMax);
		
		state.onGround = ground;
		state.onWallFront = wallTop || wallBottom;
		state.onCeiling = ceiling;
		state.onWallBack = wallTopBack || wallBottomBack;
		state.onFloorCorner = !state.onWallBack && !state.onGround && floorCorner;
		state.onCeilingCornerBack = !state.onWallBack && !state.onCeiling && ceilingCornerBack;
		state.onCeilingCornerFront = !state.onWallFront && !state.onCeiling && ceilingCornerFront;
	}
	
	void OnCollisionEnter2D (Collision2D collision) {
		float z = collision.collider.transform.position.z;
		if (z < zMin || z > zMax) {
			Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
		}
	}


	//## ANIMATION ##//
	private Animator animator;
	private int animState = 0;
	void Animate()
	{
		int newState = animState;
		if (state.onGround) {
			if (hAxis != 0) {
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

	private Vector3 warpVector;
	private bool warping = false;
	private float maxWarpDistance = 1.5f;

	private void Recharge()
	{
		state.drained = false;
	}
	
	private void WarpController(float hAxis, float vAxis, float warpHAxis, float warpVAxis) 
	{
		//if (!state.drained && InputWrapper.GetWarp())
		if (!state.drained)
		{
			if(state.onWallFront) 
			{
				if (state.facingRight && warpHAxis >= axisActivationPoint) {
					RaycastHit2D hit = Physics2D.Linecast (wallCheckerBottom.position, wallCheckerTop.position, 1 << LayerMask.NameToLayer ("Wall"));
					Bounds hitBounds = hit.collider.bounds;
					Vector3 size = hitBounds.size;
					if (size.x < maxWarpDistance){
						warpVector = new Vector3 (size.x + playerWidth, 0, 0);
						FlipPlayer ();
						warping = true;
					}
				} else if (!state.facingRight && warpHAxis <= -axisActivationPoint){
					RaycastHit2D hit = Physics2D.Linecast (wallCheckerBottom.position, wallCheckerTop.position, 1 << LayerMask.NameToLayer ("Wall"));
					Bounds hitBounds = hit.collider.bounds;
					Vector3 size = hitBounds.size; 
					if (size.x < maxWarpDistance){
						warpVector = new Vector3 (-(size.x + playerWidth), 0, 0);
						FlipPlayer ();
						warping = true;
					}
				}
			}
			if(state.onGround && warpVAxis <= -axisActivationPoint) 
			{
				RaycastHit2D hit = Physics2D.Linecast (transform.position, groundChecker.position, 1 << LayerMask.NameToLayer ("Wall"));
				Bounds hitBounds = hit.collider.bounds;
				Vector3 size = hitBounds.size;
				if (size.y < maxWarpDistance){
					warpVector = new Vector3(0, -size.y - playerHeight, 0);
					warping = true;
				}
			}
			else if(state.onCeiling && warpVAxis >= axisActivationPoint) 
			{
				RaycastHit2D hit = Physics2D.Linecast (transform.position, ceilingChecker.position, 1 << LayerMask.NameToLayer ("Wall"));
				Bounds hitBounds = hit.collider.bounds;
				Vector3 size = hitBounds.size;
				if (size.y < maxWarpDistance){
					warpVector = new Vector3(0, size.y  + playerHeight, 0);
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
		transform.position = playerStartPosition.position;
	}

	public void Reset()
	{
		PlayerInputEnabled (true);
		vAxis = 0;
		hAxis = 0;
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