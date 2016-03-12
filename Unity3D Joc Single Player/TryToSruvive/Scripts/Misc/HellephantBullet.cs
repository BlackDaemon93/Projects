using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HellephantBullet : MonoBehaviour {
	
    // Variabile necesare pentru a crea modul �n care Hellephant va ataca.
	public float speed = 600.0f;
	public float life = 3;
	public ParticleSystem normalTrailParticles;
	public ParticleSystem ImpactParticles;
	public int damage = 20;
	public Color bulletColor;
	public AudioClip hitSound;

	Vector3 velocity;
    Vector3 force;
	Vector3 newPos;
	Vector3 oldPos;
	Vector3 direction;
	bool hasHit = false;
	RaycastHit lastHit;
	AudioSource bulletAudio;  
	float timer;

	void Awake() {
		bulletAudio = GetComponent<AudioSource> ();
	}

	void Start() {
		newPos = transform.position;
		oldPos = newPos;

		// Set�m culorile sistemului de particule.
		normalTrailParticles.startColor = bulletColor;
		ImpactParticles.startColor = bulletColor;
		normalTrailParticles.gameObject.SetActive(true);
	}

	void Update() {
		if (hasHit) {
			return;
		}

		// Ad�ugam timpul trecut de la ultimul Update apelat.
		timer += Time.deltaTime;

		// Program�m distrugrea proiectilului dac� nu love�te nimic.
		if (timer >= life) {
			Dissipate();
		}

        velocity = transform.forward;
		//velocity.y = 0;
		velocity = velocity.normalized * speed;

		// Presupunem c� nu avem unde s� ne mai deplas�m
		newPos += velocity * Time.deltaTime;
	
		// Verific�m dac� am lovit ceva pe parcurs.
		direction = newPos - oldPos;
		float distance = direction.magnitude;

		if (distance > 0) {
            RaycastHit[] hits = Physics.RaycastAll(oldPos, direction, distance);

		    // Caut� prima lovitur� valid�.
		    for (int i = 0; i < hits.Length; i++) {
		        RaycastHit hit = hits[i];

				if (ShouldIgnoreHit(hit)) {
					continue;
				}

				// Ia la cunostin�� lovitura.
				OnHit(hit);

				lastHit = hit;

				if (hasHit) {
					newPos = hit.point;
					break;
				}
		    }
		}

		oldPos = transform.position;
		transform.position = newPos;
	}

    /**
	 * Nu vom lovii de dou� ori acela�i inamic cu piercing shots.
	 * Loviturile ce pot str�punge adversarul se pot reflecta din obiectele lovite daca
	 * au ambele propriet��i de bouncing �i piercing shots.
	 */
    bool ShouldIgnoreHit (RaycastHit hit) {
		if (lastHit.point == hit.point || lastHit.collider == hit.collider || hit.collider.tag == "Enemy")
			return true;
		
		return false;
	}

    /**
	 * Func�ie care determin� ce facem c�nd lovim ceva.
	 */
    void OnHit(RaycastHit hit) {
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

        if (hit.transform.tag == "Environment") {
			newPos = hit.point;
			ImpactParticles.transform.position = hit.point;
			ImpactParticles.transform.rotation = rotation;
			ImpactParticles.Play();
			hasHit = true;
			bulletAudio.clip = hitSound;
			bulletAudio.volume = 0.5f;
			bulletAudio.pitch = Random.Range(0.6f, 0.8f);
			bulletAudio.Play();
			DelayedDestroy();
        }

        if (hit.transform.tag == "Player") {
			ImpactParticles.transform.position = hit.point;
			ImpactParticles.transform.rotation = rotation;
			ImpactParticles.Play();

            // Se �ncearc� g�sirea scripului EnemyHealth pentru gameobject-ul lovit.
            PlayerHealth playerHealth = hit.collider.GetComponent<PlayerHealth>();

            // Dac� incamicul mai are via��...
            if (playerHealth != null) {
                // ... �ncaseaz� daune.
                playerHealth.TakeDamage(damage);
			}
    		hasHit = true;
			DelayedDestroy();
			bulletAudio.clip = hitSound;
			bulletAudio.volume = 0.5f;
			bulletAudio.pitch = Random.Range(0.6f, 0.8f);
			bulletAudio.Play();
        }
	}

    // Metod� de distrugere a gameobjectelor.
    void Dissipate() {
		normalTrailParticles.enableEmission = false;
		normalTrailParticles.transform.parent = null;
		Destroy(normalTrailParticles.gameObject, normalTrailParticles.duration);
		Destroy(gameObject);
	}

	void DelayedDestroy() {
		normalTrailParticles.gameObject.SetActive(false);
		Destroy(gameObject, hitSound.length);
	}
}