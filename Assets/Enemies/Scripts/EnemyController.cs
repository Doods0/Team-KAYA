using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Settings")]
    public float knockbackDecayRate = 4f;

    [Header("Stats")]
    public float speed;
    public int damage;
    public int health;
    public int maxHealth;

    [Header("Sounds")]
    public AudioClip hurt;
    public AudioClip death;

    [HideInInspector] public Rigidbody2D rigidbody;
    [HideInInspector] public Vector2 knockbackVelocity;
    [HideInInspector] public string id;

    public virtual void Awake() => rigidbody = GetComponent<Rigidbody2D>();

    public virtual void FixedUpdate()
    {
        rigidbody.linearVelocity = ToPlayer().normalized * speed * GameManager.instance.timeScale;
        ReactToKnockback();
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
            GameUtils.instance.audioSource.PlayOneShot(death, Random.Range(.75f, 1.25f));

            GameManager.instance.SpeedTime();
            GameManager.instance.AddInPool(id, gameObject);
            gameObject.SetActive(false);

        }
        else GameUtils.instance.audioSource.PlayOneShot(hurt, Random.Range(.75f, 1.25f));
    }

    // If more functions like this are created,
    // group them in a big function called: "EnemyBehaviour" or something.
    public void ReactToKnockback()
    {
        knockbackVelocity *= Mathf.Exp(-knockbackDecayRate * Time.deltaTime);
        rigidbody.linearVelocity += knockbackVelocity * GameManager.instance.timeScale;
    }

    public Vector2 ToPlayer()
    {
        Vector2 playerPosition = (Vector2)GameUtils.instance.playerPosition;
        Vector2 position = rigidbody.position;

        return playerPosition - position;
    }
}
