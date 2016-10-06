using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class EnemyMovement : MonoBehaviour {

	public float searchRadius = 1f;
	public float speed = 20;
	public float maxMovement;

	private Rigidbody2D rb2D;
	private Transform player;
	private bool chasing = false;
	private float moveStartTime = 0f;
	private float moveEndTime = 0f;
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
				chasing = true;
				break;
			}

			chasing = false;

			i++;
		}

		if (moveDirection != Vector2.zero) 
		{
			float angle = Mathf.Atan2 (moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);
		}
	}

	void FixedUpdate ()
	{
		Vector2 dir;

		if (chasing) 
		{
			dir = player.transform.position - transform.position;
			rb2D.AddForce (dir * speed);
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
