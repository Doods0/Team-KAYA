using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        Vector3 playerPosition = GameStatus.instance.playerTransform.position;
        Vector3 position = transform.position;
        Vector3 moveDirection = (playerPosition - position).normalized;

        transform.position += moveDirection * speed * Time.deltaTime;
    }
}
