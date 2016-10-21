using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class MedusaMovement : MonoBehaviour {

	public float searchRadius = 1.5f;
	public float speed;
	public float maxMovement;
	[HideInInspector]public Rigidbody2D rb2D;

	private Transform player;
	private bool chasing = false;
	private bool isAggro = true;
	private bool fleeing = false;
	private float moveStartTime = 0f;
	private float moveEndTime = 0f;
	private Vector2 dir;
	private bool hasMove = false;

	void Awake () 
	{
		player = GameObject.FindGameObjectWithTag ("Player").transform;
		rb2D = GetComponent<Rigidbody2D> ();
	}

	void Update () 
	{
		Collider2D[] objectsInArea = Physics2D.OverlapCircleAll (transform.position, searchRadius);
		Vector2 moveDirection = rb2D.velocity;

		int i = 0;
		while (i < objectsInArea.Length) 
		{
			if (objectsInArea [i].tag == "Player" && !player.GetComponent<PlayerController>().isHidden) 
			{
				if (isAggro)
				{
					chasing = true;
				} 
				else 
				{
					fleeing = true;
				}

				break;
			}

			chasing = false;
			fleeing = false;

			i++;
		}

		if (moveDirection != Vector2.zero) 
		{
			float angle = Mathf.Atan2 (moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.AngleAxis (angle - 90, Vector3.forward);
		}

		// If the player's level is higher than the enemy's level then set their collider to a trigger to be eaten.
		if (GameObject.Find ("Player").gameObject.GetComponent<PlayerController> ().level > this.GetComponent<EnemyLevel>().level) 
		{
			isAggro = false;
		}
	}

	void FixedUpdate ()
	{
		// Determine if the enemy should chase the player or flee from the player.
		if (chasing) 
		{
			dir = player.transform.position - transform.position;
			rb2D.AddForce (dir * speed);
			hasMove = false;
		} 
		else if (fleeing)
		{
			dir = -(player.transform.position - transform.position);
			rb2D.AddForce (dir * speed);
			hasMove = false;
		}
		else 
		{
			if (!hasMove)
			{
				dir = new Vector2 (Random.Range (-maxMovement, maxMovement), Random.Range (-maxMovement, maxMovement));
				rb2D.AddForce (dir * speed);
				moveStartTime = Time.time;
				moveEndTime = moveStartTime + 1f;
				hasMove = true;
			}

			if (Time.time >= moveEndTime) 
			{
				hasMove = false;
			}
		}
	}
}
