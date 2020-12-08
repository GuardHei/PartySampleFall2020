using System;
using UnityEngine;

[ExecuteInEditMode]
public class BulletController : MonoBehaviour {
	
	public string damageTag = "Player";
	public int damage;

	private void OnTriggerEnter2D(Collider2D other) {
		// the bullet disappears if it hits walls/obstacles or damages the object if it fits the damageTag
		if (other.gameObject.layer == LayerMask.NameToLayer("Environment") || other.CompareTag(damageTag)) {
			if (other.TryGetComponent(out Health health)) {
				health.OnBulletHit(damage);
			}
			
			Destroy(gameObject);
		}
	}
}
