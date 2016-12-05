using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadSceneOnClick : MonoBehaviour 
{
	public void LoadByIndex(int sceneIndex)
	{
		SceneManager.LoadScene (sceneIndex);

		if(GameManager.instance != null)
			GameManager.instance.enabled = true;
	}
}
