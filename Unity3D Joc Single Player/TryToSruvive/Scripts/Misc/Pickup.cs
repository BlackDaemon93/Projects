﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Pickup : MonoBehaviour {
    
    // Variabile
	public enum PickupType {Bullet, Bounce, Pierce, Health}
	public PickupType pickupType = PickupType.Bullet;
	public float rotateSpeed = 90f;
	
	public Text label;

	private Renderer[] quadRenderers;
	// Referinţe la GameObject-ul jucatorului.
	private GameObject player;  
	GameObject canvas;
	Light pickupLight;
	bool used = false;

	void Awake() {
		// Setăm referinţele.
		player = GameObject.FindGameObjectWithTag("Player");
		quadRenderers = GetComponentsInChildren<Renderer>();
		canvas = GameObject.Find("PickupLabelCanvas");
		pickupLight = GetComponentInChildren<Light>();
	}

	void Start () {
		label.gameObject.transform.SetParent(canvas.transform, false);
		label.color = pickupLight.color;
		label.transform.localScale = Vector3.one;
		label.transform.rotation = Quaternion.identity;
	}

	void Update() {
		if (used) {
			return;
		}

		transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);

		Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
		label.transform.position = screenPos + new Vector3(0, 40, 0);
	}

    // Atunci când un inamic moare... 
    // sunt sanse ca acesta să lase în urma sa...
    // un obiect folositor jucatorului.
	void OnTriggerEnter (Collider other) {
		if (used) {
			return;
		}

		if (other.gameObject != player) {
			return;
		}

		switch (pickupType) {
			case PickupType.Bullet:
				if (other.GetComponentInChildren<PlayerShooting>().numberOfBullets <= 36) {
					other.GetComponentInChildren<PlayerShooting>().numberOfBullets++;
				}
				break;
				
			case PickupType.Bounce:
				other.GetComponentInChildren<PlayerShooting>().BounceTimer = 0;
				break;
				
			case PickupType.Pierce:
				other.GetComponentInChildren<PlayerShooting>().PierceTimer = 0;
				break;
				
			case PickupType.Health:
				other.GetComponentInChildren<PlayerHealth>().AddHealth(25);
				break;
		}

		GetComponent<AudioSource>().Play();

		foreach (Renderer quadRenderer in quadRenderers) {
			quadRenderer.enabled = false;
		}
		GetComponent<Collider>().enabled = false;

		pickupLight.enabled = false;
		Destroy(label);

		used = true;

		Destroy(gameObject, 1);
	}
}
