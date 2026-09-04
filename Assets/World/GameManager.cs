using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Settings")]
    public float timeSpeedDecay;
    public float timeSpeedIncrease;

    [Header("Placeholder")]
    public Object enemy;
    public Object rangedEnemy;
    public HUDManager HUD;

    [Header("Session")]
    // Use to set up timescale externally and manually
    // AKA to be able to use Time.timeScale without this script overriding it
    // It pauses the time decay and time speeding too
    public bool isTimeBypassed = false;
    public float timeScale = 1f;

    private void Update() 
    {
        if (timeScale <= 0.1) TriggerGameOver();
        if (!isTimeBypassed) Time.timeScale = timeScale;
        HUD.UpdateTimeScale(timeScale);
    }

    private void Awake()
    {
        instance = this;

        InvokeRepeating(nameof(SpawnEnemy), 3.0f, 1.0f);
        InvokeRepeating(nameof(DecayTime), 0f, 1f);
    }

    public void SpeedTime()
    {
        if (!isTimeBypassed)
        {
            timeScale = (timeScale + timeSpeedIncrease) * (1 + timeSpeedIncrease);
        }
    }
    // Changing of game speed should scale with current game speed.
    private void DecayTime()
    {
        if (!isTimeBypassed)
        {
            timeScale = (timeScale - timeSpeedDecay) * (1 - timeSpeedDecay);
        }
    }
    // Changing of game speed should scale with current game speed.

    private void SpawnEnemy()
    {
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

        Object toBeSpawned;
        if (Random.Range(0, 2) == 1) toBeSpawned = enemy;
        else toBeSpawned = rangedEnemy;

        Instantiate(toBeSpawned, GameUtils.instance.playerPosition + offset, Quaternion.identity);

    }

    // Triggering game over is something global so it'll be fired from here
    public void TriggerGameOver()
    {
        instance.isTimeBypassed = true;
        Time.timeScale = 0;

        HUD.ShowLossMenu();
    }
}
