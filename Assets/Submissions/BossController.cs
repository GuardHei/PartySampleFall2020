using System;
using UnityEngine;

public class BossController : MonoBehaviour
{

    public int level;
    public GameObject edgePrototype;
    public GameObject shieldPrototype;
    public float shieldRefreshTime;
    
    private Health health;

    [Header("Weapon Settings")]
    public int damage;
    public float bulletSpeed;
    public float fireInterval = .5f;
    public string damageTag = "Player";
    public int numberBullets;
    public float angle = 360f;
    
    [Header("Weapon Configurations")]
    public Transform fireOrigin;
    public GameObject bulletPrototype;

    [Header("Status (Do not modify these fields through Editor)")]
    public float lastFireTime;
    public GameObject[] shields;

    
    void Start()
    {
        health = GetComponent<Health>();
        health.onDeath.AddListener(DropEdges);
        health.onDeath.AddListener(health.SelfDestruct);
        shields = new GameObject[level];
        CreateShields();
    }

    void CreateShields()
    {
        float angleInRad = Mathf.PI * 2;
        float degreesBetween = angleInRad / (level);
        
        float ang = 0;
        for (int i = 0; i < level; i++) {
            float xDis = Mathf.Cos(ang);
            float yDis = Mathf.Sin(ang);
            Vector2 position = fireOrigin ? fireOrigin.position : transform.position;
            var shield = Instantiate(shieldPrototype, position + 2 * new Vector2(xDis, yDis), 
                transform.rotation);
            shield.GetComponent<BossShieldController>().index = i;
            shield.transform.Rotate(0, 0, ang * Mathf.Rad2Deg);
            shields[i] = shield;
            ang += degreesBetween;
        }
    }

    void Update()
    {
        if (CanFire())
        {
            Fire();
        }   
        UpdateShield();
        
    }

    void UpdateShield()
    {
        foreach (GameObject shield in shields)
        {
            BossShieldController shieldController = shield.GetComponent<BossShieldController>();
            if (shieldController.lastDeathTime != 0 && Time.time - shieldController.lastDeathTime < shieldRefreshTime)
            {
                continue;
            }

            shieldController.lastDeathTime = 0;
            shield.GetComponent<Collider2D>().enabled = true;
            shield.GetComponent<Renderer>().enabled = true;

        }
    }

    void DropEdges()
    {
        float angleInRad = Mathf.PI * 2;
        float degreesBetween = angleInRad / (level);
        
        float ang = 0;
        for (int i = 0; i < level; i++) {
            var edge = Instantiate(edgePrototype, fireOrigin ? fireOrigin.position : transform.position, transform.rotation);
            float xVel = Mathf.Cos(ang);
            float yVel = Mathf.Sin(ang);
            edge.GetComponent<Rigidbody2D>().velocity = new Vector2(xVel, yVel);
            ang += degreesBetween;

        }
    }
    
    public bool CanFire() {
        var time = Time.time;
        return lastFireTime + fireInterval <= time;
    }
    
    private void Fire() {
        lastFireTime = Time.time;
        float angleInRad = Mathf.Deg2Rad * angle;
        float startAngle = -1 * Vector2.SignedAngle(transform.up, new Vector2(1, 0)) * Mathf.Deg2Rad;
        float firstDegree = startAngle - angleInRad / 2;
        float degreesBetween = angleInRad / (numberBullets);
        
        float ang = firstDegree;
        for (int i = 0; i < numberBullets; i++) {
            var bullet = Instantiate(bulletPrototype, fireOrigin ? fireOrigin.position : transform.position, transform.rotation);
            var controller = bullet.GetComponent<BulletController>();
            controller.damage = damage;
            controller.damageTag = damageTag;
            float xVel = Mathf.Cos(ang);
            float yVel = Mathf.Sin(ang);
            controller.GetComponent<Rigidbody2D>().velocity = new Vector2(xVel, yVel) * bulletSpeed;
            ang += degreesBetween;

        }
    }

}