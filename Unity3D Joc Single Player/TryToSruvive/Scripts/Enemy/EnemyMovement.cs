using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour {

	// Un LayerMask unde vom aplica un Raycast care va determina în ce obiecte se poate trage.
	public LayerMask shootableMask;  
	public float roamSpeed = 1.5f;
	public float attackSpeed = 4;

    // Referinţă la poziţia jucătorului.
    Transform player;
    // Referinţă la viaţa jucătorului. 
    PlayerHealth playerHealth;
    // Referinţă la viaţa inamicului.
    EnemyHealth enemyHealth;
    // Referinţă la NavMeshAgent.
    NavMeshAgent nav;
    // Referinţă la componenta Animator.
    Animator anim;
    // Referinţă la MeshRenderer.
    SkinnedMeshRenderer myRenderer;
	// Distanţa de la vârful armei la sfărşitul ei.
	Ray shootRay;          
	// Un RaycastHit pentru a stange informaţii despre ce a fost lovit.
	RaycastHit shootHit;    
	Vector3 position;
	bool hasValidTarget = false;
	bool foundPlayer = false;

	void Awake() {
		// Setăm referinţele.
		player = GameObject.FindGameObjectWithTag ("Player").transform;
		playerHealth = player.GetComponent<PlayerHealth> ();
		enemyHealth = GetComponent <EnemyHealth> ();
		nav = GetComponent <NavMeshAgent> ();
		anim = GetComponent<Animator>();
		myRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

		SetRandomNavTarget();
	}
	
	
	void Update() {
		// Dacă adversarul este în viaţă.
		if (enemyHealth.currentHealth > 0) {
			foundPlayer = false;

			// Controlăm viteza de deplasare a inamicilor.
			//print ("Speed: " + nav.speed + " Velocity: " + nav.velocity.magnitude);
			float currentSpeed = nav.velocity.magnitude;
            anim.speed = currentSpeed;

			Vector3 distanceFromTarget = position - transform.position;

			if (playerHealth.currentHealth > 0) {
				// Trage.
				Vector3 direction = (player.position + new Vector3(0, 1, 0)) - (transform.position + new Vector3(0, 1, 0));
				shootRay.origin = transform.position + new Vector3(0, 1, 0);
				shootRay.direction = direction;
				if (Physics.Raycast (shootRay, out shootHit, 25, shootableMask)) {
					if (shootHit.transform.tag == "Player") {
						position = player.position;
						hasValidTarget = true;
						foundPlayer = true;
						//myRenderer.materials[1].color = Color.Lerp(myRenderer.materials[1].color, new Color(1, 0, 0, 0), 2 * Time.deltaTime);
						myRenderer.materials[1].SetColor("_RimColor", Color.Lerp(myRenderer.materials[1].GetColor("_RimColor"), new Color(1, 0, 0, 1), 2 * Time.deltaTime));
						nav.speed = attackSpeed;
					}
					else {
						//myRenderer.materials[1].color = Color.Lerp(myRenderer.materials[1].color, new Color(0, 0, 0, 0), 2 * Time.deltaTime);
						myRenderer.materials[1].SetColor("_RimColor", Color.Lerp(myRenderer.materials[1].GetColor("_RimColor"), new Color(0, 0, 0, 1), 2 * Time.deltaTime));
					}
				}
			}

			if (!foundPlayer) {
				if (distanceFromTarget.magnitude < 1 || !hasValidTarget) {
					SetRandomNavTarget();
				}
				nav.speed = roamSpeed;
				//myRenderer.materials[1].color = Color.Lerp(myRenderer.materials[1].color, new Color(0, 0, 0, 0), 2 * Time.deltaTime);
				myRenderer.materials[1].SetColor("_RimColor", Color.Lerp(myRenderer.materials[1].GetColor("_RimColor"), new Color(0, 0, 0, 1), 2 * Time.deltaTime));
			}

			if (hasValidTarget) {
				nav.SetDestination(position);
			}
		}
		// Altfel...
		else {
			anim.speed = 1;

			// ... Dezactivează componenta NavMeshAgent.
			nav.enabled = false;

			//myRenderer.materials[1].color = Color.Lerp(myRenderer.materials[1].color, new Color(0, 0, 0, 0), 2 * Time.deltaTime);
			myRenderer.materials[1].SetColor("_RimColor", Color.Lerp(myRenderer.materials[1].GetColor("_RimColor"), new Color(0, 0, 0, 1), 2 * Time.deltaTime));
		}
	}

	void SetRandomNavTarget() {
		Vector3 randomPosition = Random.insideUnitSphere * 30;
		randomPosition.y = 0;
		randomPosition += transform.position;
		NavMeshHit hit;
		hasValidTarget = NavMesh.SamplePosition(randomPosition, out hit, 5, 1);
		Vector3 finalPosition = hit.position;
		position = finalPosition;
    }
}