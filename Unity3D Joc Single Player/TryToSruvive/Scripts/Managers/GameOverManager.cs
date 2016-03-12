using UnityEngine;

public class GameOverManager : MonoBehaviour {

    // Referin�� la via�a juc�torului.
    public PlayerHealth playerHealth;
    // Timpul de a�teptare p�n� la resetarea jocului.
    public float restartDelay = 5f;

    // Referin�� la componenta Animator.
    Animator anim;
    // Numar� c�t timp mai este p�n� la resetarea jocului.
    float restartTimer;

    void Awake() {
        // Setarea referin�elor.
        anim = GameObject.Find("HUDCanvas").GetComponent<Animator>();
    }

    void Update() {
        // Dac� juc�torul a r�mas f�r� via��...
        if (playerHealth.currentHealth <= 0) {
            // ... Animator-ul va declan�a sf�r�itul jocului.
            anim.SetTrigger("GameOver");

            // ... Incrementeaz� restartTimer.
            restartTimer += Time.deltaTime;

            // ... Dac� s-a ajuns atins timpul de resetare a jocului...
            if (restartTimer >= restartDelay) {
                // ... Reseteaz� aplica�ia.
                Application.LoadLevel(Application.loadedLevel);
            }
        }
    }
}
