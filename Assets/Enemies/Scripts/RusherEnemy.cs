using UnityEngine;

public class RusherEnemy : EnemyController
{
    [Header("Rusher Enemy")]
    public float startRushRange;
    public float endRushRange;
    public float acceleration;
    bool rushing = false;

    public override void FixedUpdate()
    {
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
