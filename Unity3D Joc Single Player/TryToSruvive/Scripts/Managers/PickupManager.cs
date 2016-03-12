using UnityEngine;
using System.Collections;

public class PickupManager : MonoBehaviour {
	
    // Necesarul de puncte pentru mai multe gloanţe
	public int scoreNeededForExtraBullet = 1500;
    // Necesarul de puncte pentru bonusuri.
    public int extraScoreNeededAfterEachPickup = 1500;

    // Extra viaţă.
	public Pickup healthPickup;
    // Glonţul armei va ricoşa.
    public Pickup bouncePickup;
    // Glonţul armei va străpunge adversarul.
    public Pickup piercePickup;
    // Mai multe gloanţe.
	public Pickup bulletPickup;
}
