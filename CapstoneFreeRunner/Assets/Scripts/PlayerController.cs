using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
	[HideInInspector] public bool facingRight = true;
	[HideInInspector] public bool playerInputEnabled = true;

	//## RUNNING ##//
	private Vector3 currentSpeedVector;
	private float boostSpeed = .4f;
	private float maxSpeed = .2f;


	//## COLLISION CHECKING ##//
	public Transform groundChecker;
	public Transform wallCheckerTop;
	public Transform wallCheckerBottom;
	public Transform ceilingChecker;

	private bool onWallBottom = false;
	private bool onCeiling = false;
	private bool onGround = false;
	private bool onWallTop = false;

	private float terminalVelocity = -.8f;
	
	void FixedUpdate()
	{
		if (!onGround && !onCeiling && !(onWallTop || onWallBottom)) {
			isFalling = true;
		}
		if(isFalling)
		{
			currentSpeedVector.y -= gravityFactor;
			if(currentSpeedVector.y < terminalVelocity) {
				currentSpeedVector.y = terminalVelocity;
			}

			if (!onCeiling && !onWallBottom || onGround) {
				isFalling = false;
				Debug.Log ("Not Falling.");
			} 
		}
		if (onGround && !(onWallTop || onWallBottom)) {
			currentSpeedVector.y = 0;
		}
	}

	void Update () 
	{
		if (playerInputEnabled){
			MoveController();
	//		WarpController();
		}
		transform.position += currentSpeedVector;
		CollisionCheck();
	}

	void MoveController () 
	{
		JumpController();

		if (onGround) {
			RunOnFloor();
		}
		if (onWallTop || onWallBottom){
			ClimbWalls();
		}
		if (onCeiling) {
			ClimbCeiling();
		}
	}

	void RunOnFloor()
	{	
		float horizontalInput = InputWrapper.GetHorizontalAxis ();
		bool boostingInput = InputWrapper.GetBoost();
		
		if (horizontalInput > 0 && !facingRight) {
			FlipPlayer ();
		} else if (horizontalInput < 0 && facingRight) {
			FlipPlayer ();
		}
		else if (onWallBottom) {
			currentSpeedVector.x = 0;
			return;
		}
		
		if (boostingInput && onGround) {
			currentSpeedVector.x = horizontalInput * boostSpeed;
		}
		else {
			currentSpeedVector.x = horizontalInput * maxSpeed;
		}
	}

	private bool leanOffWall = false;

	void ClimbWalls()
	{
		float horizontalInput = InputWrapper.GetHorizontalAxis ();
		float verticalInput = InputWrapper.GetVerticalAxis ();
		bool boostingInput = InputWrapper.GetBoost();

		if ((horizontalInput > 0 && !facingRight) || (horizontalInput < 0 && facingRight)) {
			leanOffWall = true;
		}
		else {
			leanOffWall = false;
			currentSpeedVector.x = 0;
		}

		float speed = 0;

		if(boostingInput) {
			speed = verticalInput * boostSpeed;
		} 
		else {
			speed = verticalInput * maxSpeed;
		}

		if(onGround && speed < 0 || onCeiling && speed > 0) {
			Debug.Log (speed);
			currentSpeedVector.y = 0;
			return;
		}

		currentSpeedVector.y = speed;
	}

	void ClimbCeiling()
	{
//		currentSpeedVector.y = 0;

		float horizontalInput = InputWrapper.GetHorizontalAxis ();
		bool boostingInput = InputWrapper.GetBoost();
		
		if (horizontalInput > 0 && !facingRight) {
			FlipPlayer ();
		} else if (horizontalInput < 0 && facingRight) {
			FlipPlayer ();
		}
		else if (onWallBottom) {
			currentSpeedVector.x = 0;
			return;
		}
		
		if (boostingInput && onGround) {
			currentSpeedVector.x = horizontalInput * boostSpeed;
		}
		else {
			currentSpeedVector.x = horizontalInput * maxSpeed;
		}
	}


	//##  JUMPING  ##//

	[HideInInspector] public bool jumping = false;
	private float jumpOffWallSpeedMod = .3f;
	private float initialJumpSpeed = .5f;
	private float boostJumpSpeed = .7f;
	private float currentJumpSpeed;
	private float gravityFactor = .025f;

	private bool isFalling = false;

	void JumpController()
	{
		bool jump = InputWrapper.GetJump ();
		if (jump && onCeiling) {
			isFalling = true;
			Debug.Log ("Falling!");
			return;
		}

		if(jump && (onWallTop || onWallBottom))
		{
			if(leanOffWall) {
				if(!jumping) {
					if(InputWrapper.GetBoost()) {
						currentJumpSpeed = boostJumpSpeed - jumpOffWallSpeedMod;
						currentSpeedVector.x = boostSpeed;
					}
					else {
						currentJumpSpeed = initialJumpSpeed - jumpOffWallSpeedMod;
						currentSpeedVector.x = maxSpeed;
					}
					jumping = true;
				}
			}
			else {
				isFalling = true;
				Debug.Log ("Falling!");
			}
		}

		if (jump && onGround) {
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
		else if(InputWrapper.GetAbortJump() && currentJumpSpeed > 0) {
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

	void FlipPlayer()
	{
		facingRight = !facingRight;
		Vector3 scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;
	}

	// TODO Ray casts should NOT go diagonally from the center of the player
	// Fix this when bored :)
	void CollisionCheck()
	{
		onGround = Physics2D.Linecast (transform.position, groundChecker.position, 1 << LayerMask.NameToLayer ("Ground"));
		onWallTop = Physics2D.Linecast (transform.position, wallCheckerTop.position, 1 << LayerMask.NameToLayer ("Wall"));
		onWallBottom = Physics2D.Linecast (transform.position, wallCheckerBottom.position, 1 << LayerMask.NameToLayer ("Wall"));
		onCeiling = Physics2D.Linecast (transform.position, ceilingChecker.position, 1 << LayerMask.NameToLayer ("Ground"));
	}











	//## WARP ##

	private bool canWarpRight = false;
	private bool canWarpLeft = false;
	private bool canWarpTop = false;
	private bool canWarpBottom = false;
	
	private float warpAmount = 2;
	

	void WarpController()
	{
		float warpInput = Input.GetAxis ("Fire2");
		float horizontalInput = InputWrapper.GetHorizontalAxis ();
		float verticalInput = InputWrapper.GetVerticalAxis ();
		
		if (warpInput > 0){
			Debug.Log ("Warp");
			if (horizontalInput > 0 && Mathf.Abs(horizontalInput) > Mathf.Abs(verticalInput) && canWarpRight){
				// Warp right
				transform.position = new Vector3(transform.position.x + warpAmount, transform.position.y, transform.position.z);
			} else if (horizontalInput < 0 && Mathf.Abs(horizontalInput) > Mathf.Abs(verticalInput) && canWarpLeft){
				// Warp left
				transform.position = new Vector3(transform.position.x - warpAmount, transform.position.y, transform.position.z);
			} else if (verticalInput > 0 && Mathf.Abs(verticalInput) > Mathf.Abs(horizontalInput) && canWarpTop){
				// Warp up
				transform.position = new Vector3(transform.position.x, transform.position.y + warpAmount, transform.position.z);
			} else if (verticalInput < 0 && Mathf.Abs(verticalInput) > Mathf.Abs(horizontalInput) && canWarpBottom){
				// Warp down
				transform.position = new Vector3(transform.position.x, transform.position.y - warpAmount, transform.position.z);
			}
		}
	}

	void OnTriggerEnter2D(Collider2D collision) 
	{
		string tag = collision.tag;
		switch (tag){
		case "RightWarpZone":
			canWarpRight = true;
			break;
		case "LeftWarpZone":
			canWarpLeft = true;
			break;
		case "TopWarpZone":
			canWarpTop = true;
			break;
		case "BottomWarpZone":
			canWarpBottom = true;
			break;
		}
	}

	void OnTriggerExit2D(Collider2D collision) 
	{
		string tag = collision.tag;
		switch (tag){
		case "RightWarpZone":
			canWarpRight = false;
			break;
		case "LeftWarpZone":
			canWarpLeft = false;
			break;
		case "TopWarpZone":
			canWarpTop = false;
			break;
		case "BottomWarpZone":
			canWarpBottom = false;
			break;
		}
	}
}
