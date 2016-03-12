using UnityEngine;
using System.Collections;

public class Eye : MonoBehaviour {

    // Referinţă la sistemul de particule atașat 
    // astfel încât să-l putem opri.
    public ParticleSystem deathParticles;

    // Valoarea limită pentru shader-ul care va rula animaţia de dizolvare a montrilor. 
    // Facem această schimbare pentru a putea dizolva ochii inamicilor când aceştia mor.
	float cutoffValue = 0f;
	// O var bool setată true atunci când începem să distrugem un GameObject.
	bool triggered = false;

	void Update () {
		// Un update al variabilei cuttoffValue pentru a crea efectul de dizolvare gradual.
		cutoffValue = Mathf.Lerp(cutoffValue, 1f, 0.8f * Time.deltaTime);
		GetComponent<Renderer>().materials[0].SetFloat("_Cutoff", cutoffValue);

		// Distrugera GameObject-ului .
		if (cutoffValue >= 0.8f && !triggered) {
			deathParticles.Stop();
			Destroy(gameObject, 1.5f);
			triggered = true;
		}
	}
}
