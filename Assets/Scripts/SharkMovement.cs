using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class SharkMovement : MonoBehaviour {
	
	public float speed;
	public float maxMovement;
	[HideInInspector]public Rigidbody2D rb2D;

	private bool hasMove = false;
	private float moveEnd = 0f;
	private Vector2 destination;
	private Animator animator;

	void Start ()
	{
		destination = Vector3.zero;
		animator = GetComponent<Animator> ();
	}

	void Awake () 
	{
		rb2D = GetComponent<Rigidbody2D> ();
	}

	void FixedUpdate ()
	{		
		if (GameManager.instance.doingSetup)
			return;

		if (!GameManager.instance.onPauseScreen) 
		{
			if (!hasMove)
			{
				destination = new Vector2 (Random.Range (-maxMovement, maxMovement), 0);
				hasMove = true;
				moveEnd = Time.time + 1f;
			}

			rb2D.AddForce (destination);

			if (transform.position.x == destination.x || Time.time > moveEnd) 
			{
				hasMove = false;
			}

			if ((transform.position.x - destination.x) < 0) 
			{
				GetComponent<SpriteRenderer> ().flipX = false;
			} else {
				GetComponent<SpriteRenderer> ().flipX = true;
			}
		} else {
			rb2D.velocity = Vector2.zero;
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
