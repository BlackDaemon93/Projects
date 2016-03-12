using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    // Viteza cu care jucătorul se va muta.
    public float speed = 6f;

    // Vectorul care va stoca direcția de mișcare a jucătorului.
    Vector3 movement;
    // Referinţă la componenta animator.
    Animator anim;    
	// Referinţă la componenta rigidbody a jucatorului.
	Rigidbody playerRigidbody;    
	// O mască a podelei jocului, astfel încat raza camerei sp focuseze doar obiectele ei.
	int floorMask;    
	// Lungimea razei de la cameră la scenă.
	float camRayLength = 100f;          

	void Awake() {
	    // Crează o mască a podelei.
	    floorMask = LayerMask.GetMask("Floor");

        // Configurarea referințelor.
        anim = GetComponent<Animator>();
	    playerRigidbody = GetComponent<Rigidbody>();
	}
	
	void FixedUpdate() {
	    // Salvează input-ul axelor.
	    float h = Input.GetAxisRaw("Horizontal");
	    float v = Input.GetAxisRaw("Vertical");

	    // Mişcarea jucătorului în scenă.
	    Move (h, v);

        // Jucătorul se întoarce acolo unde este plasat cursorul.
        Turning();

	    // Animaţia jucatorului.
	    Animating (h, v);
	}
	
	void Move(float h, float v) {
        // Setează vectorul mișcare pe baza axelor de intrare.
        movement.Set (h, 0f, v);

        // Normalizare vectorul mișcare și proporționarea acestuia cu viteza pe secundă.
        movement = movement.normalized * speed * Time.deltaTime;

	    // Mutarea jucătorului de la poziţia curentă la poziţia urmatoarea bazat calculele anterioare.
	    playerRigidbody.MovePosition(transform.position + movement);
	}
	
	void Turning() {
	    // Crează un ray al cursorului în direcţia acesta se poziţionează în direcţia camerei.
	    Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

	    // Crează un RaycastHit care stochează informaţii dacă ceva a fost lovit.
	    RaycastHit floorHit;

	    // Efectuăm un raycast dacă lovim ceva de pe podea.
	    if (Physics.Raycast (camRay, out floorHit, camRayLength, floorMask)) {
	        // Creăm un vector de la jucator la un punct de pe podea.
	        Vector3 playerToMouse = floorHit.point - transform.position;

            // Ne asigurăm că vectorul este în întregime de-a lungul planului podelei.
            playerToMouse.y = 0f;

	        // Creăm un quaternion bazat pe vectorul care se uita la jucător si punctul său de pe podea.
	        Quaternion newRotation = Quaternion.LookRotation(playerToMouse);

            // Setarea rotației jucătorului la noua rotație.
            playerRigidbody.MoveRotation(newRotation);
	    }
	}
	
	void Animating(float h, float v) {
        // Un boolean care este adevărat dacă una din axele de intrare este diferită de 0.
        bool walking = h != 0f || v != 0f;

	    // Animator-ul va lua la cunostinţă daca jucătorul se miscă sau nu.
	    anim.SetBool("IsWalking", walking);
	}
}