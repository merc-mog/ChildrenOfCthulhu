using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	[HideInInspector]
	public bool canHide = false;
	public bool isHidden = false;
	public bool isHit = false;
	public float speed;
	public float searchRadius = 1f;
	public int level = 1;
	public int hp = 3;
	public int stomach = 0;
	public Text stomachText;

	private float levelThreshold;
	private float hitTime;
	private int maxLevel = 3;
	private int maxHP = 3;
	private bool moving = false;
	private Rigidbody2D rb2D;
	private GameObject objectHiddenIn;
	private SpriteRenderer spriteRenderer;
	private Animator animator;
	private Image hpImage;

	void Start()
	{
		rb2D = GetComponent<Rigidbody2D> ();
		levelThreshold = 30;
		stomachText = GameObject.Find ("HungerText").GetComponent<Text>();
		hpImage = GameObject.Find ("HPImage").GetComponent<Image> ();
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
		} else {

			if (isHit) 
			{
				float currentTime = Time.time;

				if (currentTime > hitTime + 1f)
					isHit = false;
				
			} else {

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
			}

			CheckIfGameOver ();
		}
	}

	void FixedUpdate()
	{
		if (!isHit) 
		{
			float moveHorizontal = Input.GetAxis ("Horizontal");
			float moveVertical = Input.GetAxis ("Vertical");
			Vector3 movement = new Vector3 (moveHorizontal, moveVertical, 0);

			if (moveHorizontal != 0 || moveVertical != 0) {
				animator.enabled = true;
				moving = true;
			} else {
				animator.enabled = false;
				moving = false;
				transform.rotation = Quaternion.AngleAxis (0, Vector3.forward);
			}
		

			// Movement
			if (!isHidden) {
				rb2D.AddForce (movement * speed);
				GameManager.instance.hideImage.GetComponent<Image> ().sprite = Resources.Load ("hide_in_icon", typeof(Sprite)) as Sprite;
			} else {
				if (objectHiddenIn.name.Equals ("kelp-bottomhide")) {
					transform.position = new Vector3 (objectHiddenIn.transform.position.x, objectHiddenIn.transform.position.y - 1.25f, 0);
				} else if (objectHiddenIn.name.Equals ("kelp-tophide")) {
					transform.position = new Vector3 (objectHiddenIn.transform.position.x, objectHiddenIn.transform.position.y + 1, 0);
				} else {
					transform.position = new Vector3 (objectHiddenIn.transform.position.x, objectHiddenIn.transform.position.y, 0);
				}

				GameManager.instance.hideImage.GetComponent<Image> ().sprite = Resources.Load ("hide_out_icon", typeof(Sprite)) as Sprite;
			}

			if (animator) {
				animator.SetBool ("CthulhuMoving", moving);
			}

			if (!moving) {
				if (level > maxLevel) {
					spriteRenderer.sprite = Resources.Load ("Cthulhu_" + maxLevel, typeof(Sprite)) as Sprite;
				} else {
					spriteRenderer.sprite = Resources.Load ("Cthulhu_" + level, typeof(Sprite)) as Sprite;
				}
			}

			// Hiding
			if (canHide) {
				GameManager.instance.hideImage.SetActive (true);
			} else {
				GameManager.instance.hideImage.SetActive (false);
			}

			if (canHide && Input.GetButtonDown ("Jump") && !isHidden) {
				isHidden = true;
			} else if (isHidden && Input.GetButtonDown ("Jump")) { 
				isHidden = false;
			}
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
				if (!isHit) 
				{
					hp--;
					isHit = true;
					hitTime = Time.time;
					rb2D.velocity = Vector2.zero;
				}

				if (level >= 2 & hp > 0) 
				{
					if (hp == 4)
						hpImage.sprite = Resources.Load ("hp_fourfifths", typeof(Sprite)) as Sprite;
					else if (hp == 3)
						hpImage.sprite = Resources.Load ("hp_threefifths", typeof(Sprite)) as Sprite;
					else if (hp == 2)
						hpImage.sprite = Resources.Load ("hp_twofifths", typeof(Sprite)) as Sprite;
					else if (hp == 1)
						hpImage.sprite = Resources.Load ("hp_onefifth", typeof(Sprite)) as Sprite;
					else
						hpImage.sprite = Resources.Load ("hp", typeof(Sprite)) as Sprite;
				} else {
					if(hp == 2)
						hpImage.sprite = Resources.Load ("hp_twothirds", typeof(Sprite)) as Sprite;
					else if (hp == 1)
						hpImage.sprite = Resources.Load ("hp_onethird", typeof(Sprite)) as Sprite;
					else
						hpImage.sprite = Resources.Load ("hp", typeof(Sprite)) as Sprite;
				}
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
		level++;
		levelThreshold += 30 * level;

		if (level >= 2) 
		{
			maxHP = 5;
		}

		hp = maxHP;
		hpImage.sprite = Resources.Load ("hp_full", typeof(Sprite)) as Sprite;

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
