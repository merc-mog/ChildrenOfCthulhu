using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	[HideInInspector]public bool canHide = false;
	public float speed;
	public float searchRadius = 1f;
	public bool isHidden = false;
	public int level = 1;
	public int hp = 3;
	public int stomach = 0;
	public Text stomachText;
	public Text hpText;

	private Rigidbody2D rb2D;
	private GameObject objectHiddenIn;
	private float levelThreshold;
	private int maxLevel = 3;

	void Start()
	{
		rb2D = GetComponent<Rigidbody2D> ();
		levelThreshold = 30;
		stomachText = GameObject.Find ("SustenanceText").GetComponent<Text>();
		hpText = GameObject.Find("HPText").GetComponent<Text>();
	}

	void Update()
	{
		Collider2D[] objectsInArea = Physics2D.OverlapCircleAll(transform.position, searchRadius);
		Vector2 moveDirection = rb2D.velocity;

		if (GameManager.instance.onPauseScreen) 
		{
			rb2D.gravityScale = 0f;
			return;
		} else 	{
			// Check to see if the player can hide in shelter
			int i = 0;
			while (i < objectsInArea.Length && !canHide) 
			{
				if (objectsInArea [i].tag == "Shelter" && !canHide) 
				{
					canHide = true;
					objectHiddenIn = objectsInArea [i].gameObject;
					break;
				}
				i++;
			}

			if (Physics2D.OverlapCircleAll (transform.position, searchRadius).Length <= 1) 
			{
				canHide = false;
			}

			// Rotation
			if (moveDirection != Vector2.zero) 
			{
				float angle = Mathf.Atan2 (moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
				transform.rotation = Quaternion.AngleAxis (angle - 90, Vector3.forward);
			}

			// Check to see if stomach is big enough to level up
			if (stomach >= levelThreshold && level < maxLevel) 
			{
				LevelUp ();
			}

			CheckIfGameOver ();
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
		if (canHide && Input.GetButtonDown ("Jump") && !isHidden) 
		{
			isHidden = true;
		} else if (isHidden && Input.GetButtonDown ("Jump")) // If the player is already hidden and presses space, reveal the character
		{
			isHidden = false;
		}

		// If the player is hidden, keep the player under the shelter
		if (isHidden) 
		{
			transform.position = new Vector3(objectHiddenIn.transform.position.x, objectHiddenIn.transform.position.y, 0);
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.gameObject.CompareTag("Enemy"))
		{
			if (other.gameObject.GetComponent<EnemyLevel> ().level > level) 
			{
				hp--;
				hpText.text = "HP: " + hp;
			} else {
				other.gameObject.SetActive (false);
				stomach += other.gameObject.GetComponent<EnemyLevel> ().level * 10;
				stomachText.text = "Sustenance: " + stomach;
			}
		}
	}

	void LevelUp()
	{
		levelThreshold *= (levelThreshold / 10);
		level++;

		gameObject.GetComponent<SpriteRenderer> ().sprite = Resources.Load("Cthulhu_" + level, typeof(Sprite)) as Sprite;
	}

	void CheckIfGameOver()
	{
		if (hp <= 0) 
		{
			enabled = false;

			GameManager.instance.GameOver ();
		}
	}
}
