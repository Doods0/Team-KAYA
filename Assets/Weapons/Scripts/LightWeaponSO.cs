using System;
using UnityEngine;

// To create a special light weapon, please inherit from this SO

[CreateAssetMenu(menuName = "Weapons/Basic Light Weapon")]
public class LightWeaponSO : WeaponSO
{
    [Header("Throw")]
    [Header("Stats")]
    public ThrowStats throwStats;

    [Header("Animations and Sounds")]
    public AnimationClip[] throwAnimations; // need to exist already in the AnimationController
    public AudioClip[] throwSounds;

    [Header("Projectile")]
    public GameObject projectile;
    public Sprite projectileTexture;
    public string projectileId;
    public bool projectileSpins;

    public virtual void Throw(WeaponsBuffs buffs)
    {
        Vector3 currentPos = GameUtils.instance.playerPosition;
        Vector3 direction = CameraController.cursorDirectionVector;
        float angleToDirection = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        // PROJECTILE MUST FACE UP IN ART

        GameObject proj = GameManager.instance.GetFromPool(projectileId);
        if (!proj) proj = Instantiate(projectile);

        proj.SetActive(true);
        proj.transform.position = currentPos;
        proj.transform.rotation = Quaternion.Euler(0f, 0f, angleToDirection);
        proj.transform.localScale = Vector3.one * throwStats.projectileSize;

        ProjectileController controller = proj.GetComponent<ProjectileController>();

        controller.id = projectileId;
        controller.damage = throwStats.throwDamage;
        controller.speed = throwStats.projectileSpeed;
        controller.knockback = throwStats.throwKnockback;
        controller.moveDirection = direction;
        controller.lifetime = throwStats.projectileLifetime;
        controller.spins = projectileSpins;
        controller.renderer.sprite = projectileTexture;
        controller.isEnemy = false;
    }
}

[Serializable]
public class ThrowStats // Define addition of two of those
{
    public int throwDamage;
    public float throwKnockback;
    public float throwCooldown;
    public float projectileLifetime;
    public int projectileSpeed;
    public float projectileSize;
}