using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
	//## INPUT ##//
	[HideInInspector] public bool playerInputEnabled = true;

	//## ANIMATION ##//
	private Animator animator;

	//## RUNNING ##//
	private bool facingRight = true;
	private Vector3 currentSpeedVector;
	private float boostSpeed = .4f;
	private float maxSpeed = .2f;
	
	//## COLLISION CHECKING ##//
	public Transform groundChecker;
	public Transform wallCheckerTop;
	public Transform wallCheckerBottom;
	public Transform ceilingChecker;

	//## RAYCASTING ##//
	private bool onWallBottom = false;
	private bool onCeiling = false;
	private bool onGround = false;
	private bool onWallTop = false;

	//## FALLING ##//
	private float terminalVelocity = -.8f;
	private bool isFalling = false;
	private float gravityFactor = .02f;	
	
	//##  JUMPING  ##//
	[HideInInspector] public bool jumping = false;
	private float jumpOffWallSpeedMod = .5f;
	private float initialJumpSpeed = .4f;
	private float boostJumpSpeed = .6f;
	private float currentJumpSpeed;
	private bool leanOffWall = false;
	private float xJumpMod = .1f;
	private float boostXJumpMod = .2f;

	//## CURRENT INPUT VALUES ##//
	private float hAxis = 0;
	private float vAxis = 0;
	private bool boostButton = false;

	void Start(){
		animator = this.GetComponent<Animator>();
	}

	void FixedUpdate()
	{
		vAxis = InputWrapper.GetVerticalAxis ();
		hAxis = InputWrapper.GetHorizontalAxis ();
		boostButton = InputWrapper.GetBoost();
		
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
		transform.position += currentSpeedVector;
		CollisionCheck();
		Animate();
	}

	void Animate()
	{
		if (onGround) {
			if (hAxis != 0){
				if (boostButton){
					animator.SetInteger("State", 2);
				}else{
					animator.SetInteger("State", 1);
				}
			}else{
				animator.SetInteger("State",0);
			}
		} else if (onWallBottom || onWallTop) {
			animator.SetInteger("State", 3);
		}

	}

	private void RunOnFloor()
	{
		if (hAxis > 0 && !facingRight) {
			FlipPlayer ();
		} else if (hAxis < 0 && facingRight) {
			FlipPlayer ();
		}
		else if (onWallBottom) {
			currentSpeedVector.x = 0;
			return;
		}
		
		if (boostButton && onGround) {
			currentSpeedVector.x = hAxis * boostSpeed;
		}
		else {
			currentSpeedVector.x = hAxis * maxSpeed;
		}
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

		float verticalSpeed = 0;

		verticalSpeed = vAxis * (boostButton ? boostSpeed : maxSpeed);

		if(onGround && verticalSpeed < 0 || onCeiling && verticalSpeed > 0) {
			verticalSpeed = 0;
		}

		currentSpeedVector.y = verticalSpeed;
	}

	private void ClimbCeiling()
	{
		if ((hAxis > 0 && !facingRight) || (hAxis < 0 && facingRight)) {
			FlipPlayer ();
		} 
		else if (onWallBottom || onWallTop) {
			currentSpeedVector.x = 0;
			return;
		}

		currentSpeedVector.x = hAxis * (boostButton ? boostSpeed : maxSpeed);
	}

	private void JumpController()
	{
		bool jumpButtonDown = InputWrapper.GetJump ();
		bool jumpButtonUp = InputWrapper.GetAbortJump ();

		if (jumpButtonDown && onCeiling) {
			isFalling = true;
			return;
		}

		if(jumpButtonDown && (onWallTop || onWallBottom))
		{
			if(leanOffWall) {
				if(!jumping) {
					if(InputWrapper.GetBoost()) {
						currentJumpSpeed = Mathf.Sign(hAxis) * (boostJumpSpeed - jumpOffWallSpeedMod);
						currentSpeedVector.x = Mathf.Sign(hAxis) * boostSpeed;
					}
					else {
						currentJumpSpeed = Mathf.Sign(hAxis) * (initialJumpSpeed - jumpOffWallSpeedMod);
						currentSpeedVector.x = Mathf.Sign(hAxis) * maxSpeed;
					}
					jumping = true;
				}
			}
			else {
				isFalling = true;
			}
		}

		if (jumpButtonDown && onGround) {
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
		else if(jumpButtonUp && currentJumpSpeed > 0) {
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

	private void FlipPlayer()
	{
		facingRight = !facingRight;
		Vector3 scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;
	}

	// TODO Ray casts should NOT go diagonally from the center of the player
	// Fix this when bored :)
	private void CollisionCheck()
	{
		onGround = Physics2D.Linecast (transform.position, groundChecker.position, 1 << LayerMask.NameToLayer ("Ground"));
		onWallTop = Physics2D.Linecast (transform.position, wallCheckerTop.position, 1 << LayerMask.NameToLayer ("Wall"));
		onWallBottom = Physics2D.Linecast (transform.position, wallCheckerBottom.position, 1 << LayerMask.NameToLayer ("Wall"));
		onCeiling = Physics2D.Linecast (transform.position, ceilingChecker.position, 1 << LayerMask.NameToLayer ("Ground"));
	}







	//when we warp, we want to hide the player, move them one pixel at a time 
	//very quickly until they are colliding with the opposite wall.  :)



	//## WARP ##

	private bool canWarpRight = false;
	private bool canWarpLeft = false;
	private bool canWarpTop = false;
	private bool canWarpBottom = false;
	
	private float warpAmount = 2;
	

	private void WarpController()
	{
		float warpInput = Input.GetAxis ("Fire2");

		if (warpInput > 0){

		}
	}
}
