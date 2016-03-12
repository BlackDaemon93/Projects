using UnityEngine;
using UnityEngine.UI;

public class PlayerShooting : MonoBehaviour {

    // Culoarea gloanţelor.
    public Color[] bulletColors;
    // Cat timp glontele se reflectă. 
    public float bounceDuration = 10;
    // Cat timp glontele străpunge adversarul.
    public float pierceDuration = 10;
    // Daunele facute de fiecare glonţ.
    public int damagePerShot = 20;
    // Numarul gloanţelor
    public int numberOfBullets = 1;
    // Timpul dintre fiecare atac.
    public float timeBetweenBullets = 0.15f;
    public float angleBetweenBullets = 10f;
    // Distanţa de tragere a armei.
    public float range = 100f;
    // O mască a raycast-ului care poate lovi doar obiecte în care se poate trage.
    public LayerMask shootableMask;
    // Refetinţă la imaginea glontelui care se reflectă.
    public Image bounceImage;
    // Refetinţă la imaginea glontelui care străpunge adeversarul.
    public Image pierceImage;
    // Glontele
    public GameObject bullet;
    public Transform bulletSpawnAnchor;

    // Timpul care determina când tragem.
    float timer;
    // O raza de la baza armei la vărful acesteia.
    Ray shootRay;
    // Un raycasthit pentru a obține informații cu privire la ceea ce a fost lovit.
    RaycastHit shootHit;
    // Referinţă la sistemul de particule.
    ParticleSystem gunParticles;
    // Referinţă la LineRenderer.
    LineRenderer gunLine;
    // Referinţă la AudioSource.
    AudioSource gunAudio;
    // Referinţă la componeta de lumină a armei.
    Light gunLight;
    // Durata efectului.
    float effectsDisplayTime = 0.2f;
    float bounceTimer;
    float pierceTimer;
    bool bounce;
    bool piercing;
    Color bulletColor;

    public float BounceTimer {
        get { return bounceTimer; }
        set { bounceTimer = value; }
    }

    public float PierceTimer {
        get { return pierceTimer; }
        set { pierceTimer = value; }
    }

    void Awake() {
        // Setam referinţele.
        gunParticles = GetComponent<ParticleSystem>();
        gunAudio = GetComponent<AudioSource>();
        gunLight = GetComponentInChildren<Light>();

        bounceTimer = bounceDuration;
        pierceTimer = pierceDuration;
    }

    void Update() {
        //Identificăm ce tip de efect are arma în momentul în care tragem.
        if (bounceTimer < bounceDuration) {
            bounce = true;
        }
        else {
            bounce = false;
        }

        if (pierceTimer < pierceDuration) {
            piercing = true;
        }
        else {
            piercing = false;
        }

        bulletColor = bulletColors[0];
        if (bounce) {
            bulletColor = bulletColors[1];
            bounceImage.color = bulletColors[1];
        }
        bounceImage.gameObject.SetActive(bounce);

        if (piercing) {
            bulletColor = bulletColors[2];
            pierceImage.color = bulletColors[2];
        }
        pierceImage.gameObject.SetActive(piercing);

        if (piercing & bounce) {
            bulletColor = bulletColors[3];
            bounceImage.color = bulletColors[3];
            pierceImage.color = bulletColors[3];
        }

        //Particulele pentru efectele armei.
        gunParticles.startColor = bulletColor;
        gunLight.color = (piercing & bounce) ? new Color(1, 140f / 255f, 30f / 255f, 1) : bulletColor;

        // Actualizăm timpul de la ultimul Update.
        bounceTimer += Time.deltaTime;
        pierceTimer += Time.deltaTime;
        timer += Time.deltaTime;

        // Dacă s-a apăsat Fire1 atunci începe să tragi...
        if (Input.GetButton("Fire1") && timer >= timeBetweenBullets && Time.timeScale != 0) {
            // ... trage cu arma.
            Shoot();
        }

        // Dacă timer a depăşit timeBetweenBullets atunci rulăm efectele...
        if (timer >= timeBetweenBullets * effectsDisplayTime) {
            // ... dezactivăm efectele.
            DisableEffects();
        }
    }

    public void DisableEffects() {
        // Dezactivăm efectele de lumină.
        gunLight.enabled = false;
    }

    void Shoot() {
        // Resetăm timpul.
        timer = 0f;

        // Sunetul armei când trage în toate cele 3 cazuri.
        gunAudio.pitch = Random.Range(1.2f, 1.3f);
        if (bounce) {
            gunAudio.pitch = Random.Range(1.1f, 1.2f);
        }
        if (piercing) {
            gunAudio.pitch = Random.Range(1.0f, 1.1f);
        }
        if (piercing & bounce) {
            gunAudio.pitch = Random.Range(0.9f, 1.0f);
        }
        gunAudio.Play();

        // Activăm luminile.
        gunLight.intensity = 2 + (0.25f * (numberOfBullets - 1));
        gunLight.enabled = true;

        // Oprim particulele dacă acestea sunt active, după le ativăm cu noile valori.
        gunParticles.Stop();
        gunParticles.startSize = 1 + (0.1f * (numberOfBullets - 1));
        gunParticles.Play();

        // Setăm originea si direcţia shootRay-ului
        shootRay.origin = transform.position;
        shootRay.direction = transform.forward;

        for (int i = 0; i < numberOfBullets; i++) {
            // Ne asiguram că gloanţele sunt raspăndite în mod egal.
            float angle = i * angleBetweenBullets - ((angleBetweenBullets / 2) * (numberOfBullets - 1));
            Quaternion rot = transform.rotation * Quaternion.AngleAxis(angle, Vector3.up);
            GameObject instantiatedBullet = Instantiate(bullet, bulletSpawnAnchor.transform.position, rot) as GameObject;
            instantiatedBullet.GetComponent<Bullet>().piercing = piercing;
            instantiatedBullet.GetComponent<Bullet>().bounce = bounce;
            instantiatedBullet.GetComponent<Bullet>().bulletColor = bulletColor;
        }
    }
}
