using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {


	public enum state{
		Player, Enemy
	};

	public state stateBullet;

	public GameObject explosion;
	public float lifeTime;

	[HideInInspector]
	public bool inLifeBall;

	void OnEnable(){
		inLifeBall = true;
		Invoke ("ForceBall", lifeTime);
	}

	void Start(){
		Invoke ("ActiveCollider", 0.5f);
	}

	public void ActiveCollider(){
		CancelInvoke ("ActiveCollider");
		GetComponent<SphereCollider> ().enabled = true;
	}

	void ForceBall(){
		if (inLifeBall) {
			ActionBall ();
		}
	}

	void OnCollisionEnter(Collision col){
		if (stateBullet == state.Player) {
			if (!col.gameObject.tag.Contains ("MainCamera")) {
				ActionBall ();
			}
		}
	}

	void OnTriggerEnter(Collider col){
		if (stateBullet == state.Enemy) {
			if (col.tag.Contains ("MainCamera")) {
				col.gameObject.GetComponent<PlayerController> ().Hit ();
				ActionBall ();
			}
		}
	}

	void ActionBall(){
		CancelInvoke ("ForceBall");
		GetComponent<AudioSource> ().Play ();
		explosion.SetActive (true);
		explosion.GetComponent<ParticleSystem> ().Play ();
		GetComponent<SphereCollider> ().enabled = false;
		foreach (MeshRenderer m in GetComponentsInChildren<MeshRenderer>()) {
			m.enabled = false;
		}
		Invoke ("DesapearBall", 2.0f);
	}


	void DesapearBall(){
		CancelInvoke ("DesapearBall");
		explosion.SetActive (false);
		foreach (MeshRenderer m in GetComponentsInChildren<MeshRenderer>()) {
			m.enabled = true;
		}
		if (stateBullet == state.Player) {
			GameObject.Find ("PoolObjectsPlayer").GetComponent<PoolCannonBullet> ().SetBullet (this.gameObject);
		} else if (stateBullet == state.Enemy) {
			GameObject.Find ("PoolObjectsEnemy").GetComponent<PoolCannonBullet> ().SetBullet (this.gameObject);
		}

		inLifeBall = false;
	}
}
