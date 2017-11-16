using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {
	private Animator anim;
	private CharacterController charController;
	//collisionflags = bitmask (binary number) - represents whether it collides with other objects like none , sides, above or below
	private CollisionFlags collisionFlags = CollisionFlags.None;

	private float moveSpeed = 5f;
	private bool canMove;
	private bool finished_Movement = true;

	private float player_ToPointDistance;

	//Vector3 is the position class, Vector3.zero = Vector3(0,0,0);
	private Vector3 target_Pos = Vector3.zero;
	private Vector3 player_Move = Vector3.zero;


	private float gravity = 9.8f;
	private float height;

	// Use this for initialization
	void Awake () {
		anim = GetComponent<Animator> ();
		charController = GetComponent<CharacterController> ();
	}
	
	// Update is called once per frame
	void Update () {
		CalculateHeight ();
		CheckIfFinishedMovement ();
	}
	//screen point = mouse input (pixels)
	//world point = unity input (coords)

	bool IsGrounded(){
		return collisionFlags == CollisionFlags.CollidedBelow ? true : false;
	}

	void CalculateHeight(){
		if (IsGrounded()) {
			height = 0f;
		} else {
			height -= gravity * Time.deltaTime;
		}
	}

	void CheckIfFinishedMovement(){
		if (!finished_Movement) {
			if (!anim.IsInTransition (0) && !anim.GetCurrentAnimatorStateInfo (0).IsName ("Stand")
			    && anim.GetCurrentAnimatorStateInfo (0).normalizedTime >= 0.8f) {
				finished_Movement = true;
			}
		} else {
			MoveThePlayer ();
			player_Move.y = height * Time.deltaTime;
			collisionFlags = charController.Move (player_Move);
		}
	}

	void MoveThePlayer(){
		//getmousebuttondown(0) - 0 = left click
		if (Input.GetMouseButtonDown (0)) {
			/**
				Ray = infinite line starting origin and going to some direction.
				ScreePointToRay = ray going from camera via screen point (mouse input in this e.g.)
				RaycastHit hit; = making ray to move in a direction (x,y)
				Physics.Raycast(ray , hit) = if the physics class in unity of the ray hits on raycast then do something in out hit
			**/
			/** 
				1. this part is to get the input from mouse click in the screen. ScreenPointToRay is to convert mouse click to unity's unit
				2. check if the ray with physics unity class to determine the click is on Terrrain
				3. then get the distance between the clicked position and current player model's position
				4. set canMove to true and target_pos (use for further below parts) value to hit.point (hit.point = the point clicked on world map)
			**/
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit)) {
				if (hit.collider is TerrainCollider) {
					//Vector3.Distance ** useful
					player_ToPointDistance = Vector3.Distance (transform.position, hit.point);
					if (player_ToPointDistance >= 1.0f) {
						canMove = true;
						target_Pos = hit.point;
					}
				}
			}
		}// mousebuttondown

		/**
			1. check if canMove 
				1.1 a.if yes then make some animation 
					b.store a temporarily vector variable from the clicked hit.point
					c. do some model rotation based on the vector (target_temp -> value from hit.point)
					d. move to the clicked position in unity world.
					e. check if the targetted position and current player's position is less than 0.5f unit then canMove = false,
							else , just set everythign to 0 and idle on animation
		**/
		if (canMove) {
			anim.SetFloat("Walk", 1.0f);
			Vector3 target_Temp = new Vector3(target_Pos.x, transform.position.y, target_Pos.z);
			//Quaternion = rotation shits
			//Quaternion.Slerp = rotate like a sphere
			//			-> param1 = current player model rotation value
			//			-> param2 = LookRotation from the clicked point (stored in target_temp) - the current player model rotation value
			// 			-> param3 = time to rotate (value is up to you)
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target_Temp - transform.position),
						15.0f * Time.deltaTime);
			//transform.foward = blue axis in unity world when I click on player(black knight) model
			player_Move = transform.forward * moveSpeed * Time.deltaTime;
			if(Vector3.Distance(transform.position, target_Pos) <= 0.5f){
				canMove = false;
				}
			}else{
				player_Move.Set(0f, 0f, 0f);
				anim.SetFloat("Walk",0f);
			}
	}
}
