using UnityEngine;
using System.Collections;

// Setez� nivelul de luminozitate al jocului.
public class BrightnessSetting : MonoBehaviour {

    Brightness brightness;

    void Start() {
        brightness = Camera.main.GetComponent<Brightness>();
    }

    public void SetBrightness(float value) {
        brightness.brightness = value;
    }
}
