using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class WaveGenerator : MonoBehaviour {

    public static WaveGenerator instance;
    
    [Serializable]
    public class WavePattern {
        // public int numOfEnemy;
        // public GameObject enemyType;
        public void Init() {
            // this.enemies = enemies;
            // this.numOfEnemy = numOfEnemy;
            totalEnemies = numOfEnemy.Sum();
            deadEnemies = 0;
        }
        
        [SerializeField]
        public List<GameObject> enemies;
        [SerializeField]
        public List<int> numOfEnemy;
        
        public int deadEnemies;
        public int totalEnemies;
    }

    public WaveEnumerator waveEnumerator;
    public int totalWave = 7;
    public float waveInterval = 5f;
    private bool canStartWave;
    private bool isWaiting;
    private int waveCount;
    private WavePattern pattern;
    private List<GameObject> spawnPoints = new List<GameObject>();
    private List<int> usedSpawnPoint = new List<int>();
    public GameObject player;
    public GameObject playerBase;
    public GameObject waveStartUI;
    public GameObject waveFinishUI;
    public GameObject finishUI;
    public SpriteRenderer playerRenderer;
    public Sprite perfectCircle;

    // Start is called before the first frame update
    void Start() {
        instance = this;
        foreach (GameObject spawnPoint in GameObject.FindGameObjectsWithTag("Respawn")) spawnPoints.Add(spawnPoint);
        StartCoroutine(DisplayUI(waveStartUI, 3));
        LoadNextWave();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyUp(KeyCode.Q)) {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        
        if (pattern == null) return;
        if (pattern.totalEnemies == pattern.deadEnemies) { //if there are no more enemies
            // Debug.Log(pattern.totalEnemies);
            if (!canStartWave) {
                if (!isWaiting) {
                    isWaiting = true;
                    Debug.Log("Wave finishes");
                    StartCoroutine(DisplayUI(waveFinishUI, 3));
                    StartCoroutine(WaitForNextWaveToStart());
                }
            } else {
                canStartWave = false;
                isWaiting = false;
                StartCoroutine(DisplayUI(waveStartUI, 3));
                LoadNextWave();
            }
        }
    }

    public IEnumerator WaitForNextWaveToStart() {
        yield return new WaitForSeconds(waveInterval);
        canStartWave = true;
    }

    public IEnumerator DisplayUI(GameObject ui, float duration) {
        ui.SetActive(true);
        yield return new WaitForSeconds(duration);
        ui.SetActive(false);
    }

    public void LoadNextWave() {
        pattern = waveEnumerator.NextWave();
        waveCount += 1;
        if (waveCount > totalWave) {
            finishUI?.SetActive(true);
            if (playerRenderer) {
                playerRenderer.sprite = perfectCircle;
                playerRenderer.transform.localScale = new Vector3(2, 2, 2);
            }
            return;
        }
        
        for (int i = 0; i < pattern.enemies.Count; i++) Spawn(i);
    }
    
    public void Spawn(int index) {
        int numOfEnemy = pattern.numOfEnemy[index];
        for (int i = 0; i < numOfEnemy; i++) {
            GameObject spawnPoint =  ChooseSpawnPoint();
            var enemy = Instantiate(pattern.enemies[index], spawnPoint.transform.position, spawnPoint.transform.rotation);
            var controller = enemy.GetComponent<BasicEnemyMovement>();
            controller.strategy =  (TargetStrategy) UnityEngine.Random.Range(0, 2);
            controller.player = player.transform;
            if (playerBase) {
                controller.playerBase = playerBase.transform;
            }
            controller.rigidbody = enemy.GetComponent<Rigidbody2D>();
            var health = enemy.GetComponent<Health>();
            health.onDeath.AddListener(DecreaseEnemies);
        }
        usedSpawnPoint.Clear();
    }

    public GameObject ChooseSpawnPoint() {
        int numOfAll = spawnPoints.Count;
        int numOfUnused = numOfAll - usedSpawnPoint.Count;
        int r = UnityEngine.Random.Range(0, numOfUnused);
        int counter = 0;
        int assigned;
        for (assigned = r;
            usedSpawnPoint.Contains(assigned) || counter < numOfAll;
            counter++, assigned = (assigned + 1) % numOfAll);
        usedSpawnPoint.Add(assigned);
        return spawnPoints[assigned];
    }

    private void DecreaseEnemies() {
        pattern.deadEnemies++;
    }

    [Serializable]
    public class WaveEnumerator {
        [SerializeField] public WavePattern[] waves;
        private int index;

        public WaveEnumerator() => index = 0;

        public WavePattern NextWave() {
            WavePattern pattern = index == waves.Length ? null : waves[index++];
            pattern?.Init();
            return pattern;
        }

        public int NumWaves() => waves.Length;
    }

    public static void AddScore(int number) {
        instance.player.GetComponent<PlayerController>().score += number;
    }
}
