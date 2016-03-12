using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bullet : MonoBehaviour {
	
    // Variabile necesare clasei Bullet
	public float speed = 600.0f;
	public float life = 3;
	public ParticleSystem normalTrailParticles;
	public ParticleSystem bounceTrailParticles;
	public ParticleSystem pierceTrailParticles;
	public ParticleSystem ImpactParticles;
	public int damage = 20;
	public bool piercing = false;
	public bool bounce = false;
	public Color bulletColor;
	public AudioClip bounceSound;
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

		// Setãm culorile particulelor tipurilor de gloanþe.
		normalTrailParticles.startColor = bulletColor;
		bounceTrailParticles.startColor = bulletColor;
		pierceTrailParticles.startColor = bulletColor;
		ImpactParticles.startColor = bulletColor;

		normalTrailParticles.gameObject.SetActive(true);
		if (bounce) {
			bounceTrailParticles.gameObject.SetActive(true);
			normalTrailParticles.gameObject.SetActive(false);
			life = 1;
			speed = 20;
		}
		if (piercing) {
			pierceTrailParticles.gameObject.SetActive(true);
			normalTrailParticles.gameObject.SetActive(false);
			speed = 40;
		}
	}

	void Update() {
		if (hasHit) {
			return;
		}

		// Adãugam timpul trecut de la ultimul Update.
		timer += Time.deltaTime;

		// Programat pentru distrugere dacã glontele nu loveºte nimic.
		if (timer >= life) {
			Dissipate();
		}

        velocity = transform.forward;
		velocity.y = 0;
		velocity = velocity.normalized * speed;

		newPos += velocity * Time.deltaTime;
	
		// Verificãm dacã am lovit ceva în calea noastrã.
		direction = newPos - oldPos;
		float distance = direction.magnitude;

		if (distance > 0) {
            RaycastHit[] hits = Physics.RaycastAll(oldPos, direction, distance);

		    // Cautã prima loviturã validã.
		    for (int i = 0; i < hits.Length; i++) {
		        RaycastHit hit = hits[i];

				if (ShouldIgnoreHit(hit)) {
					continue;
				}

                // notificã dacã am lovit
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
	 * Nu vom lovii de douã ori acelaºi inamic cu piercing shots.
	 * Loviturile ce pot strâpunge adversarul se pot reflecta din obiectele lovite daca
	 * au ambele proprietãþi de bouncing ºi piercing shots.
	 */
	bool ShouldIgnoreHit (RaycastHit hit) {
		if (lastHit.point == hit.point || lastHit.collider == hit.collider)
			return true;
		
		return false;
	}

	/**
	 * Funcþie care determinã ce facem când lovim ceva.
	 */
	void OnHit(RaycastHit hit) {
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

        if (hit.transform.tag == "Environment") {
			newPos = hit.point;
			ImpactParticles.transform.position = hit.point;
			ImpactParticles.transform.rotation = rotation;
			ImpactParticles.Play();
			if (bounce) {
				Vector3 reflect = Vector3.Reflect(direction, hit.normal);
				transform.forward = reflect;
				bulletAudio.clip = bounceSound;
				bulletAudio.pitch = Random.Range(0.8f, 1.2f);
				bulletAudio.Play();
			}
			else {
				hasHit = true;
				bulletAudio.clip = hitSound;
				bulletAudio.volume = 0.5f;
				bulletAudio.pitch = Random.Range(1.2f, 1.3f);
				bulletAudio.Play();
				DelayedDestroy();
			}
        }

        if (hit.transform.tag == "Enemy") {
			ImpactParticles.transform.position = hit.point;
			ImpactParticles.transform.rotation = rotation;
			ImpactParticles.Play();

			// Se încearcã gãsirea scripului EnemyHealth pentru gameobject-ul lovit.
			EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();
			
			// Dacã incamicul mai are viaþã...
			if (enemyHealth != null) {
				// ... încaseazã daune.
				enemyHealth.TakeDamage(damage, hit.point);
			}
			if (!piercing) {
            	hasHit = true;
				DelayedDestroy();
			}
			bulletAudio.clip = hitSound;
			bulletAudio.volume = 0.5f;
			bulletAudio.pitch = Random.Range(1.2f, 1.3f);
			bulletAudio.Play();
        }
	}

	// Metodã de distrugere a gameobjectelor.
	void Dissipate() {
		normalTrailParticles.enableEmission = false;
		normalTrailParticles.transform.parent = null;
		Destroy(normalTrailParticles.gameObject, normalTrailParticles.duration);

		if (bounce) {
			bounceTrailParticles.enableEmission = false;
			bounceTrailParticles.transform.parent = null;
			Destroy(bounceTrailParticles.gameObject, bounceTrailParticles.duration);
		}
		if (piercing) {
			pierceTrailParticles.enableEmission = false;
			pierceTrailParticles.transform.parent = null;
			Destroy(pierceTrailParticles.gameObject, pierceTrailParticles.duration);
		}

		Destroy(gameObject);
	}

	void DelayedDestroy() {
		normalTrailParticles.gameObject.SetActive(false);
		if (bounce) {
			bounceTrailParticles.gameObject.SetActive(false);
		}
		if (piercing) {
			pierceTrailParticles.gameObject.SetActive(false);
		}
		Destroy(gameObject, hitSound.length);
	}
}