using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour {

    // Variabile
    public string pausedTitle = "PAUSED";
    public string resumeButtonText = "RESUME";

    // Referinþe la componetele Text.
    public Text titleText;
    public Text buttonText;

    // Referinþe la Canvas-ul meniului ºi interfeþei-
    Canvas menuCanvas;
    Canvas hudCanvas;

    bool gameHasStarted = false;

    void Start() {
        // Setãm referinþele
        menuCanvas = GetComponent<Canvas>();
        hudCanvas = GameObject.Find("HUDCanvas").GetComponent<Canvas>();
        hudCanvas.enabled = false;
        Time.timeScale = 0;
    }

    void Update() {
        // Dacã s-a apãsat tast ESC -> jocul se va oprii pãnã la reapãsarea tastei ESC 
        if (Input.GetKeyDown(KeyCode.Escape)) {
            gameHasStarted = true;
            Pause();
        }
    }

    // Funcþia de pauzã.
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
