using UnityEngine;

public class ExploderEnemyAnimator : EntityAnimator
{
    [Header("Sounds")]
    [SerializeField] private AudioClip hissingSound;
    [SerializeField] private AudioClip explosionSound;

    [Header("Particles")]
    [SerializeField] private ParticleSystem hissingParticle;
    [SerializeField] private ParticleController explosionParticle;
    [SerializeField] private string explosionParticleId;

    private AudioClip hissing;

    public void Explode()
    {
        hissingParticle.Stop();

        GameUtils.instance.audioSource.PlayOneShot(explosionSound);

        IPoolable particle = GameManager.instance.GetFromPool(explosionParticleId) ?? Instantiate(explosionParticle);
        if (!(particle is ParticleController controller)) return;
        controller.Initialize(transform.position, explosionParticleId);
    }

    public void StartHissing()
    {
        hissingParticle.Play();
        GameUtils.instance.audioSource.PlayOneShot(hissingSound);
    }
}
