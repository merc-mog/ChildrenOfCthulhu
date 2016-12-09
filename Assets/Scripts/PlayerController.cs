using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	[HideInInspector]
	public bool canHide = false;
	public bool isHidden = false;
	public float speed;
	public float searchRadius = 1f;
	public int level = 1;
	public int hp = 3;
	public int stomach = 0;
	public Text stomachText;
	public Text hpText;

	private float levelThreshold;
	private int maxLevel = 3;
	private bool moving = false;
	private Rigidbody2D rb2D;
	private GameObject objectHiddenIn;
	private SpriteRenderer spriteRenderer;
	private Animator animator;

	void Start()
	{
		rb2D = GetComponent<Rigidbody2D> ();
		levelThreshold = 30;
		stomachText = GameObject.Find ("HungerText").GetComponent<Text>();
		hpText = GameObject.Find("HPText").GetComponent<Text>();
		spriteRenderer = GetComponent<SpriteRenderer> ();
		animator = GetComponent<Animator> ();
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
			rb2D.gravityScale = 0.5f;

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
			if (moving) 
			{
				float angle = Mathf.Atan2 (moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
				transform.rotation = Quaternion.AngleAxis (angle - 90, Vector3.forward);
			}

			// Check to see if stomach is big enough to level up
			if (stomach >= levelThreshold) 
			{
				LevelUp ();
			}

			CheckIfGameOver ();
		}
	}

	void FixedUpdate()
	{
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");
		Vector3 movement = new Vector3 (moveHorizontal, moveVertical, 0);

		if (moveHorizontal != 0 || moveVertical != 0) 
		{
			moving = true;
		} else {
			moving = false;
		}

		// Movement
		if (!isHidden) 
		{
			rb2D.AddForce (movement * speed);
			GameManager.instance.hideImage.GetComponent<Image> ().sprite = Resources.Load ("hide_in_icon", typeof(Sprite)) as Sprite;
		} else {
			transform.position = new Vector3(objectHiddenIn.transform.position.x, objectHiddenIn.transform.position.y, 0);
			GameManager.instance.hideImage.GetComponent<Image> ().sprite = Resources.Load ("hide_out_icon", typeof(Sprite)) as Sprite;
		}

		animator.SetBool ("CthulhuMoving", moving);

		if (canHide) 
		{
			GameManager.instance.hideImage.SetActive (true);
		} else {
			GameManager.instance.hideImage.SetActive (false);
		}

		// If the player can hide and presses space, hide the character
		if (canHide && Input.GetButtonDown ("Jump") && !isHidden) 
		{
			isHidden = true;
		} else if (isHidden && Input.GetButtonDown ("Jump")) { // If the player is hidden and presses jump again, reveal the player.
			isHidden = false;
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (isHidden) 
			return;
		
		if(other.gameObject.CompareTag("Enemy"))
		{
			if (other.gameObject.GetComponent<EnemyLevel> ().level > level) 
			{
				hp--;
				hpText.text = "HP: " + hp;
			} else {
				GameManager.instance.enemies.Remove (other.gameObject);
				other.gameObject.SetActive (false);
				stomach += other.gameObject.GetComponent<EnemyLevel> ().level * 10;
				stomachText.text = "Hunger: " + stomach;
			}
		}
	}

	void LevelUp()
	{
		levelThreshold += 30 * level;
		level++;

		if (level > maxLevel) 
		{
			spriteRenderer.sprite = Resources.Load ("Cthulhu_" + maxLevel, typeof(Sprite)) as Sprite;
		} else {
			spriteRenderer.sprite = Resources.Load ("Cthulhu_" + level, typeof(Sprite)) as Sprite;
		}

		animator.SetInteger ("Level", level);
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
