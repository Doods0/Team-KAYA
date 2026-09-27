using UnityEngine;

public class ParticleController : MonoBehaviour, IPoolable
{
    private string id;
    private ParticleSystem particleSys;

    private void Awake() => particleSys = GetComponent<ParticleSystem>();

    public void Initialize(Vector3 position, string particleId)
    {
        gameObject.SetActive(true);
        particleSys.Play();

        transform.position = position;
        transform.rotation = Quaternion.identity;
        id = particleId;
    }

    private void OnParticleSystemStopped()
    {
        gameObject.SetActive(false);
        GameManager.instance.AddInPool(id, this);
    }
}
