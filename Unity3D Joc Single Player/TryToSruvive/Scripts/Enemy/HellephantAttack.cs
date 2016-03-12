using UnityEngine;
using System.Collections;

public class HellephantAttack : MonoBehaviour {

    // Timpul dintre fiecare atac.
    public float timeBetweenAttacks = 3f;
    // Variabile referitoare la abilitatea inamicului  
	public int bulletsPerVolley = 5;
	public float timeBetweenBullets = 0.1f;
	public int numberOfBullets = 36;
	public float angleBetweenBullets = 10f;
    public int attacksPerSpecialAttack = 3;
    // Referinţe
    public GameObject bullet;
	public AudioClip shootClip;
	public AudioClip specialClip;
	   
	// Referinţă la GameObject-ul jucătorului.
	GameObject player;
    // Referinţă la viaţa inamicului.
    EnemyHealth enemyHealth; 
	// Daca jucătorul se află în raza de atac a inamicului.
	bool playerInRange;
    // Referinţă la componenta Animator.
    Animator anim;
	// Timpul dintre atacuri.
	float attackTimer;
	float bulletTimer;
	int attackCount;
	int bulletCount;
	HellephantMovement helleMovement;
	float floatHeight = 3f;
	float landingTime;
	bool usedSpecial = false;
	bool landed = false;
	// Referinţă la componenta audio.
	AudioSource enemyAudio;  
	
	void Awake() {
		// Setăm referinţele.
		player = GameObject.FindGameObjectWithTag("Player");
		enemyHealth = GetComponent<EnemyHealth>();
		enemyAudio = GetComponent<AudioSource>();
		anim = GetComponent<Animator>();
		helleMovement = GetComponent<HellephantMovement>();
	}

	void Start() {
		// Ne asigurăm că acest element va apărea din aer.
		transform.position = new Vector3(transform.position.x, floatHeight, transform.position.z);
	}

	void Update() {
        // Timpul dintre atacuri.
        attackTimer += Time.deltaTime;
        // Dacă timpul dintre atacuri este depăşit şi jucătorul este în raza de atac a adversarului
        // şi inamicul este în viaţă, atacă.
		if (attackTimer > timeBetweenAttacks && enemyHealth.currentHealth > 0) {
			Attack();
		}
	}
	
	void Attack() {
		// The time between each bullet in our normal attack.
		bulletTimer += Time.deltaTime;

		if (attackCount < attacksPerSpecialAttack) {
			if (bulletTimer > timeBetweenBullets && bulletCount < bulletsPerVolley) {
				Vector3 relativePos = player.transform.position - transform.position;
				Quaternion rotation = Quaternion.LookRotation(relativePos);
				Quaternion rot = rotation * Quaternion.AngleAxis(Random.Range(-5.0f, 5.0f), Vector3.up) * Quaternion.AngleAxis(Random.Range(-5.0f, 5.0f), Vector3.right);
				Instantiate(bullet, transform.position + new Vector3(0, 0.5f, 0), rot);

				// Componenta audio pentru efectul de tragre .
				enemyAudio.clip = shootClip;
				enemyAudio.Play();

				// Resetăm timpul.
				bulletTimer = 0f;
				bulletCount++;

				if (bulletCount == (bulletsPerVolley)) {
					bulletCount = 0;
					attackTimer = 0;
					attackCount++;
				}
			}
		}
		else {
			SpecialAttack();
		}
	}

	void SpecialAttack() {
		// Pregătim aterizarea inamicului...
		if (!landed) {
			anim.SetBool("Landing", true);
			helleMovement.shouldMove = false;
			landingTime += Time.deltaTime * 5f;
			transform.position = new Vector3(transform.position.x, Mathf.Lerp(floatHeight, 0, landingTime), transform.position.z);
		}
		// ... începe mişcarea lui.
		else {
			anim.SetBool("Landing", false);
			helleMovement.shouldMove = true;
			landingTime += Time.deltaTime * 2f;
			transform.position = new Vector3(transform.position.x, Mathf.Lerp(0, floatHeight, landingTime), transform.position.z);
		}

		// La aterizare vom trage în toate direcţiile.
		if (transform.position.y == 0) {
			landed = true;
			if (!usedSpecial) {
				for (int i = 0; i < numberOfBullets; i++) {
					// Ne asiguram că distanţa dintre fiecare glonte este în mod egală.
					float angle = i * angleBetweenBullets - ((angleBetweenBullets / 2) * (numberOfBullets - 1));
					Quaternion rot = transform.rotation * Quaternion.AngleAxis(angle, Vector3.up);
					Instantiate(bullet, transform.position + new Vector3(0, 0.5f, 0), rot);
				}

				// Efect sonor special acestui tip de inamic.
				enemyAudio.clip = specialClip;
				enemyAudio.Play();

				usedSpecial = true;
				// Resetăm valorile pentru a pregăti urmatorul atac.
				attackTimer = 0;
				landingTime = 0;
			}
		}

		if (transform.position.y == floatHeight) {
			attackCount = 0;
			landed = false;
			usedSpecial = false;
			landingTime = 0;
		}
	}
}