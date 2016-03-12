using UnityEngine;
using System.Collections;

public class HellephantMovement : MonoBehaviour {
	
    // Variabile necesare mişcării.
	public float moveSpeed = 3f;
	public float rotateSpeed = 2f;
	[HideInInspector]
	public bool shouldMove = true;

    // Referinţă la poziţia jucătorului.
    Transform player;
    // Referinţă la viaţa jucătorului. 
    PlayerHealth playerHealth;
    // Referinţă la viaţa inamicului.
    EnemyHealth enemyHealth;
    // Referinţă la MeshRenderer.
    SkinnedMeshRenderer myRenderer;
	Vector3 position;
	float currentSpeed;

	void Awake() {
		// Setăm referinţele.
		player = GameObject.FindGameObjectWithTag ("Player").transform;
		playerHealth = player.GetComponent<PlayerHealth> ();
		enemyHealth = GetComponent <EnemyHealth> ();
		myRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
	}
	
	void Update() {
		// Daca inamicul este în viaţă...
		if (enemyHealth.currentHealth > 0) {
			if (playerHealth.currentHealth > 0) {
				Rotate();

				if (shouldMove) {
					Move();
				}
			}
		}
		// Altfel...
		else {
			myRenderer.materials[1].SetColor("_RimColor", Color.Lerp(myRenderer.materials[1].GetColor("_RimColor"), new Color(0, 0, 0, 1), 2 * Time.deltaTime));
		}
	}

    // Modul în care Hellephant-ul se va roti.
	void Rotate() {
		Quaternion rot = Quaternion.LookRotation(player.position - transform.position);
		float rotationX = rot.eulerAngles.y;
		Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
		transform.localRotation = Quaternion.Slerp(transform.localRotation, xQuaternion, Time.deltaTime * rotateSpeed);
	}
	
    // Mişcarea acestuia.
	void Move() {
		transform.GetComponent<Rigidbody>().MovePosition(transform.GetComponent<Rigidbody>().position + transform.TransformDirection(0, 0, moveSpeed) * Time.deltaTime);
	}
}