using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public static GameManager instance = null;
	public int sustenanceLevel = 0;
	public float levelStartDelay = 2f;
	public bool onPauseScreen = false;
	public bool levelCompleted = false;

	private int level = 0;
	private GameObject gameOverImage;
	private GameObject levelImage;
	private GameObject pauseMenu;
	private GameObject levelCompleteImage;
	private Text levelText;
	private bool doingSetup = false;
	private bool isGameOver = false;

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
		

		if (isGameOver && Input.GetButtonDown ("Jump")) 
		{
			isGameOver = false;
			SceneManager.LoadScene (0);
			enabled = false;
			level = 0;
		}

		if (levelCompleted && Input.GetButtonDown ("Jump")) 
		{
			levelCompleted = false;
			level++;
			InitGame ();
		}
	}

	void InitGame()
	{
		//While doingSetup is true the player can't move, prevent player from moving while title card is up.
		doingSetup = true;

		//Get a reference to our image LevelImage by finding it by name.
		levelImage = GameObject.Find ("LevelImage");
		gameOverImage = GameObject.Find ("GameOverImage");
		pauseMenu = GameObject.Find ("PauseMenuPanel");
		levelCompleteImage = GameObject.Find ("LevelCompleteImage");

		//Get a reference to our text LevelText's text component by finding it by name and calling GetComponent.
		levelText = GameObject.Find("LevelText").GetComponent<Text>();

		//Set the text of levelText to the string "Day" and append the current level number.
		if(levelText)
			levelText.text = "Level " + level;

		//Set levelImage to active blocking player's view of the game board during setup.
		if(levelImage)
			levelImage.SetActive(true);
		if(gameOverImage)
			gameOverImage.SetActive (true);
		if(pauseMenu)
			pauseMenu.SetActive (true);
		if (levelCompleteImage)
			levelCompleteImage.SetActive (true);

		//Call the HideLevelImage function with a delay in seconds of levelStartDelay.
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
		
		doingSetup = false;
	}

	// This is called each time a scene is loaded
	void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
	{
		level++;

		InitGame ();
	}

	void OnEnable()
	{
		//Tell our ‘OnLevelFinishedLoading’ function to start listening for a scene change event as soon as this script is enabled.
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}

	void OnDisable()
	{
		//Tell our ‘OnLevelFinishedLoading’ function to stop listening for a scene change event as soon as this script is disabled.
		SceneManager.sceneLoaded -= OnLevelFinishedLoading;
	}

	public void GameOver()
	{
		gameOverImage.SetActive (true);
		isGameOver = true;
	}

	public void levelComplete()
	{
		levelCompleted = true;
	}

	void TogglePauseMenu()
	{
		// Check to see if the Pause Menu is already up
		if (pauseMenu.activeSelf) // If it is, deactivate it.
		{
			pauseMenu.SetActive (false);
			onPauseScreen = false;
		} else { // Else if it's not, activate it and freeze elements on the screen.
			pauseMenu.SetActive (true);
			onPauseScreen = true;
		}
	}

	public void OnMainMenuButtonClick()
	{
		SceneManager.LoadScene (0);
		level--;
		onPauseScreen = false;
		enabled = false;
	}
}
