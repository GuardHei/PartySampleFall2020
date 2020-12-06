using System;
using System.Diagnostics;
using UnityEngine;

public class BossShieldController : MonoBehaviour
{

    public GameObject boss;
    public float angleSpeed;
    public Vector3 relativeLoc;
    public int index;
    private Health health;
    public float lastDeathTime;

    void Start()
    {
        relativeLoc = transform.position - boss.transform.position;
        health = GetComponent<Health>();
        health.onDeath.AddListener(Die);
        lastDeathTime = 0;
    }
    
    void Update()
    {
        if (boss == null)
        {
            Destroy(gameObject);
            return;
        }
        
        float angle = angleSpeed * Time.deltaTime;
        Quaternion qangle = Quaternion.Euler(0, 0, angle);
        transform.position = boss.transform.position + relativeLoc;
        transform.position = qangle * (transform.position - boss.transform.position) + boss.transform.position;
        // transform.RotateAround(boss.transform.position, transform.forward, angle);
        transform.Rotate(0, 0, angle);
        relativeLoc = transform.position - boss.transform.position;
    }

    void Die()
    {
        gameObject.GetComponent<Renderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        lastDeathTime = Time.time;
    }

}