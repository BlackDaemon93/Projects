using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyHealth : MonoBehaviour {

    // Viaþa inamicului la inceput.
    public int startingHealth = 100;
    // Viaþa curentã a adversarului.
    [HideInInspector]
	public int currentHealth;  
	// Viteza cu care acesta se va dizolva dupa moarte.
	public float sinkSpeed = 2.5f;   
	// Valoarea în puncte ce va fi adãugata la scor când inamicul va muri. 
	public int scoreValue = 10; 
	// Sunetul facut de acesta când moare.
	public AudioClip deathClip;    
	// Sunetul facut de acesta cand arde.
	public AudioClip burnClip;  
	// Sistemul de particule.
	public ParticleSystem deathParticles;  
	// Viaþa afiºata deasupra fiecãrui adversar.
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

	// Componete ºi scripuri la care ne vom referii.
	Animator anim;            
	AudioSource enemyAudio;        
	CapsuleCollider capsuleCollider;   
	SkinnedMeshRenderer myRenderer;
	GameObject enemyHealthbarManager;
	WaveManager waveManager;
	ScoreManager scoreManager;
	PickupManager pickupManager;

	void Awake() {
		// Setãm referinþele.
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
		// Setãm viaþa inamicilor la apariþia acestora.
		currentHealth = startingHealth;

		// Instanþiem bara de viaþã.
		sliderInstance = Instantiate(healthBarSlider, gameObject.transform.position, Quaternion.identity) as Slider;
		sliderInstance.gameObject.transform.SetParent(enemyHealthbarManager.transform, false);
		sliderInstance.GetComponent<Healthbar>().enemy = gameObject;
		sliderInstance.gameObject.SetActive(false);

		// Culoarea ºi intensitea materialului.
		rimColor = myRenderer.materials[0].GetColor("_RimColor");
        rimPower = myRenderer.materials[0].GetFloat("_RimPower");
    }

	void Update() {
		// Animaþia de dizolvare a inamicului.
		if (isBurning) {
			cutoffValue = Mathf.Lerp(cutoffValue, 1f, 1.3f * Time.deltaTime);
			myRenderer.materials[0].SetFloat("_Cutoff", cutoffValue);
			myRenderer.materials[1].SetFloat("_Cutoff", 1);
		}
	}

	public void TakeDamage(int amount, Vector3 hitPoint) {
        StopCoroutine("Ishit");
        StartCoroutine("Ishit");

		// Dacã inamicul este mort, ieºi din funcþie.
		if (isDead)
			return;

		GetComponent<Rigidbody>().AddForceAtPosition(transform.forward * -300, hitPoint);
		
		// Reduce viaþa în funcþie de daunele încasate.
		currentHealth -= amount;

		// Seteazã viaþa la valoarea curentã.
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

		// Animator-ul va lua la cunoºtiþã moartea inamicului.
		anim.SetTrigger("Dead");
		
		enemyAudio.clip = deathClip;
		enemyAudio.Play();

		// Gãseºte ºi dezactiveazã Nav Mesh Agent.
		if (GetComponent<NavMeshAgent>()) {
			GetComponent<NavMeshAgent>().enabled = false;
		}

        // Gãseºte componenta rigidbody ºi aplica-i o transformare Kinematic.
        GetComponent<Rigidbody>().isKinematic = true;

		// Actualizãm scorul.
		scoreManager.AddScore(scoreValue);

		waveManager.enemiesAlive--;

		capsuleCollider.isTrigger = true;

		// Dizolvãm obiectul.
		StartCoroutine(StartSinking());

		Destroy(sliderInstance.gameObject);
	}

	IEnumerator StartSinking() {
		yield return new WaitForSeconds(2);

		isBurning = true;

		// Porneºte particulele.
		deathParticles.Play();

		enemyAudio.clip = burnClip;
		enemyAudio.Play();

		for (int i = 0; i < 2; i++) {
			GameObject instantiatedEye = Instantiate(eye, transform.position + new Vector3(0, 0.3f, 0), transform.rotation) as GameObject;
			instantiatedEye.GetComponent<Rigidbody>().velocity = transform.TransformDirection(new Vector3 (Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f)));
		}

		SpawnPickup();

		// Dupã 2 secunde distruge inamicul.
		Destroy(gameObject, 3f);
	}

	/** 
	 * Goodies ^_^. 
	 */
	void SpawnPickup() {
		// Acestea vor aparea usor deasupra podelei.
		Vector3 spawnPosition = transform.position + new Vector3(0, 0.3f, 0);

		// Cele 3 bonusuri vor fi raspandite random.
		// Sansele ca aceste sã aparã sunt de: 
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
