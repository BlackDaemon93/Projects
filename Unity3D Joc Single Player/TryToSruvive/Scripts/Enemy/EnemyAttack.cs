using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour {

    // Timpul dintre fiecare atac.
    public float timeBetweenAttacks = 0.5f;
    // Daunele facute de fiecare lovitură aplicată jucatorului.
    public int attackDamage = 10;               
	   
	// Referinţă la GameObjectul jucătorului.
	GameObject player;
    // Referinţă la viaţa jucătorului.
    PlayerHealth playerHealth;
    // Referinţă la viaţa inamicului.
    EnemyHealth enemyHealth; 
	// Jucătorul se află în proximitatea inamicului.
	bool playerInRange;   
	// Timpul de asteptare pentru urmatorul atac.
	float timer;                               
	
	void Awake() {
		// Setăm referinţele.
		player = GameObject.FindGameObjectWithTag("Player");
		playerHealth = player.GetComponent<PlayerHealth>();
		enemyHealth = GetComponent<EnemyHealth>();
	}

	void OnTriggerEnter(Collider other) {
        // Dacă jucătorul se află în proximitatea inamicului...
        if (other.gameObject == player) {
			playerInRange = true;
			// ... adaugă un "timp de reacţie".
			timer = 0.2f;
		}
	}
	
	
	void OnTriggerExit(Collider other) {
        // Dacă jucătorul nu se află în proximitatea inamicului.
        if (other.gameObject == player) { 
			playerInRange = false;
		}
	}
	
	
	void Update() {
        // Actualizăm timpul de la ultimul Update.
        timer += Time.deltaTime;
		
		// Dacă a trecut suficient timp de la ultimul atac şi jucătorul este în raza inamicului
		// şi jucatorul se află în viaţă, atunci atacă.
		if (timer >= timeBetweenAttacks && playerInRange && enemyHealth.currentHealth > 0 && playerHealth.currentHealth > 0) {
			Attack();
		}
	}
	
	
	void Attack() {
		// Resetăm timpul.
		timer = 0f;
		
		// Aplică daunele jucătorului.
		playerHealth.TakeDamage(attackDamage);
	}
}