using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreeperController : MonoBehaviour {

    public float encounterTime;
    public float radius;
    public float explodeRadius = 2.5f;
    public int damage;
    public Color explodeColor = Color.red;
    private float firstEncounterTime = -1f;
    private BasicEnemyMovement movementController;
    private float slowSpeed;
    private Health health;

    // Start is called before the first frame update
    private void Start() {
        health = GetComponent<Health>();
        health.onDeath.AddListener(Explode);
        health.onDeath.AddListener(health.SelfDestruct);
        
        movementController = GetComponent<BasicEnemyMovement>();
        slowSpeed = movementController.speed;
    }


    // Update is called once per frame
    private void Update() {
        //check timer
        if (firstEncounterTime >= 0f && Time.time - firstEncounterTime >= encounterTime) {
            health.OnBulletHit(health.health + 1);
        }
        
        if (movementController != null) {
            if (movementController.distanceSqrToTarget <= radius * radius && movementController.distanceSqrToTarget != 0) {
                // Debug.Log(movementController.distanceSqrToTarget + " " + radius * radius);
                movementController.speed = slowSpeed * 1.5f;
                if (firstEncounterTime < 0) {
                    firstEncounterTime = Time.time;
                    GetComponent<SpriteRenderer>().color = explodeColor;
                }
            } else {
                movementController.speed = slowSpeed;
            }
        }
    }
    
    public void Explode() {
        foreach (Collider2D col in Physics2D.OverlapCircleAll(transform.position, explodeRadius)) {
            if (!(col.gameObject == gameObject) && (col.CompareTag("Player") || col.CompareTag("Enemy"))) {
                if (col.TryGetComponent(out Health health)) {
                    health.OnBulletHit(damage);
                }
            } 
        }
    }

}
