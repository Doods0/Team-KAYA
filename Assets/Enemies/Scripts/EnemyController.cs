using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float knockbackDecayRate = 4f;

    [Header("Stats")]
    public float speed;
    public int damage;
    public int health;

    [HideInInspector] public Rigidbody2D rigidbody;
    private Vector2 knockbackVelocity;

    private void Awake() => rigidbody = GetComponent<Rigidbody2D>();

    private void FixedUpdate()
    {
        knockbackVelocity *= Mathf.Exp(-knockbackDecayRate * Time.deltaTime);

        Vector2 playerPosition = (Vector2)GameUtils.instance.playerPosition;
        Vector2 position = rigidbody.position;
        Vector2 moveDirection = (playerPosition - position).normalized;

        rigidbody.linearVelocity = moveDirection * speed + knockbackVelocity;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        PlayerStats stats = other.gameObject.GetComponent<PlayerStats>();
        if (stats == null) return;
        stats.TakeDamage(damage);
    }

    public void ApplyKnockback(Vector2 impulse) => knockbackVelocity += impulse;

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            GameManager.instance.SpeedTime();
            Destroy(gameObject); // To be replaced by enemy pooling
        }
    }
}
