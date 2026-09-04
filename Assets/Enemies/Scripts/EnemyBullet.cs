using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public Vector3 moveDirection;
    public float speed;
    public int damage;
    public float torque;


    Rigidbody2D rigidBody;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();

        moveDirection.Normalize();

        rigidBody.linearVelocityX = moveDirection.x * speed;
        rigidBody.linearVelocityY = moveDirection.y * speed;
        rigidBody.AddTorque(torque);
    }

    void FixedUpdate()
    {
        if ((transform.position - GameUtils.instance.playerTransform.position).magnitude > 30)
        {
            Destroy(gameObject);
        }
    }


    void OnCollisionEnter2D(Collision2D other)
    {
        PlayerStats playerController = other.gameObject.GetComponent<PlayerStats>();
        if (playerController == null) return;

        playerController.TakeDamage(damage);
        Destroy(gameObject);
    }
}
