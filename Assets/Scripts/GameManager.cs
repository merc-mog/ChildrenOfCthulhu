using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public static GameManager instance = null;

	private List<EnemyMovement> enemies;
	private Text gameOverText;


	// Use this for initialization
	void Awake () 
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);

		DontDestroyOnLoad (gameObject);

		enemies = new List<EnemyMovement> ();

		gameOverText = GameObject.Find ("GameOverText").GetComponent<Text>();
		gameOverText.text = "";
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	public void GameOver()
	{
		gameOverText.text = "Game Over";

		gameObject.GetComponent<CameraController> ().CenterCamera ();
	}
}
