using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class FishMovement : MonoBehaviour {

	public float searchRadius = 50f;
	public float speed;
	public float maxSpeed = 20;
	[HideInInspector]public Rigidbody2D rb2D;

	private Transform player;
	private bool chasing = false;
	private bool isAggro = true;
	private bool fleeing = false;
	private bool moving = false;
	private Vector3 destination;
	private Animator animator;

	void Start () {
		animator = GetComponent<Animator> ();
	}

	void Awake () 
	{
		player = GameObject.FindGameObjectWithTag ("Player").transform;
		rb2D = GetComponent<Rigidbody2D> ();
	}

	void Update () {
		Collider2D[] objectsInArea = Physics2D.OverlapCircleAll (transform.position, searchRadius);
		Vector2 moveDirection = rb2D.velocity;

		if (GameManager.instance.onPauseScreen) 
		{
			rb2D.gravityScale = 0f;
			return;
		} else {
			int i = 0;
			while (i < objectsInArea.Length) 
			{
				if (objectsInArea [i].tag == "Player" && !player.GetComponent<PlayerController> ().isHidden)
				{
					if (isAggro) 
					{
						chasing = true;

					} else {
						fleeing = true;
					}

					break;
				}

				chasing = false;
				fleeing = false;
				i++;
			}

			animator.SetBool ("FishSwimming", moving);

			// Rotation
			if (moveDirection != Vector2.zero) 
			{
				float angle = Mathf.Atan2 (moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
				transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);
			}

			// If the player's level is higher than the enemy's level then set their collider to a trigger to be eaten.
			if (GameObject.Find ("Player").gameObject.GetComponent<PlayerController> ().level >= this.GetComponent<EnemyLevel> ().level) 
			{
				isAggro = false;
			}
		}
	}

	void FixedUpdate ()
	{		
		if (GameManager.instance.doingSetup)
			return;

		if (!GameManager.instance.onPauseScreen)
		{	
			// Determine where the enemy should chase the player or flee from the player.
			if (!player.GetComponent<PlayerController> ().isHit) 
			{
				if (chasing) 
				{
					destination = player.transform.position - transform.position;
					moving = true;
					rb2D.AddForce (destination * speed);
				} else if (fleeing) {
					destination = -(player.transform.position - transform.position);
					rb2D.AddForce (destination * speed);
				} 
			}
		}
	}
}
