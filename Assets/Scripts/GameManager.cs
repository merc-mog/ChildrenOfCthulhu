using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public static GameManager instance = null;
	public int sustenanceLevel = 0;
	public float levelStartDelay = 2f;

	//private List<EnemyMovement> enemies;
	private int level = 0;
	private GameObject gameOverImage;
	private Text levelText;
	private GameObject levelImage;
	private bool doingSetup = false;

	// Use this for initialization
	void Awake () 
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);

		DontDestroyOnLoad (gameObject);

		//enemies = new List<EnemyMovement> ();
		InitGame();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (doingSetup)
			return;
	}

	void InitGame()
	{
		//While doingSetup is true the player can't move, prevent player from moving while title card is up.
		doingSetup = true;

		//Get a reference to our image LevelImage by finding it by name.
		levelImage = GameObject.Find ("LevelImage");
		gameOverImage = GameObject.Find ("GameOverImage");

		//Get a reference to our text LevelText's text component by finding it by name and calling GetComponent.
		levelText = GameObject.Find("LevelText").GetComponent<Text>();

		//Set the text of levelText to the string "Day" and append the current level number.
		levelText.text = "Level " + level;

		//Set levelImage to active blocking player's view of the game board during setup.
		levelImage.SetActive(true);
		gameOverImage.SetActive (true);

		//Call the HideLevelImage function with a delay in seconds of levelStartDelay.
		Invoke("HideLevelImage", levelStartDelay);
	}

	void HideLevelImage()
	{
		levelImage.SetActive (false);
		gameOverImage.SetActive (false);
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

		enabled = false;
	}
}
