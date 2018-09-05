using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {


	public enum statePlayer
	{
		Alive, Dead
	};

	public statePlayer state;

	public Transform spawnBullet;
	public Image shield, damage, dead;
	public float firingAngle = 45.0f;
	public float gravity = 9.8f;

	public int life;

	[HideInInspector]
	public int numberOfKill;

	public PoolCannonBullet enlacePoolCannonBullet;

	bool canShoot;
	Vector3 pointShoot;

	void Start(){
		OVRTouchpad.Create();
		OVRTouchpad.TouchHandler += HandleTouchHandler;
	}
	

	void Update(){
		if (state == statePlayer.Dead)
			return;
		canShoot = false;
		Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit)) {
			if (hit.transform.tag.Contains ("Floor") || hit.transform.tag.Contains ("Object") || hit.transform.tag.Contains ("Enemy") || hit.transform.tag.Contains ("Eye")) {
				canShoot = true;
				pointShoot = hit.point;
			}
		} 
	}

	void HandleTouchHandler (object sender, System.EventArgs e){
		OVRTouchpad.TouchArgs touchArgs = (OVRTouchpad.TouchArgs)e;
		if (touchArgs.TouchType == OVRTouchpad.TouchEvent.Right) {
			if (!shield.gameObject.activeSelf && !damage.gameObject.activeSelf) {
				shield.gameObject.SetActive (true);
				Invoke ("DesactiveShield", 3.0f);
			}
		}else if (touchArgs.TouchType == OVRTouchpad.TouchEvent.SingleTap) {
			if (canShoot && enlacePoolCannonBullet.poolBullets.Count > 0) {
				StartCoroutine(ShootBullet(enlacePoolCannonBullet.GetBullet ().transform, pointShoot));
			}
		}
	}

	void DesactiveShield(){
		CancelInvoke ("DesactiveShield");
		shield.gameObject.SetActive (false);
	}

	public void Hit(){
		if (life > 0) {
			if (!shield.gameObject.activeSelf) {
				life--;
				damage.gameObject.SetActive (true);
				damage.GetComponent<Animator> ().SetBool ("damage", true);
				Invoke ("DesactiveDamage", 1.0f);
			}
		} else {
			dead.gameObject.SetActive (true);
			dead.GetComponent<Animator> ().SetBool ("dead", true);
			state = statePlayer.Dead;

		}
	}

	void DesactiveDamage(){
		CancelInvoke("DesactiveDamage");
		damage.gameObject.SetActive (false);
		damage.GetComponent<Animator> ().SetBool ("damage", false);
	}
		


	public IEnumerator ShootBullet(Transform projectile, Vector3 target){
		projectile.position = spawnBullet.position;
		float target_Distance = Vector3.Distance(projectile.position, target);
		float projectile_Velocity = target_Distance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / gravity);

		float Vx = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
		float Vy = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);

		float flightDuration = target_Distance / Vx;

		projectile.rotation = Quaternion.LookRotation(target - projectile.position);
		float elapse_time = 0;
		while (elapse_time < flightDuration){
			projectile.Translate(0, (Vy - (gravity * elapse_time)) * Time.deltaTime, Vx * Time.deltaTime);
			elapse_time += Time.deltaTime;
			yield return null;
		}
	}
}
