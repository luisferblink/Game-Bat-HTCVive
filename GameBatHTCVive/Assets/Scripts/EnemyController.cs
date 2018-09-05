using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour {


	public enum level {
		Training, Normal, Hard
	};

	public enum state {
		Run, Attack, Dead
	};

	public PoolCannonBullet enlacePoolCannonBullet;

	public level stateLevelEnemy;
	public state enemyState;

	public float firingAngle = 45.0f;
	public float gravity = 9.8f;

	public float timeAttack;

	public float wanderRadius;
	public float wanderTimer;
	public float life;

	private Transform target;
	private NavMeshAgent agent;
	private float timer;
	private float rateAttack;

	PlayerController enlacePlayer;

	Animator animEnemy;

	void Start(){
		enlacePlayer = GameObject.FindObjectOfType<PlayerController> ();
		animEnemy = GetComponent<Animator> ();
	}


	void OnEnable(){
		agent = GetComponent<NavMeshAgent> ();
		timer = wanderTimer;
	}


	void Update(){
		timer += Time.deltaTime;
		if(timer >= wanderTimer){
			Vector3 newPos = RandomNavEnemy(transform.position, wanderRadius, -1);
			agent.SetDestination(newPos);
			timer = 0;
		}

		if (stateLevelEnemy == level.Normal) {
			if (enemyState == state.Attack) {
				transform.LookAt (Camera.main.transform);
				rateAttack += Time.deltaTime;
				if (rateAttack < timeAttack) {
					animEnemy.SetBool ("Attack", true);
					animEnemy.SetBool ("isDead", false);

				} else if (rateAttack > timeAttack) {
					animEnemy.SetBool ("Attack", false);
					animEnemy.SetBool ("isDead", false);
					if (rateAttack >= timeAttack * 6) {
						rateAttack = 0;
					}
				}
			} else if (enemyState == state.Dead) {
				animEnemy.SetBool ("Attack", false);
				animEnemy.SetBool ("isDead", true);
			} else if (enemyState == state.Run) {
				animEnemy.SetBool ("Attack", false);
				animEnemy.SetBool ("isDead", false);
			}

			if (enlacePlayer.state == PlayerController.statePlayer.Dead) {
				if (enemyState != state.Dead) {
					enemyState = state.Run;
				}
			}
		}
	}

	public void ShootEye(){
		if (enlacePoolCannonBullet.poolBullets.Count > 0) {
			StartCoroutine(ShootBullet(enlacePoolCannonBullet.GetBullet ().transform, Camera.main.transform.position));
		}
	}


	public IEnumerator ShootBullet(Transform projectile, Vector3 target){
		projectile.position = transform.GetChild(2).position;
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

	public static Vector3 RandomNavEnemy(Vector3 origin, float dist, int layermask){
		Vector3 randDirection = Random.insideUnitSphere * dist;

		randDirection += origin;

		NavMeshHit navHit;

		NavMesh.SamplePosition (randDirection, out navHit, dist, layermask);

		return navHit.position;
	}

	public void OnTriggerEnter(Collider col){
		if (col.tag.Contains ("Bullet")) {
			if (life > 0) {
				life--;
				if (enemyState != state.Attack && enemyState != state.Dead) {
					enemyState = state.Attack;
				}
			} else {
				enlacePlayer.numberOfKill++;
				agent.isStopped = true;
				enemyState = state.Dead;
			}
		}
	}
}
