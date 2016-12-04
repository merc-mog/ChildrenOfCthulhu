using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadSceneOnClick : MonoBehaviour 
{
	public void LoadByIndex(int sceneIndex)
	{
		print (sceneIndex);
		SceneManager.LoadScene (sceneIndex);
		GameManager.instance.enabled = true;
	}
}
