using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreManager : MonoBehaviour {

	// Scorul jucătorului.
	int score;       
	
	// Referinta la componenta Text.
	public Text number;

	void Awake() {
		// Resetează scorul.
		score = 0;
	}

	void Update() {
		// Afişează scorul.
		number.text = score.ToString();
	}

    // Funcţia de adăugare a punctelor la scorul curent.
	public void AddScore(int toAdd) {
		score += toAdd;
		number.GetComponent<Animation>().Stop();
		number.GetComponent<Animation>().Play();
	}

	public int GetScore() {
		return score;
	}
}