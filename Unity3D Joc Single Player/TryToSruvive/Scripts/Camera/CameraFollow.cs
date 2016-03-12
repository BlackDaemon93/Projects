using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	// Poziţia obiectului pe care camera o va urmării.
	public Transform target; 
	// Viteza cu care camera se va deplasa.
	public float smoothing = 5f;        

	// Iniţializarea offset-ului.
	Vector3 offset;                     

	void Start() {
		// Calculează offset-ul iniţial.
		offset = transform.position - target.position;
	}

	void FixedUpdate () {
		// Crează poziţia camerei bazat pe offset-ul obiectului urmarit.
		Vector3 targetCamPos = target.position + offset;

        // Interpolare lină între poziția curentă a camerei și poziția țintei.
        transform.position = Vector3.Lerp (transform.position, targetCamPos, smoothing * Time.deltaTime);
	}
}