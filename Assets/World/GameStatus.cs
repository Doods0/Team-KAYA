using UnityEngine;

public class GameStatus : MonoBehaviour
{
    public static GameStatus instance;
    public LayerMask enemyLayer;

    public Transform playerTransform;
    public Object enemy;

    public void Awake()
    {
        instance = this;
        //InvokeRepeating(nameof(SpawnEnemy), 3.0f, 1.0f);
    }

    void SpawnEnemy()
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
        Instantiate(enemy, playerTransform.position + offset, Quaternion.identity);
    }

}
