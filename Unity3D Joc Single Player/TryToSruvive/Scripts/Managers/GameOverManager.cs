using UnityEngine;

public class GameOverManager : MonoBehaviour {

    // Referinþã la viaþa jucãtorului.
    public PlayerHealth playerHealth;
    // Timpul de aºteptare pãnã la resetarea jocului.
    public float restartDelay = 5f;

    // Referinþã la componenta Animator.
    Animator anim;
    // Numarã cât timp mai este pãnã la resetarea jocului.
    float restartTimer;

    void Awake() {
        // Setarea referinþelor.
        anim = GameObject.Find("HUDCanvas").GetComponent<Animator>();
    }

    void Update() {
        // Dacã jucãtorul a rãmas fãrã viaþã...
        if (playerHealth.currentHealth <= 0) {
            // ... Animator-ul va declanºa sfãrºitul jocului.
            anim.SetTrigger("GameOver");

            // ... Incrementeazã restartTimer.
            restartTimer += Time.deltaTime;

            // ... Dacã s-a ajuns atins timpul de resetare a jocului...
            if (restartTimer >= restartDelay) {
                // ... Reseteazã aplicaþia.
                Application.LoadLevel(Application.loadedLevel);
            }
        }
    }
}
