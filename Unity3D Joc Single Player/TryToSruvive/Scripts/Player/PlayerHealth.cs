using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour {

	// Viaţa jucatorului la inceput.
	public int startingHealth = 100;  
	// Viaţa curentă a jucatorului.
	public int currentHealth;
    // Timpul în secunde după ce am luat ultima daună înainte de a putea fi atacat din nou.
	public float invulnerabilityTime = 1f;
    // Timpul în secunde, înainte ca bara de viaţă se va modifica în funcţie de daunele încasate.
    public float timeAfterWeLastTookDamage = 1f;  
	// Referinţă la bara verde de viaţă.
	public Slider healthSliderForeground;
    // Referinţă la bara rosie de viaţă.
    public Slider healthSliderBackground;   
	// Referinţă la imaginea care se va desfăsura atunci când suntem atacaţi.
	public Image damageImage;      
	// Sunetul care se va auzi atunci când jucatorul va muri.
	public AudioClip deathClip;         
	// Viteza cu care damageImage va disparea.
	public float flashSpeed = 5f;     
	// Culoarea setată damageImage-ului.
	public Color flashColour = new Color(1f, 0f, 0f, 0.1f);     

	// Referinţă la componenta Animator.
	Animator anim;
    // Referinţă la componenta AudioSource.
    AudioSource playerAudio;
    // Referinţă la mişcarea jucatorului.
    PlayerMovement playerMovement;
    // Referinţă la scriptul pe care jucaorul îl folosete când trage cu arma.
    PlayerShooting playerShooting;
    // Dacă jucătorul este mort.
    bool isDead;
    // Adevărat atunci când jucatorul este atacat.
    bool damaged;          
	// Daunele acumulate în time frame-ul curent.
	float timer;
	SkinnedMeshRenderer myRenderer;
    // Culoarea Shader-ului. O să schimbăm această culoare când o să simulam efectul de a fi lovit.
    Color rimColor;
    float rimPower;

    void Awake() {
		// Setăm referinţele
		anim = GetComponent<Animator>();
		playerAudio = GetComponent<AudioSource>();
		playerMovement = GetComponent<PlayerMovement>();
		playerShooting = GetComponentInChildren<PlayerShooting>();
		
		// Setăm viaţa iniţiala a jucatorului.
		currentHealth = startingHealth;

		// Luam SkinnedMeshRenderer-ul jucatorului.
		SkinnedMeshRenderer[] meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
		foreach (SkinnedMeshRenderer meshRenderer in meshRenderers) {
			if (meshRenderer.gameObject.name == "Player") {
				myRenderer = meshRenderer;
				break;
			}
		}
	}

	void Start() {
        // Culoarea şi intensitatea materialului.
        rimColor = myRenderer.materials[0].GetColor("_RimColor");
        rimPower = myRenderer.materials[0].GetFloat("_RimPower");
    }
	
	void Update() {
		// Dacă jucătorul a încasat daune...
		if (damaged) {
			// ... setam culoarea damageImage-ului.
			damageImage.color = flashColour;
		}
		// Altfel...
		else {
			// ... Schimbăm proprietatea culorii damageImage-ului înapoi la "clean".
			damageImage.color = Color.Lerp (damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
		}

        // Actualizăm timpul de la ultimul Update.
        timer += Time.deltaTime;

        // Dacă timer depășește timpul dintre atacuris, player-ul este în raza de acțiune și inamicul este în viață, atacă.
        if (timer >= timeAfterWeLastTookDamage) {
			healthSliderBackground.value = Mathf.Lerp(healthSliderBackground.value, healthSliderForeground.value, 2 * Time.deltaTime);
		}

		// Resetam dacă jucatorul a încasat daunele.
		damaged = false;
	}
	
	
	public void TakeDamage(int amount) {
		if (timer < invulnerabilityTime) {
			return;
		}

        StopCoroutine("Ishit");
        StartCoroutine("Ishit");

		damaged = true;

        // Scade viaţa curentă a jucătorului în funcţie de daunele încasate.
        currentHealth -= amount;

		if (currentHealth > startingHealth) {
			currentHealth = startingHealth;
		}
		
		// Actualizează bara de viaţa.
		healthSliderForeground.value = currentHealth;

		// Daunele acumulate.
		timer = 0;
		
		// Efectul sonor atunci când jucătorul încasează daune.
		playerAudio.Play ();

        // Dacă jucatorul a rămas fără viaţă, iar animaţia morţii sale nu a fost afişată...
        if (currentHealth <= 0 && !isDead) {
			// ... desfăşoară animaţia.
			Death();
		}
	}

    //Calculul culorii atunci cand jucătorul încasează daune.
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

    // Adăugam viaţă jucătorului atunci când strânge viaţa cazută de la inamici.
    public void AddHealth(int amount) {
		currentHealth += amount;
		
		if (currentHealth > startingHealth) {
			currentHealth = startingHealth;
		}

        // Actualizează bara de viaţa..
        healthSliderForeground.value = currentHealth;
	}
	
	
	void Death() {
		isDead = true;
		
		// Opresc efectele armei.
		playerShooting.DisableEffects ();
		
		// Animatorul va recunoaşte că jucatorul este mort.
		anim.SetTrigger ("Die");
		
		// Setez sunetul şi animaţia care va rula atunci când jucatorul a murit.
		playerAudio.clip = deathClip;
		playerAudio.Play ();
		
		// Opresc scripturile de mişcare şi tragere.
		playerMovement.enabled = false;
		playerShooting.enabled = false;
	}	
}
