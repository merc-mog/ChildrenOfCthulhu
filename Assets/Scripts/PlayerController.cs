using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	[HideInInspector]public bool hide = false;
	public float speed;
	public float searchRadius = 1f;
	public bool isHidden = false;

	private Rigidbody2D rb2D;
	private GameObject objectHiddenIn;

	void Awake()
	{
		rb2D = GetComponent<Rigidbody2D> ();
	}

	void Update()
	{
		Collider2D[] objectsInArea = Physics2D.OverlapCircleAll(transform.position, searchRadius);
		Vector2 moveDirection = rb2D.velocity;

		// Check to see if the player can hide in shelter
		int i = 0;
		while (i < objectsInArea.Length && !hide)
		{
			if (objectsInArea [i].tag == "Shelter" && !hide)
			{
				hide = true;
				objectHiddenIn = objectsInArea [i].gameObject;
				break;
			}
			i++;
		}

		if (Physics2D.OverlapCircleAll(transform.position, searchRadius).Length <= 1) 
		{
			hide = false;
		}

		if (moveDirection != Vector2.zero) 
		{
			float angle = Mathf.Atan2 (moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);
		}
	}

	void FixedUpdate()
	{
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVeritcal = Input.GetAxis ("Vertical");
		Vector3 movement = new Vector3 (moveHorizontal, moveVeritcal, 0);

		// Movement
		if (!isHidden) 
		{
			rb2D.AddForce (movement * speed);
		}

		// If the player can hide and presses space, hide the character
		if (hide && Input.GetButtonDown ("Jump") && !isHidden) 
		{
			isHidden = true;
		} 
		else if (isHidden && Input.GetButtonDown ("Jump")) // If the player is already hidden and presses space, reveal the character
		{
			isHidden = false;
		}

		// If the player is hidden, keep the player under the shelter
		if (isHidden) 
		{
			transform.position = new Vector3(objectHiddenIn.transform.position.x, objectHiddenIn.transform.position.y, 0);
		}
	}
}
