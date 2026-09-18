using UnityEngine;

public class ExploderEnemyAnimator : EntityAnimator
{
    [Header("Sounds")]
    [SerializeField] private AudioClip hissingSound;
    [SerializeField] private AudioClip explosionSound;

    [Header("Particles")]
    [SerializeField] private ParticleSystem hissingParticle;
    [SerializeField] private GameObject explosionParticle;
    [SerializeField] private string explosionParticleId;

    private AudioClip hissing;

    public void Explode()
    {
        hissingParticle.Stop();

        GameUtils.instance.audioSource.PlayOneShot(explosionSound);

        GameObject particle = GameManager.instance.GetFromPool(explosionParticleId);
        if (particle == null) particle = Instantiate(explosionParticle);
        else particle.SetActive(true);
        particle.transform.position = transform.position;
        particle.transform.rotation = Quaternion.identity;
        ParticleDriver driver = particle.GetComponent<ParticleDriver>();
        driver.id = explosionParticleId;
    }

    public void StartHissing()
    {
        hissingParticle.Play();
        GameUtils.instance.audioSource.PlayOneShot(hissingSound);
    }
}
