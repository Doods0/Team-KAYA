using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public SpriteRenderer fullBar;
    public Transform maskTransform;

    float maskOriginalXPosition;


    void Start() => maskOriginalXPosition = maskTransform.localPosition.x;

    void FixedUpdate()
    {
        PlayerController player = GameStatus.instance.playerController;
        float healthPercentage = (float)player.health / player.maxHealth;

        maskTransform.localPosition = new Vector3(maskOriginalXPosition * healthPercentage, 0, 0);
    }

}
