using UnityEngine;
using System.Collections;

public class VolumeMute : MonoBehaviour {

	// Controlul volumului aplicažiei.
    public float volume = 1.0f;

	void Start () {
		AudioListener.volume = volume;
	}
}
