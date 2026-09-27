using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Effects/Lightning")]
public class LightningEffect : Effect
{
    [SerializeField] private StillVFXController lightningObject;
    [SerializeField] private string lightningObjectId;
    [SerializeField] private float range;
    [SerializeField] private AudioClip lightningSound;

    public override void ApplyEffect(int intensity, int damage, LocalWeaponsData weaponData)
    {
        List<Collider2D> hitsBuffer = weaponData.hitsBuffer;
        hitsBuffer.Clear();

        int count = Physics2D.OverlapCircle(GameUtils.instance.playerPosition, range, weaponData.enemyFilter, hitsBuffer);
        for (int i = 0; i < Mathf.Clamp(intensity, 0, count); i++)
        {
            Collider2D hitCollider = hitsBuffer[i];
            GameObject hitGO = hitCollider.gameObject;

            IPoolable lightning = GameManager.instance.GetFromPool(lightningObjectId) ?? Instantiate(lightningObject);
            if (!(lightning is StillVFXController vfxController)) return;
            vfxController.Initialize(GameUtils.instance.playerPosition, lightningObjectId, 30f);

            if (!(hitGO.TryGetComponent(out EnemyController enemyController))) return;
            enemyController.TakeDamage(damage);

            GameUtils.instance.audioSource.PlayOneShot(lightningSound);
        }
    }

}
