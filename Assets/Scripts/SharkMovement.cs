﻿using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class SharkMovement : MonoBehaviour {

	public float searchRadius = 1.5f;
	public float speed;
	public float maxMovement;
	[HideInInspector]public Rigidbody2D rb2D;

	private Transform player;
	private bool chasing = false;
	private bool isAggro = true;
	private bool fleeing = false;
	private bool hasMove = false;
	private float moveStart = 0f;
	private float moveEnd = 0f;
	private Vector3 destination;
	private Animator animator;

	void Start ()
	{
		destination = Vector3.zero;
		animator = GetComponent<Animator> ();
	}

	void Awake () 
	{
		player = GameObject.FindGameObjectWithTag ("Player").transform;
		rb2D = GetComponent<Rigidbody2D> ();
	}

	void Update () 
	{
		Collider2D[] objectsInArea = Physics2D.OverlapCircleAll (transform.position, searchRadius);

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

			// If the player's level is higher than the enemy's level then set their collider to a trigger to be eaten.
			if (GameObject.Find ("Player").gameObject.GetComponent<PlayerController> ().level > this.GetComponent<EnemyLevel> ().level) 
			{
				isAggro = false;
			}
		}
	}

	void FixedUpdate ()
	{			
		// Determine if the enemy should chase the player or flee from the player.
		if (chasing) 
		{
			destination = player.transform.position - transform.position;
			rb2D.AddForce (destination * speed);
			hasMove = false;
		} 
		else if (fleeing)
		{
			destination = -(player.transform.position - transform.position);
			rb2D.AddForce (destination * speed);
			hasMove = false;
		}
		else 
		{
			if (!hasMove)
			{
				destination = new Vector3 (Random.Range (-maxMovement, maxMovement), 0, 0);
				hasMove = true;
				moveStart = Time.time;
				moveEnd = moveStart + 1f;

				if ((rb2D.position.x - destination.x) < 0) 
				{
					GetComponent<SpriteRenderer> ().flipX = false;
				} else {
					GetComponent<SpriteRenderer> ().flipX = true;
				}
			}
			else if (transform.position == destination || Time.time >= moveEnd) 
			{
				hasMove = false;
			}
			else if (transform.position != destination) // Shark should move steadily toward a position when it has a move.
			{
				rb2D.velocity = (destination * speed) / 10;
			}
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.CompareTag ("Player")) 
		{
			animator.SetTrigger ("SharkAttack");
		}
	}
}
