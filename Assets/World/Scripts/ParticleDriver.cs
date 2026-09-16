using UnityEngine;

public class ParticleDriver : MonoBehaviour
{
    [HideInInspector] public string id;
    private ParticleSystem particleSys;

    private void Awake() => particleSys = GetComponent<ParticleSystem>();
    private void OnEnable() => particleSys.Play();
    private void OnParticleSystemStopped() 
    {
        gameObject.SetActive(false);
        GameManager.instance.AddInPool(id, gameObject);
    }
}
