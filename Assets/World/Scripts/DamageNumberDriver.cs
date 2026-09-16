using TMPro;
using UnityEngine;

public class DamageNumberDriver : MonoBehaviour
{
    [HideInInspector] public string id;

    private TextMeshPro textComponent;
    private Color textColor;

    [Header("Settings")]
    public float moveSpeed = 1.5f;
    public float fadeSpeed = 2f;
    public float lifetime = 1f;

    private void Awake()
    {
        textComponent = GetComponent<TextMeshPro>();
        textColor = textComponent.color;
    }
    private void OnEnable() => textColor.a = 1;
    public void Setup(int damageAmount) => textComponent.text = damageAmount.ToString();

    void Update()
    {
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        textColor.a -= fadeSpeed * Time.deltaTime;
        textComponent.color = textColor;

        if (textColor.a <= 0)
        {
            GameManager.instance.AddInPool(id, gameObject);
            gameObject.SetActive(false);
        }
    }
}
