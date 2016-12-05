using UnityEngine;
using System.Collections;

public class EnemyLevel : MonoBehaviour {

	public int level;

	void Start () {
		level = this.level;
//		GameManager.instance.AddEnemyToList (this);
	}
}
