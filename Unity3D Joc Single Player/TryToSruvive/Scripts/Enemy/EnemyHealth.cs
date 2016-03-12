using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyHealth : MonoBehaviour {

    // Via�a inamicului la inceput.
    public int startingHealth = 100;
    // Via�a curent� a adversarului.
    [HideInInspector]
	public int currentHealth;  
	// Viteza cu care acesta se va dizolva dupa moarte.
	public float sinkSpeed = 2.5f;   
	// Valoarea �n puncte ce va fi ad�ugata la scor c�nd inamicul va muri. 
	public int scoreValue = 10; 
	// Sunetul facut de acesta c�nd moare.
	public AudioClip deathClip;    
	// Sunetul facut de acesta cand arde.
	public AudioClip burnClip;  
	// Sistemul de particule.
	public ParticleSystem deathParticles;  
	// Via�a afi�ata deasupra fiec�rui adversar.
	public Slider healthBarSlider;
	public GameObject eye;
	
	// HealthBarSlider 
	Slider sliderInstance;
	// Inamicul este mort.
	bool isDead;
    // Inamicul arde.
    bool isBurning = false;
    // Efecte
    Color rimColor;
    float rimPower;
    float cutoffValue = 0f;

	// Componete �i scripuri la care ne vom referii.
	Animator anim;            
	AudioSource enemyAudio;        
	CapsuleCollider capsuleCollider;   
	SkinnedMeshRenderer myRenderer;
	GameObject enemyHealthbarManager;
	WaveManager waveManager;
	ScoreManager scoreManager;
	PickupManager pickupManager;

	void Awake() {
		// Set�m referin�ele.
		anim = GetComponent<Animator>();
		enemyAudio = GetComponent<AudioSource>();
		capsuleCollider = GetComponent<CapsuleCollider>();
		myRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
		enemyHealthbarManager = GameObject.Find("EnemyHealthbarsCanvas");
		waveManager = GameObject.Find("WaveManager").GetComponent<WaveManager>();
		scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
		pickupManager = GameObject.Find("PickupManager").GetComponent<PickupManager>();
	}

	void Start() {
		// Set�m via�a inamicilor la apari�ia acestora.
		currentHealth = startingHealth;

		// Instan�iem bara de via��.
		sliderInstance = Instantiate(healthBarSlider, gameObject.transform.position, Quaternion.identity) as Slider;
		sliderInstance.gameObject.transform.SetParent(enemyHealthbarManager.transform, false);
		sliderInstance.GetComponent<Healthbar>().enemy = gameObject;
		sliderInstance.gameObject.SetActive(false);

		// Culoarea �i intensitea materialului.
		rimColor = myRenderer.materials[0].GetColor("_RimColor");
        rimPower = myRenderer.materials[0].GetFloat("_RimPower");
    }

	void Update() {
		// Anima�ia de dizolvare a inamicului.
		if (isBurning) {
			cutoffValue = Mathf.Lerp(cutoffValue, 1f, 1.3f * Time.deltaTime);
			myRenderer.materials[0].SetFloat("_Cutoff", cutoffValue);
			myRenderer.materials[1].SetFloat("_Cutoff", 1);
		}
	}

	public void TakeDamage(int amount, Vector3 hitPoint) {
        StopCoroutine("Ishit");
        StartCoroutine("Ishit");

		// Dac� inamicul este mort, ie�i din func�ie.
		if (isDead)
			return;

		GetComponent<Rigidbody>().AddForceAtPosition(transform.forward * -300, hitPoint);
		
		// Reduce via�a �n func�ie de daunele �ncasate.
		currentHealth -= amount;

		// Seteaz� via�a la valoarea curent�.
		if (currentHealth <= startingHealth) {
			sliderInstance.gameObject.SetActive(true);
		}
		int sliderValue = (int) Mathf.Round(((float)currentHealth / (float)startingHealth) * 100);
		sliderInstance.value = sliderValue;
		
		// Inamicul este mort.
		if (currentHealth <= 0) {
			Death();
		}
	}

	IEnumerator Ishit() {
		Color newColor = new Color(10, 0, 0, 0);
        float newPower = 0.5f;

		myRenderer.materials[0].SetColor("_RimColor", newColor);
        myRenderer.materials[0].SetFloat("_RimPower", newPower);

        float time = 0.25f;
		float elapsedTime = 0;
		while (elapsedTime < time) {
			newColor = Color.Lerp(newColor, rimColor, elapsedTime / time);
			myRenderer.materials[0].SetColor("_RimColor", newColor);
            newPower = Mathf.Lerp(newPower, rimPower, elapsedTime / time);
            myRenderer.materials[0].SetFloat("_RimPower", newPower);
            elapsedTime += Time.deltaTime;
			yield return null;
		}
        myRenderer.materials[0].SetColor("_RimColor", rimColor);
        myRenderer.materials[0].SetFloat("_RimPower", rimPower);
    }

	void Death() {
		// Inamicul este mort.
		isDead = true;

		// Animator-ul va lua la cuno�ti�� moartea inamicului.
		anim.SetTrigger("Dead");
		
		enemyAudio.clip = deathClip;
		enemyAudio.Play();

		// G�se�te �i dezactiveaz� Nav Mesh Agent.
		if (GetComponent<NavMeshAgent>()) {
			GetComponent<NavMeshAgent>().enabled = false;
		}

        // G�se�te componenta rigidbody �i aplica-i o transformare Kinematic.
        GetComponent<Rigidbody>().isKinematic = true;

		// Actualiz�m scorul.
		scoreManager.AddScore(scoreValue);

		waveManager.enemiesAlive--;

		capsuleCollider.isTrigger = true;

		// Dizolv�m obiectul.
		StartCoroutine(StartSinking());

		Destroy(sliderInstance.gameObject);
	}

	IEnumerator StartSinking() {
		yield return new WaitForSeconds(2);

		isBurning = true;

		// Porne�te particulele.
		deathParticles.Play();

		enemyAudio.clip = burnClip;
		enemyAudio.Play();

		for (int i = 0; i < 2; i++) {
			GameObject instantiatedEye = Instantiate(eye, transform.position + new Vector3(0, 0.3f, 0), transform.rotation) as GameObject;
			instantiatedEye.GetComponent<Rigidbody>().velocity = transform.TransformDirection(new Vector3 (Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f)));
		}

		SpawnPickup();

		// Dup� 2 secunde distruge inamicul.
		Destroy(gameObject, 3f);
	}

	/** 
	 * Goodies ^_^. 
	 */
	void SpawnPickup() {
		// Acestea vor aparea usor deasupra podelei.
		Vector3 spawnPosition = transform.position + new Vector3(0, 0.3f, 0);

		// Cele 3 bonusuri vor fi raspandite random.
		// Sansele ca aceste s� apar� sunt de: 
		// - 30% bounce pickup
		// - 20% pierce pickup
		// - 50% health pickup
		float rand = Random.value;
		if (rand <= 0.2f) {
			// Bounce.
			if (rand <= 0.06f) {
				Instantiate(pickupManager.bouncePickup, spawnPosition, transform.rotation);
			}
			// Pierce.
			else if (rand > 0.06f && rand <= 0.1f) {
				Instantiate(pickupManager.piercePickup, spawnPosition, transform.rotation);
			}
			// Health.
			else {
				Instantiate(pickupManager.healthPickup, spawnPosition, transform.rotation);
			}
		}

		// Un extra pickup dupa atingerea unui scor anume.
		if (scoreManager.GetScore() >= pickupManager.scoreNeededForExtraBullet) {
			Instantiate(pickupManager.bulletPickup, spawnPosition, transform.rotation);

			pickupManager.scoreNeededForExtraBullet += pickupManager.extraScoreNeededAfterEachPickup;
		}
	}
}
