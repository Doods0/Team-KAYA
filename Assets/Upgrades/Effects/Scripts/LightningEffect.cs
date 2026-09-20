using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Effects/Lightning")]
public class LightningEffect : Effect
{
    [SerializeField] private GameObject lightningObject;
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

            GameObject lightning = GameManager.instance.GetFromPool(lightningObjectId);
            if (lightning == null) lightning = Instantiate(lightningObject);
            else lightning.SetActive(true);

            lightning.transform.position = hitGO.transform.position;
            lightning.transform.rotation = Quaternion.Euler(0, 0, Random.Range(-30, 30));

            EnemyController enemyController = hitGO.GetComponent<EnemyController>();
            enemyController.TakeDamage(damage);

            GameManager.instance.StartCoroutine(ScheduleDeletion(lightning));
            GameUtils.instance.audioSource.PlayOneShot(lightningSound);
        }
    }

    private IEnumerator ScheduleDeletion(GameObject lightningObject)
    {
        yield return new WaitForSeconds(0.5f);

        lightningObject.SetActive(false);
        GameManager.instance.AddInPool(lightningObjectId, lightningObject);
    }
}
