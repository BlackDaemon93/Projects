using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour {

    // Variabile
    public string pausedTitle = "PAUSED";
    public string resumeButtonText = "RESUME";

    // Referin�e la componetele Text.
    public Text titleText;
    public Text buttonText;

    // Referin�e la Canvas-ul meniului �i interfe�ei-
    Canvas menuCanvas;
    Canvas hudCanvas;

    bool gameHasStarted = false;

    void Start() {
        // Set�m referin�ele
        menuCanvas = GetComponent<Canvas>();
        hudCanvas = GameObject.Find("HUDCanvas").GetComponent<Canvas>();
        hudCanvas.enabled = false;
        Time.timeScale = 0;
    }

    void Update() {
        // Dac� s-a ap�sat tast ESC -> jocul se va oprii p�n� la reap�sarea tastei ESC 
        if (Input.GetKeyDown(KeyCode.Escape)) {
            gameHasStarted = true;
            Pause();
        }
    }

    // Func�ia de pauz�.
    public void Pause() {
        menuCanvas.enabled = !menuCanvas.enabled;
        hudCanvas.enabled = !hudCanvas.enabled;

        Time.timeScale = Time.timeScale == 0 ? 1 : 0;

        if (gameHasStarted) {
            titleText.text = pausedTitle;
            buttonText.text = resumeButtonText;
        }
    }
}
