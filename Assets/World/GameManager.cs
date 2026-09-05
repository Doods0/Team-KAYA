using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class enemyEntry
{
    public GameObject enemyPrefab;
    public string enemyId;
    public int enemyPrice; // per unit
    public int enemyCapPrice; // hard cap to how much you can purchase based on currency
    public int enemyCapPerPrice; // how much will be purchasable if you meet the cap
    public int enemyPriority; // which will the game begin spending currency on

    [HideInInspector] public int enemyCap = 0; // How many of these can we buy and spawn (CHANGE DURING RUNTIME)
    [HideInInspector] public int instancesToSpawn;
    [HideInInspector] public int numberOfInstances = 0; // How many of those exist now
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Stats")]
    public float timeSpeedDecay;
    public float timeSpeedIncrease;
    

    [Header("Placeholder")]
    public GameObject enemy;
    public GameObject rangedEnemy;
    public HUDManager HUD;

    [Header("Settings")] // These are constants
    [Header("Enemy Spawnrate")]
    public float enemySpawnrate;
    public float minEnemySpawnrate;
    public float spawnrateDecreasePerPhase;
    [Header("Phase")]
    public int timeTillNextPhase;
    public int enemyCurrencyPerPhase;

    [Header("Session")]
    // Use to set up timescale externally and manually
    // AKA to be able to use Time.timeScale without this script overriding it
    // It pauses the time decay and time speeding too
    
    public bool isTimeBypassed = false;
    public float timeScale = 1f;
    
    public float timePassed = 0;
    private int currentPhase;
    private int lastComputedPhase = -1;
    private int currentEnemyCurrency;

    public List<enemyEntry> enemies = new List<enemyEntry>();
    private Dictionary<string, List<GameObject>> resourcePool = new Dictionary<string, List<GameObject>>();

    private void Update()
    {
        if (timeScale <= 0.2) TriggerGameOver();
        if (!isTimeBypassed) Time.timeScale = timeScale;

        timePassed = Time.realtimeSinceStartup;
        currentPhase = ((int)timePassed / timeTillNextPhase) + 1;

        HUD.UpdateUI(timeScale, timePassed);
    }

    private void Awake()
    {
        instance = this;

        StartCoroutine(SpawnEnemy());
        StartCoroutine(DecayTime());
    }

    public void AddInPool(string id, GameObject obj)
    {
        if (resourcePool.TryGetValue(id, out List<GameObject> objs))
        {
            resourcePool[id].Add(obj);
        }
        else
        {
            resourcePool[id] = new List<GameObject>();
            resourcePool[id].Add(obj);
        }
        enemies.Find(entry => entry.enemyId == id).numberOfInstances--;
    }

    public GameObject GetFromPool(string id)
    {
        if (resourcePool.TryGetValue(id, out List<GameObject> objs) && objs.Count > 0)
        {
            GameObject obj = objs[^1];
            objs.RemoveAt(objs.Count - 1);
            return obj;
        }
        return null;
    }

    public void UpdateDifficulty(int currentPhase)
    {
        enemySpawnrate = Mathf.Clamp(enemySpawnrate - (spawnrateDecreasePerPhase * currentPhase), minEnemySpawnrate, Mathf.Infinity);
        currentEnemyCurrency = enemyCurrencyPerPhase * currentPhase;

        int localEnemyCurrency = currentEnemyCurrency;
        foreach (enemyEntry entry in enemies)
        {
            entry.enemyCap = (localEnemyCurrency / entry.enemyCapPrice) * entry.enemyCapPerPrice;
            int affordable = localEnemyCurrency / entry.enemyPrice;
            int enemiesPurchased = Mathf.Min(entry.enemyCap, affordable);
            localEnemyCurrency -= enemiesPurchased * entry.enemyPrice;
            entry.instancesToSpawn = enemiesPurchased;
        }
    }

    public void SpeedTime()
    {
        if (!isTimeBypassed)
        {
            timeScale = (timeScale + timeSpeedIncrease) * (1 + timeSpeedIncrease);
        }
    }
    private IEnumerator DecayTime()
    {
        while (true)
        {
            if (!isTimeBypassed)
            {
                timeScale = (timeScale - timeSpeedDecay) * (1 - timeSpeedDecay);
            }

            yield return new WaitForSecondsRealtime(0.1f);
        }
    }

    private IEnumerator SpawnEnemy() // spawns based on cap
    {
        while (true)
        {
            if (currentPhase != lastComputedPhase)
            {
                lastComputedPhase = currentPhase;
                UpdateDifficulty(currentPhase);
            }

            static float RR()
            {
                float x = 0f;
                while (x == 0)
                {
                    x = Random.Range(-1f, 1f);
                }

                return x;
            }
            Vector3 offset = new Vector3(RR(), RR(), 0).normalized * Random.Range(20f, 40f);

            foreach (enemyEntry entry in enemies)
            {
                if (entry.instancesToSpawn <= entry.numberOfInstances) continue;

                GameObject toBeSpawned = GetFromPool(entry.enemyId);
                if (toBeSpawned == null)
                {
                    toBeSpawned = entry.enemyPrefab;
                    GameObject spawnedEnemy = Instantiate(toBeSpawned, GameUtils.instance.playerPosition + offset, Quaternion.identity);
                    spawnedEnemy.GetComponent<EnemyController>().id = entry.enemyId;
                }
                else
                {
                    toBeSpawned.SetActive(true);
                    EnemyController controller = toBeSpawned.GetComponent<EnemyController>();
                    controller.health = controller.maxHealth;
                    toBeSpawned.transform.position = (GameUtils.instance.playerPosition + offset);
                }
                entry.numberOfInstances++;
            }

            yield return new WaitForSecondsRealtime(enemySpawnrate);
        }
    }

    // Triggering game over is something global so it'll be fired from here
    public void TriggerGameOver()
    {
        isTimeBypassed = true;
        Time.timeScale = 0;

        HUD.ShowLossMenu();
    }
}
