using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolBulletsEnemies : MonoBehaviour {

	[HideInInspector]
	public List<GameObject> poolBullets = new List<GameObject>();

	public GameObject cannonBullet;
	public int numberCannonBullets;

	void Start(){
		for (int i = 0; i < numberCannonBullets; i++) {
			GameObject b = Instantiate (cannonBullet)as GameObject;
			b.SetActive (false);
			poolBullets.Add (b);
		}
	}

	public GameObject GetBullet(){
		if (poolBullets.Count > 0) {
			GameObject b = poolBullets [0];
			poolBullets.RemoveAt (0);
			b.SetActive (true);
			b.GetComponent<Bullet> ().Invoke ("ActiveCollider", 0.5f);
			return b;
		}
		return null;
	}

	public void SetBullet(GameObject b){
		poolBullets.Add (b);
		b.SetActive (false);
	}
}
