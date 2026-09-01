using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed;
    public int damage;
    public int health;

    private Rigidbody2D rigidbody;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Vector3 playerPosition = GameStatus.instance.playerTransform.position;
        Vector3 position = transform.position;
        Vector3 moveDirection = (playerPosition - position).normalized * speed * Time.deltaTime;

        rigidbody.linearVelocityX = moveDirection.x;
        rigidbody.linearVelocityY = moveDirection.y;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
        if (playerController == null)
        {
            return;
        }

        playerController.TakeDamage(damage);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0) Destroy(gameObject);
    }
}
