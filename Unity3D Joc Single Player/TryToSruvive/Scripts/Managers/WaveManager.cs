using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour {
	
	// Referinţă la viaţa jucătorului.
	public PlayerHealth playerHealth; 
	public float bufferDistance = 200;
	// Timpul de aşteptare pentru fiecare val de monştrii.
	public float timeBetweenWaves = 5f;
    // Timpul pentru apariţiea monştrilor.
    public float spawnTime = 3f;
    // Nivelul jocului.
	public int startingWave = 1;
    // Dificultatea jocului.
	public int startingDifficulty = 1;
	// Referinţă la componenta Text.
	public Text number; 
    // Numarul de monştri rămaşi în viaţă pentru nivelul curent.
	[HideInInspector]
	public int enemiesAlive = 0;

    // O clasă care descrie un nivel cu un număr X de intrări.
    [System.Serializable]
    public class Wave {
        public Entry[] entries;

        // Descriera unui nivel.
        [System.Serializable]
        public class Entry {
            // Tipul de monştrii.
            public GameObject enemy;
            // Numărul de monştrii eliberaţi.
            public int count;
            // O număratoare a monştrilor eliberaţi.
            [System.NonSerialized]
            public int spawned;
        }
    }

    // Toate nivelele.
    public Wave[] waves;

    // Variabile necesare pentru a face ca totul să funcţioneze OK.
    Vector3 spawnPosition = Vector3.zero;
	int waveNumber;
	float timer; 
	Wave currentWave;
	int spawnedThisWave = 0;
	int totalToSpawnForWave;
	bool shouldSpawn = false;
	int difficulty;

	void Start() {
        // Să începem de la un nivel mai mare și mai greu, dacă dorim.
        waveNumber = startingWave > 0 ? startingWave - 1 : 0;
		difficulty = startingDifficulty;

		// Următorul val de monştri.
		StartCoroutine("StartNextWave");
	}
	
	void Update() {
		// Are valoarea fals cât timp setăm următorul val.
		if (!shouldSpawn) {
			return;
        }

		// Începem urmatorul val dupa ce toţi monştrii au fost eliberaţi.
		// Toţi adversarii au murit.
		if (spawnedThisWave == totalToSpawnForWave && enemiesAlive == 0) {
			StartCoroutine("StartNextWave");
			return;
		}

        // Adaugam timpul trecut de la ultimul Update.
		timer += Time.deltaTime;

        // Eliberează un adversar pentru fiecare intrare din valul respectiv.
        // Nivelul dificultăţii va determina numarul de adversari ce vor apărea
        // pentru fiecare buclă.
        if (timer >= spawnTime) {
            foreach (Wave.Entry entry in currentWave.entries) {
				if (entry.spawned < (entry.count * difficulty)) {
					Spawn(entry);
				}
			}
		}
	}

	/**
	 * 
	 */
	IEnumerator StartNextWave() {
		shouldSpawn = false;

		yield return new WaitForSeconds(timeBetweenWaves);

		if (waveNumber == waves.Length) {
			waveNumber = 0;
			difficulty++;
		}

		currentWave = waves[waveNumber];

        // Nivelul dificultăţii va determina numarul de adversari ce vor apărea
        // pentru fiecare buclă.
        totalToSpawnForWave = 0;
		foreach (Wave.Entry entry in currentWave.entries) {
			totalToSpawnForWave += (entry.count * difficulty);
		}

		spawnedThisWave = 0;
		shouldSpawn = true;

		waveNumber++;

		number.text = (waveNumber + ((difficulty - 1) * waves.Length)).ToString();
		number.GetComponent<Animation>().Play();
	}

	/**
	 * Această metodă va elibera adversari la un interval regulat.
	 */
	void Spawn(Wave.Entry entry) {
		// Resetează timpul.
		timer = 0f;
		
		// Dacă jucătorul nu mai are viaţă atunci opreşte apariţia monstrilor.
		if (playerHealth.currentHealth <= 0f) {
			return;
		}
		
		// Găseşte o pozişie random.
		Vector3 randomPosition = Random.insideUnitSphere * 35;
		randomPosition.y = 0;
		
		// Găseşte cea mai apropiată poziţie pe NavMash faţă de punctul random generat.
		// Dacă nu găsim o poziţie validă returnăm şi încercam iar.
		NavMeshHit hit;
		if (!NavMesh.SamplePosition(randomPosition, out hit, 5, 1)) {
			return;
		}
		
		// Avem toate punctele de eliberare a adverarilor pe NavMash.
		spawnPosition = hit.position;
		
		// Verifică dacă poziţia este vizibilă, dacă este
		// returnăm şi încercăm iar.
		Vector3 screenPos = Camera.main.WorldToScreenPoint(spawnPosition);
		if ((screenPos.x > -bufferDistance && screenPos.x < (Screen.width + bufferDistance)) && 
		    (screenPos.y > -bufferDistance && screenPos.y < (Screen.height + bufferDistance))) 
		{
			return;
		}

		// Am trecut de toate verificările, apar inamicii.
		GameObject enemy =  Instantiate(entry.enemy, spawnPosition, Quaternion.identity) as GameObject;
		// Multiplica viaţa şi puncele oferite în funcţie de dificultate.
		enemy.GetComponent<EnemyHealth>().startingHealth *= difficulty;
		enemy.GetComponent<EnemyHealth>().scoreValue *= difficulty;
		
		entry.spawned++;
		spawnedThisWave++;
		enemiesAlive++;
	}
}
