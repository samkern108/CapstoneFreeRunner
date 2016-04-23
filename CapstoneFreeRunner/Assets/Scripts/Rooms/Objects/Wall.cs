using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour {

	public bool warp;
	public bool floor;

	public enum Direction {ANY, UP, DOWN, LEFT, RIGHT}

	public GameObject companion;
	public Direction direction = Direction.ANY;

	public static float maxWarpDistance = 3f;

	public static Sprite singleGradient;
	public static Sprite doubleGradient;

	/*void Start()
	{
		
	}*/

	private void WarpEffects ()
	{
		/*if (companion != null && transform.parent != companion.transform.parent) {
			transform.parent.gameObject.SetActive (false);
			companion.transform.parent.gameObject.SetActive (true);
		}*/
	}

	public Vector3 WarpVertical(Vector3 heroPos, int onGround, float playerHeight, float playerZLayer)
	{
		Bounds hitBounds;
		Vector3 size;
		Vector3 diff = new Vector3();

		if (direction.Equals(Direction.ANY) || (onGround == 1) && direction.Equals (Direction.DOWN) || (onGround != 1) && direction.Equals (Direction.UP)) {

			if (companion != null) {
				hitBounds = companion.GetComponent <BoxCollider2D> ().bounds;
				diff = hitBounds.center - GetComponent<BoxCollider2D> ().bounds.center;
			} else {
				hitBounds = GetComponent<BoxCollider2D> ().bounds;
			}
			size = hitBounds.size;

			if (size.y <= maxWarpDistance) {
				WarpEffects ();
				return new Vector3 (heroPos.x + diff.x, hitBounds.center.y + onGround * ((size.y / 2) + ((playerHeight - .2f) / 2)), playerZLayer);
			}
		}

		return heroPos;
	}

	public Vector3 WarpHorizontal(Vector3 heroPos, int facingRight, float playerWidth, float playerZLayer)
	{
		Bounds hitBounds;
		Vector3 size;
		Vector3 diff = new Vector3 ();

		if (direction.Equals(Direction.ANY) || (facingRight == 1) && direction.Equals (Direction.LEFT) || (facingRight != 1) && direction.Equals (Direction.RIGHT)) {

			if (companion != null) {
				hitBounds = companion.GetComponent <BoxCollider2D> ().bounds;
				diff = hitBounds.center - GetComponent<BoxCollider2D> ().bounds.center;

			} else {
				hitBounds = GetComponent<BoxCollider2D> ().bounds;
			}
			size = hitBounds.size;

			if (size.x <= maxWarpDistance) {
				WarpEffects ();
				return new Vector3 (hitBounds.center.x + facingRight * (size.x / 2 + (playerWidth - .2f) / 2), heroPos.y + diff.y, playerZLayer);
			}
		}

		return heroPos;
	}
}
