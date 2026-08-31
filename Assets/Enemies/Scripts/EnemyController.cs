using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed;
    public int damage;

    private Rigidbody2D rigidbody;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

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
}
