using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.U2D;

public class PlayerAnimator : EntityAnimator
{
    [Header("Weapon Animations")]
    [SerializeField] private Transform weaponPivot;
    [SerializeField] private Animator weaponAnimator;
    [SerializeField] private GameObject activeWeapon;
    [SerializeField] private GameObject concealedWeapon;
    [Header("Trail")]
    [SerializeField] private Transform trailTip;
    [SerializeField] private TrailRenderer trailRenderer;

    private int nextAnimIndex = -1;

    public override void Update()
    {
        // Weapon mouse follow section

        Vector2 aimDirection = GameUtils.instance.cursorWorldLocation;
        float targetAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);

        weaponPivot.rotation = targetRotation;

        if (math.sign(aimDirection.x) < 0) weaponPivot.transform.localScale = new Vector3(1, -1, 1);
        else weaponPivot.transform.localScale = new Vector3(1, 1, 1);

        base.Update(); // Regular EntityAnimator behavior
    }

    // Melee slash animations are all the same, and are managed from here, later though

    public void AlignWeapons(WeaponSO weaponInUse, WeaponSO otherWeapon)
    {
        activeWeapon.transform.localPosition = new Vector3(0, weaponInUse.inUseGripOffset, 0);
        concealedWeapon.transform.localPosition = new Vector3(0, otherWeapon.concealedGripOffset, 0);
    }

    public void SwapWeapons(WeaponSO weaponInUse, WeaponSO otherWeapon)
    {
        activeWeapon.GetComponent<SpriteRenderer>().sprite = weaponInUse.texture;
        concealedWeapon.GetComponent<SpriteRenderer>().sprite = otherWeapon.texture;
    }

    // You can add custom animations for each weapon but they need to exist in the curre-
    // -ntly shared AnimationController on the weapon GO
    public void TriggerWeaponAnimation(WeaponSO weaponInUse, WeaponSO otherWeapon, bool isThrow)
    {
        SwapWeapons(weaponInUse, otherWeapon);
        AlignWeapons(weaponInUse, otherWeapon);

        AnimationClip[] animations;
        AudioClip[] audio;

        if (isThrow && weaponInUse is LightWeaponSO lightWeapon)
        {
            animations = lightWeapon.throwAnimations;
            audio = lightWeapon.throwSounds;
        }

        else
        {
            animations = weaponInUse.slashAnimations;
            audio = weaponInUse.slashSounds;
        }

        nextAnimIndex++;
        if (nextAnimIndex >= animations.Length) nextAnimIndex = 0;
        AnimationClip selectedAnim = animations[nextAnimIndex];
        weaponAnimator.Play(selectedAnim.name, layer: 0, normalizedTime: 0f);
        int audioIndex = UnityEngine.Random.Range(0, audio.Length);

        GameUtils.instance.audioSource.PlayOneShot(audio[audioIndex]);

        StartCoroutine(
            PlaySlashTrail(weaponPivot, weaponInUse.slashRadius, weaponInUse.slashAngle, selectedAnim.length
            ));

        IEnumerator stopAnimation()
        {
            yield return new WaitForSecondsRealtime(animations[nextAnimIndex].length * (1 / weaponAnimator.speed));

            weaponAnimator.Play("Empty", layer: 0, normalizedTime: 0f);
        }

        StartCoroutine(stopAnimation());
    }

    public IEnumerator PlaySlashTrail(Transform pivot, float radius, float angleDegrees, float duration)
    {
        Vector3 frozenForward = pivot.right;
        Vector3 origin = pivot.position;

        float startAngle = -angleDegrees / 2f;
        float endAngle = angleDegrees / 2f;

        Vector3 startDir = Quaternion.Euler(0, 0, startAngle) * frozenForward;
        trailTip.position = origin + startDir * radius;

        trailRenderer.widthMultiplier = radius;
        trailRenderer.Clear();
        trailTip.gameObject.SetActive(true);
        trailRenderer.emitting = true;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float linearT = elapsed / duration;
            float easedT = EaseOutQuart(linearT);

            float currentAngle = Mathf.Lerp(startAngle, endAngle, easedT);

            Vector3 dir = Quaternion.Euler(0, 0, currentAngle) * frozenForward;
            trailTip.position = origin + dir * (1.5f * radius / 2);

            yield return null;
        }

        trailRenderer.emitting = false;
        trailTip.gameObject.SetActive(false);
    }

    private float EaseOutQuart(float t) => 1f - Mathf.Pow(1f - t, 4);
}
