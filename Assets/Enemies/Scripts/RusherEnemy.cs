using UnityEngine;

public class RusherEnemy : EnemyController
{
    [Header("Rusher Enemy")]
    public float startRushRange;
    public float endRushRange;
    public float acceleration;
    bool rushing = false;
    // Since we need to stop moving when the game is paused, we need to remember the speed at
    // which we were going before pausing.
    Vector3 storedSpeed = Vector3.zero;

    public override void FixedUpdate()
    {
        if (GameManager.instance.timeScale == 0)
        {
            if (storedSpeed == Vector3.zero) storedSpeed = rigidbody.linearVelocity;
            rigidbody.linearVelocity = Vector3.zero;
        }
        else if (storedSpeed != Vector3.zero)
        {
            rigidbody.linearVelocity = storedSpeed;
            storedSpeed = Vector3.zero;
        }

        float distanceToPlayer = ToPlayer().magnitude;

        if (distanceToPlayer >= endRushRange) rushing = false;
        else if (distanceToPlayer <= startRushRange && !rushing)
        {
            rushing = true;
            rigidbody.linearVelocity = ToPlayer().normalized * ScaledSpeed();
        }

        if (rushing) rigidbody.linearVelocity *= 1 + acceleration / 1000;
        else if (rigidbody.linearVelocity.magnitude > speed * 1.1 * GameManager.instance.timeScale)
        {
            rigidbody.linearVelocity /= 1 + acceleration / 100;
        }
        else rigidbody.linearVelocity = ToPlayer().normalized * ScaledSpeed();


        ReactToKnockback();
    }
}
