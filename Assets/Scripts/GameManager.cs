using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public static GameManager instance = null;
	public GameObject hideImage;
	public int sustenanceLevel = 0;
	public float levelStartDelay = 2f;
	public bool levelCompleted = false;
	public bool onPauseScreen = false;
	public bool doingSetup = false;
	public List<GameObject> enemies;
	public int enemyCount = 0;

	private GameObject gameOverImage;
	private GameObject pauseMenu;
	private GameObject levelImage;
	private GameObject levelCompleteImage;
	private Text levelText;
	private int level = 1;
	private bool isGameOver = false;
	private bool onCredits = false;

	// Use this for initialization
	void Awake () 
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);

		DontDestroyOnLoad (gameObject);
	}

	void Start()
	{
		enemies = new List<GameObject>();

		InitGame();
	}

	// Update is called once per frame
	void Update () 
	{
		// Freeze the game while doing setup?
		if (doingSetup)
			return;

		// Toggle the Pause Menu when the escape key is pressed
		if (Input.GetKeyDown ("escape"))
			TogglePauseMenu ();

		// If there are no more enemies, the level is complete
		if (enemies.Count < 1)
		{
			levelCompleted = true;
			levelCompleteImage.SetActive (true);
		}

		if (isGameOver && Input.GetButtonDown ("Jump")) 
		{
			isGameOver = false;
			SceneManager.LoadScene (0);
			enabled = false;
		}

		if (levelCompleted && Input.GetButtonDown ("Jump")) 
		{
			levelCompleted = false;
			SceneManager.LoadScene (++level);
			if (level > 3)
			{
				onCredits = true;
			}
		}

		if (onCredits && Input.GetButtonDown ("Jump")) 
		{
			SceneManager.LoadScene (0);
			level = 1;
		}

		enemyCount = enemies.Count;
	}

	void InitGame()
	{
		// While doingSetup is true the player can't move, prevent player from moving while title card is up.
		doingSetup = true;

		// Set up the list of enemies to determine if the level is over or not.
		enemies.Clear ();

		GameObject[] tempEnemies = GameObject.FindGameObjectsWithTag ("Enemy");

		for (int i = 0; i < tempEnemies.Length; i++)
			enemies.Add (tempEnemies [i]);

		// Get a references to our images, menus, and texts
		levelImage = GameObject.Find ("LevelImage");
		gameOverImage = GameObject.Find ("GameOverImage");
		pauseMenu = GameObject.Find ("PauseMenuPanel");
		levelCompleteImage = GameObject.Find ("LevelCompleteImage");
		levelText = GameObject.Find("LevelText").GetComponent<Text>();
		hideImage = GameObject.Find ("HideImage");

		// Set the level text
		if(levelText)
			levelText.text = "Level " + level;

		// Set levelImage to active blocking player's view of the game board during setup.
		if(levelImage)
			levelImage.SetActive(true);
		if(gameOverImage)
			gameOverImage.SetActive (true);
		if(pauseMenu)
			pauseMenu.SetActive (true);
		if (levelCompleteImage)
			levelCompleteImage.SetActive (true);
		if (hideImage)
			hideImage.SetActive (true);

		// Call the HideLevelImage function with a delay in seconds of levelStartDelay.
		Invoke("HideLevelImage", levelStartDelay);
	}

	void HideLevelImage()
	{
		if(levelImage)
			levelImage.SetActive (false);
		if(gameOverImage)
			gameOverImage.SetActive (false);
		if(pauseMenu)
			pauseMenu.SetActive (false);
		if (levelCompleteImage)
			levelCompleteImage.SetActive (false);
		if (hideImage)
			hideImage.SetActive (false);
		
		doingSetup = false;
	}

	// This is called each time a scene is loaded
	void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
	{
		InitGame ();
	}

	void OnEnable()
	{
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}

	void OnDisable()
	{
		SceneManager.sceneLoaded -= OnLevelFinishedLoading;
	}

	public void GameOver()
	{
		gameOverImage.SetActive (true);
		isGameOver = true;
	}

	void TogglePauseMenu()
	{
		// Check to see if the Pause Menu is already up
		if (pauseMenu.activeSelf) // If it is active, deactivate it.
		{
			pauseMenu.SetActive (false);
			onPauseScreen = false;
		} else { // Else if it's not activated, activate it and freeze elements on the screen.
			pauseMenu.SetActive (true);
			onPauseScreen = true;
		}
	}

	public void OnMainMenuButtonClick()
	{
		SceneManager.LoadScene (0);
		onPauseScreen = false;
		enabled = false;
	}
}
