using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

	[Header("Settings")]
	public int healthLimit = 6;
	public int edge = 6;
	public int score;
	public float speed;
	public KeyCode upKey = KeyCode.W;
	public KeyCode downKey = KeyCode.S;
	public KeyCode leftKey = KeyCode.A;
	public KeyCode rightKey = KeyCode.D;
	public KeyCode fireKey = KeyCode.Mouse0;
	public KeyCode restartKey = KeyCode.R;
	public List<Sprite> sprites = new List<Sprite>(6);
	public List<WeaponKeyPair> weaponKeyPairs = new List<WeaponKeyPair>(9);

	[Header("Configurations")]
	public SpriteRenderer renderer;
	public Rigidbody2D rigidbody;
	public IWeapon weapon;

	[Header("Status (Do not modify these fields through Editor)")]
	public Health health;
	public float velocityX;
	public float velocityY;

	private void Start() {
		Application.targetFrameRate = 60;
		health = GetComponent<Health>();
		health.onHit.AddListener(UpdateSprite);
		health.onRecover.AddListener(UpdateSprite);
		UpdateSprite();
	}

	public void UpdateSprite() {
		int h = health.health;
		if (h <= 0) h = 1;
		else if (h > healthLimit) h = healthLimit;
		renderer.sprite = sprites[h - 1];
	}

	private void Update() {
		if (Input.GetKey(restartKey)) OnDeath();
		
		var vertical = 0f;
		var horizontal = 0f;
			
		if (Input.GetKey(upKey)) vertical = speed;
		else if (Input.GetKey(downKey)) vertical = -speed;
			
		if (Input.GetKey(leftKey)) horizontal = -speed;
		else if (Input.GetKey(rightKey)) horizontal = speed;

		if (vertical != 0 && horizontal != 0) {
			velocityX = horizontal / Mathf.Sqrt(2f);
			velocityY = vertical / Mathf.Sqrt(2f);
		} else {
			velocityX = horizontal;
			velocityY = vertical;
		}
			
		rigidbody.velocity = new Vector2(velocityX, velocityY);
		
		var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		var rotZ = Quaternion.LookRotation(weapon.transform.position - mousePos, Vector3.forward).eulerAngles.z;
		weapon.transform.rotation = Quaternion.Euler(0, 0, rotZ);

		if (Input.GetKey(fireKey)) weapon.OnTriggerPressed();
		if (Input.GetKeyUp(fireKey)) weapon.OnTriggerReleased();

		foreach (var pair in weaponKeyPairs) {
			if (pair.enabled && pair.weapon && weapon != pair.weapon) {
				if (Input.GetKeyUp(pair.keyCode)) {
					weapon.SwitchOff();
					weapon = pair.weapon;
					weapon.SwitchOn();
				}
			}
		}
	}

	public void OnDeath() {
		Debug.Log("YOU DIED !");
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void PickEdge() {
		edge++;
		if (edge > healthLimit) edge = healthLimit;
		else health.maxHealth++;
		health.Recover(health.maxHealth);
	}
}

[Serializable]
public class WeaponKeyPair {
	public KeyCode keyCode;
	public IWeapon weapon;
	public bool enabled;
}