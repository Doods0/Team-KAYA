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
        Vector3 position = this.transform.position;
        Vector3 moveDirection = (playerPosition - position).normalized;

        print(moveDirection * speed);

        this.transform.position += moveDirection * speed * Time.deltaTime;
        // this.transform.position += new Vector3(100, 100, 0);
    }
}
