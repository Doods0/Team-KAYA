using UnityEngine;
using UnityEngine.UIElements;

// To create a special light weapon, please inherit from this SO

[CreateAssetMenu(menuName = "Weapons/Basic Light Weapon")]
public class LightWeaponSO : WeaponSO
{
    [Header("Throw")]
    [Header("Stats")]
    public int throwDamage;
    public float throwKnockback;
    public float throwCooldown;

    [Header("Animations and Sounds")]
    public AnimationClip[] throwAnimations; // need to exist already in the AnimationController
    public AudioClip[] throwSounds;

    [Header("Projectile")]
    public GameObject projectile;
    public Sprite projectileTexture;
    public string projectileId;
    public bool projectileSpins;
    public float projectileLifetime;
    public int projectileSpeed;
    public float projectileSize;


    public virtual void Throw()
    {
        Vector3 currentPos = GameUtils.instance.playerPosition;
        Vector3 direction = CameraController.cursorDirectionVector;
        float angleToDirection = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        // PROJECTILE MUST FACE UP IN ART

        GameManager manager = GameManager.instance;

        GameObject proj = GameManager.instance.GetFromPool(projectileId);
        if (proj == null) proj = Instantiate(projectile);

        proj.SetActive(true);
        proj.transform.position = currentPos;
        proj.transform.rotation = Quaternion.Euler(0f, 0f, angleToDirection);
        proj.transform.localScale = Vector3.one * projectileSize;

        ProjectileController controller = proj.GetComponent<ProjectileController>();

        controller.id = projectileId;
        controller.damage = throwDamage;
        controller.speed = projectileSpeed;
        controller.knockback = throwKnockback;
        controller.moveDirection = direction;
        controller.lifetime = projectileLifetime;
        controller.spins = projectileSpins;
        controller.renderer.sprite = projectileTexture;
        controller.isEnemy = false;
    }
}
