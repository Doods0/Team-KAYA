using System.Collections;
using UnityEngine;

public class StillVFXController : MonoBehaviour, IPoolable
{
    public void Initialize(Vector3 position, string id, float rotationRange = 0, float effectDuration = 0.25f)
        => StartCoroutine(InitializeIE(position, id, rotationRange, effectDuration));

    public IEnumerator InitializeIE(Vector3 position, string id, float rotationRange, float effectDuration)
    {
        gameObject.SetActive(true);
        transform.position = position;
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(-rotationRange, rotationRange));

        yield return new WaitForSeconds(effectDuration);

        gameObject.SetActive(false);
        GameManager.instance.AddInPool(id, this);
    }
}
