using System;
using UnityEngine;

public class EdgeBehavior : MonoBehaviour {

	public AudioClip pickupSfx;
	
    void OnCollisionEnter2D(Collision2D other) {
	    if (other.collider.CompareTag("Player")) {
		    other.gameObject.GetComponent<PlayerController>().PickEdge();
		    Debug.Log(other.gameObject.GetComponents<PlayerController>()[0].edge);
		    AudioManager.PlayAtPoint(pickupSfx, transform.position);
		    Destroy(gameObject);   
	    }
    }
}
