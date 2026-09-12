using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class EnemyEntry
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
    public HUDManager HUD;

    [Header("Settings")] // These are constants
    [Header("Enemy Spawnrate")]
    public float enemySpawnrate;
    public float minEnemySpawnrate;
    public float spawnrateDecreasePerPhase;
    [Header("Phase")]
    public int timeTillNextPhase;
    public int enemyCurrencyPerPhase;
    [Header("Pickups")]
    public float chanceOfPickup;
    public GameObject pointPickup;
    public string pointPickupId;
    public PickupChance[] specialPickups;

    [Header("Sounds")]
    public AudioClip deathSound;

    [Header("Session")]
    // Use to set up timescale externally and manually
    // AKA to be able to use Time.timeScale without this script overriding it
    // It pauses the time decay and time speeding too

    public bool isTimeBypassed = false;
    public float runtimeScale = 1f;
    public float maxTimeScale;
    public float timeScale;

    public float timePassed = 0;
    private int currentPhase;
    private int lastComputedPhase = -1;
    private int currentEnemyCurrency;

    public List<EnemyEntry> enemies = new();
    private readonly Dictionary<string, List<GameObject>> resourcePool = new();

    private void Update()
    {
        if (runtimeScale <= 0.2) TriggerGameOver();
        runtimeScale = Mathf.Clamp(runtimeScale, 0, maxTimeScale);
        if (!isTimeBypassed) timeScale = runtimeScale;

        if (timeScale != 0) timePassed += Time.deltaTime * timeScale;
        currentPhase = ((int)timePassed / timeTillNextPhase) + 1;

        HUD.UpdateUI(runtimeScale, timePassed);
    }

    private async void Awake()
    {
        instance = this;
        await StartGame();
    }

    private async Task StartGame()
    {
        isTimeBypassed = true;
        timeScale = 0;

        await HUD.PlayIntro();

        isTimeBypassed = false;

        StartCoroutine(SpawnEnemy());
        StartCoroutine(DecayTime());
    }

    #region Resource Pooling

    public void AddInPool(string id, GameObject obj)
    {
        if (!resourcePool.TryGetValue(id, out List<GameObject> objs)) resourcePool[id] = new();
        resourcePool[id].Add(obj);

        EnemyEntry enemy = enemies.Find(entry => entry.enemyId == id);
        if (enemy is not null) enemy.numberOfInstances--;
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

    #endregion

    #region Time Manipulation

    public void SpeedTime()
    {
        if (!isTimeBypassed) runtimeScale = (runtimeScale + timeSpeedIncrease) * (1 + timeSpeedIncrease);
    }
    private IEnumerator DecayTime()
    {
        while (true)
        {
            if (!isTimeBypassed) runtimeScale = (runtimeScale - timeSpeedDecay) * (1 - timeSpeedDecay);

            yield return new WaitForSecondsRealtime(0.1f);
        }
    }

    #endregion

    #region Difficulty Escalation

    public void UpdateDifficulty(int currentPhase)
    {
        enemySpawnrate = Mathf.Clamp(enemySpawnrate - (spawnrateDecreasePerPhase * currentPhase), minEnemySpawnrate, Mathf.Infinity);
        currentEnemyCurrency = enemyCurrencyPerPhase * currentPhase;

        int localEnemyCurrency = currentEnemyCurrency;
        foreach (EnemyEntry entry in enemies)
        {
            // If (localEnemyCurrency < entry.enemyCapPrice) we will get a 0, as they're both int.
            entry.enemyCap = localEnemyCurrency / entry.enemyCapPrice * entry.enemyCapPerPrice;
            int affordable = localEnemyCurrency / entry.enemyPrice;
            int enemiesPurchased = Mathf.Min(entry.enemyCap, affordable);
            localEnemyCurrency -= enemiesPurchased * entry.enemyPrice;
            entry.instancesToSpawn = enemiesPurchased;
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
                while (x == 0) x = Random.Range(-1f, 1f);
                return x;
            }
            Vector3 offset = new Vector3(RR(), RR(), 0).normalized * Random.Range(20f, 40f);

            foreach (EnemyEntry entry in enemies)
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
                    toBeSpawned.transform.position = GameUtils.instance.playerPosition + offset;
                }
                entry.numberOfInstances++;
            }

            yield return new WaitForSecondsRealtime(enemySpawnrate);
        }
    }

    #endregion

    // Triggering game over is something global so it'll be fired from here
    public void TriggerGameOver() => StartCoroutine(OnGameOver());

    public IEnumerator OnGameOver()
    {
        GameUtils.instance.audioSource.PlayOneShot(deathSound);

        isTimeBypassed = true;
        timeScale = 1;


        yield return new WaitForSeconds(2f);

        HUD.PlayLossAnimations();
    }

    private void OnDestroy() { if (instance == this) instance = null; }
}
