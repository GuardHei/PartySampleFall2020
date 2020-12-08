using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseController : MonoBehaviour {
    public GameObject shoppingMenu;
    public GameObject playerBase;
    public GameObject player;
    public int radius;
    public bool canShop;
    public AudioClip purchaseSfx;
    public Image[] buttons;
    public int[] prices;
    public int playerHealthRegainPrice = 1;
    public int baseHealthRegainPrice = 2;

    private Health health;
    private bool[] purchasement = new bool[6];

    // Start is called before the first frame update
    void Start() {
        shoppingMenu.SetActive(false);
        health = GetComponent<Health>();
    }

    // Update is called once per frame
    void Update() {
        if (canShop) {
            if (Input.GetKeyUp(KeyCode.Alpha2)) PurchaseWeapon(2);
            
            if (Input.GetKeyUp(KeyCode.Alpha3)) PurchaseWeapon(3);

            if (Input.GetKeyUp(KeyCode.Alpha4)) PurchaseWeapon(4);
            
            if (Input.GetKeyUp(KeyCode.Alpha5)) PurchaseWeapon(5);
            
            if (Input.GetKeyUp(KeyCode.Alpha6)) PurchaseWeapon(6);

            if (Input.GetKeyUp(KeyCode.Alpha7)) PurchaseWeapon(7);
            
            if (Input.GetKeyUp(KeyCode.Alpha8)) RegainPlayerHealth();

            if (Input.GetKeyUp(KeyCode.Alpha9)) RegainBaseHealth();

            if (Input.GetKeyUp(KeyCode.Alpha0)) SelfDestroy();
        }
        
        foreach (Collider2D col in Physics2D.OverlapCircleAll(playerBase.transform.position, radius)) {
            if (col.Equals(player.GetComponent<Collider2D>())) {
                if (shoppingMenu != null) {
                    shoppingMenu.SetActive(true);
                    canShop = true;
                }
                // Debug.Log("hi");
                return;
            } 
        }

        // Debug.Log("goodbye");

        if (shoppingMenu != null) {
            shoppingMenu.SetActive(false);
            canShop = false;
        }
    }

    public bool CheckPayment(int cost) {
        var p = player.GetComponent<PlayerController>();
        if (p.score >= cost) {
            p.score -= cost;
            return true;
        }

        return false;
    }

    public void PurchaseWeapon(int index) {
        int pIndex = index - 2;
        int wIndex = index - 1;
        if (purchasement[pIndex]) return;
        if (CheckPayment(prices[pIndex])) {
            AudioManager.PlayAtPoint(purchaseSfx, transform.position);
            var p = player.GetComponent<PlayerController>();
            p.weaponKeyPairs[wIndex].enabled = true;
            purchasement[pIndex] = true;
            GreyOut(buttons[pIndex]);
        }
    }

    public void GreyOut(Image img) {
        img.color = new Color(70f / 255f, 70f / 255f, 70f / 255f, img.color.a);
    }

    public void RegainPlayerHealth() {
        var p = player.GetComponent<PlayerController>();
        if (p.health.health < p.health.maxHealth) {
            if (CheckPayment(playerHealthRegainPrice)) {
                AudioManager.PlayAtPoint(purchaseSfx, transform.position);
                p.health.Recover(1);
            }
        }
    }

    public void RegainBaseHealth() {
        var p = player.GetComponent<PlayerController>();
        if (health.health < health.maxHealth) {
            if (CheckPayment(baseHealthRegainPrice)) {
                AudioManager.PlayAtPoint(purchaseSfx, transform.position);
                health.Recover(5);
            }
        }
    }

    public void SelfDestroy() {
        Destroy(gameObject);
    }
}
