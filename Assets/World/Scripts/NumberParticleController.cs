using TMPro;
using UnityEngine;

public class NumberParticleController : MonoBehaviour, IPoolable
{
    private string id;

    private TextMeshPro textComponent;
    private Color textColor;

    [Header("Settings")]
    private readonly float moveSpeed = 1.5f;
    private readonly float fadeSpeed = 0.5f;

    private void Awake()
    {
        textComponent = GetComponent<TextMeshPro>();
        textColor = textComponent.color;
    }

    public void Initialize(Vector3 position, string poolId, int damageAmount) 
    {
        gameObject.SetActive(true);
        transform.position = position;
        transform.rotation = Quaternion.identity;
        id = poolId;

        textColor.a = 1;
        textComponent.text = damageAmount.ToString();

    }

    void Update()
    {
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        textColor.a -= fadeSpeed * Time.deltaTime;
        textComponent.color = textColor;

        if (textColor.a <= 0)
        {
            GameManager.instance.AddInPool(id, this);
            gameObject.SetActive(false);
        }
    }
}
